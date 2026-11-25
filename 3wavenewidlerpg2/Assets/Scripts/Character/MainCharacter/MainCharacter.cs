using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MainCharacter : CharacterBase
{
    public JoystickController Controller = null;
    public Vector3 RollDirection = Vector3.zero;
    float lastRollTime = 0f;

    public override void Init()
    {
        if (bInitialized)
            return;

        CharacterType = CharacterType.MainCharacter;

        FSMMain = new FSM[(int)State.Dead + 1];
        FSMMain[(int)State.Idle] = new MainCharacter_Idle(this);
        FSMMain[(int)State.Move] = new MainCharacter_Move(this);
        FSMMain[(int)State.Dash] = new MainCharacter_Roll(this);
        FSMMain[(int)State.Attack] = new MainCharacter_Attack(this);
        FSMMain[(int)State.Skill] = new MainCharacter_Skill(this);
        FSMMain[(int)State.Hit] = new MainCharacter_Hit(this);
        FSMMain[(int)State.Dead] = new MainCharacter_Dead(this);

        if (Controller == null)
            Controller = InGameManager.Instance.UIInGameScene.JoystickController;
        Controller.onDrag += this.HandleDrag;
        Controller.onDragEnd += this.HandleDragEnd;

        bInitialized = true;
    }

    public override void SetupStatus(bool bRefill = false)
    {
        if (GameManager.Instance.gameType == GameType.InGame)
            Status = InGameBuffManager.Instance.BuffStatus.TotalStatus;

        if (bRefill)
            CurHp = GetTotalHp();
    }

    // 화면에 보이거나 공격 범위 안에 있는 몬스터 중 가장 가까운 몬스터
    public Monster FindTarget()
    {
        Monster target = null;
        float minDist = float.MaxValue;
        foreach (Monster monster in InGameManager.Instance.Monsters)
        {
            float dist = Vector2.Distance(transform.position, monster.transform.position);
            Vector3 viewPos = InGameManager.Instance.MainCamera.WorldToViewportPoint(monster.transform.position);
            bool isVisible = viewPos.x >= 0f && viewPos.x <= 1f && viewPos.y >= 0f && viewPos.y <= 1f;
            if (!isVisible && dist > Status.Attack_Range)
                continue;

            if (dist < minDist)
            {
                minDist = dist;
                target = monster;
            }
        }
        return target;
    }

    // 목표 몬스터 중심 Attack_Splash_Range 안에서 가까운 순으로 _count마리 (처치로 목록이 바뀌어도 되도록 복사본)
    public List<Monster> FindAttackTargets(int _count)
    {
        Vector3 center = Target.transform.position;
        float splashRange = DataManager.Instance.SystemData.GetFloat("Attack_Splash_Range");
        return InGameManager.Instance.Monsters
            .Where(x => Vector2.Distance(center, x.transform.position) <= splashRange)
            .OrderBy(x => Vector2.Distance(center, x.transform.position))
            .Take(_count)
            .ToList();
    }

    const string HitSparkKey = "hit_spark";
    const string DamageTextKey = "DamageText";
    const float HitSparkOffset = 19f / 64f; // 몬스터 기준점에서 공격자 쪽 앞면 바닥

    // 캐릭터 크기에 맞춰 이펙트 재생 (_flip: 1 = 그림 방향 그대로, -1 = 좌우 반전)
    public void PlayEffect(string _key, Vector3 _position, float _flip, int _sortingOrder)
    {
        Effect effect = ObjectPoolManager.Instance.GetObject<Effect>("Effect", _key);
        effect.key = _key;
        Vector3 scale = transform.localScale;
        scale.x *= _flip;
        effect.Play(_position, scale, _sortingOrder);
    }

    // 공격 범위 안 _count마리에 데미지 + 피격 연출, 히트스톱·흔들림은 한 번 (밀림·흔들림은 px)
    public void HitTargets(BigDouble _damage, int _count, float _hitStopTime, float _knockback, float _shake, bool _spark)
    {
        foreach (Monster target in FindAttackTargets(_count))
        {
            float side = transform.position.x < target.transform.position.x ? -1f : 1f;
            if (_spark)
            {
                Vector3 sparkPosition = target.transform.position + Vector3.right * side * HitSparkOffset * target.transform.localScale.x;
                PlayEffect(HitSparkKey, sparkPosition, SkeletonAnim.Skeleton.ScaleX, target.SkeletonRenderer.sortingOrder + 1);
            }

            target.TakeDamage(_damage, this, AttackType.Normal);
            target.PlayHitReaction(-side, _hitStopTime, _knockback);

            // 여러 번 맞아도 겹치지 않게 체력바 위에서 좌우로 살짝 흩뿌림
            DamageText damageText = ObjectPoolManager.Instance.GetObject<DamageText>("Effect", DamageTextKey);
            damageText.key = DamageTextKey;
            damageText.Play(new Vector3(target.transform.position.x + Random.Range(-0.15f, 0.15f), target.HpBar.transform.position.y + 0.15f, 0f), _damage);
        }
        InGameManager.Instance.HitStop(_hitStopTime, _shake);
    }

    const int SkillId = 20000;  // 스킬 장착 기능 전까지 고정
    float lastSkillTime = 0f;
    public SkillData SkillData { get { return DataManager.Instance.SkillData.GetSkillData(SkillId); } }

    public float SkillRemainTime { get { return Mathf.Max(0f, SkillData.Cooltime - (Time.time - lastSkillTime)); } }

    // 자동 전투 중 쿨타임이 차면 스킬 사용
    public bool TryUseSkill()
    {
        if (SkillRemainTime > 0f)
            return false;

        lastSkillTime = Time.time;
        ChangeState(State.Skill);
        return true;
    }

    public bool IsTargetInAttackRange()
    {
        return Vector2.Distance(transform.position, Target.transform.position) <= Status.Attack_Range;
    }

    public float RollRemainTime { get { return Mathf.Max(0f, DataManager.Instance.SystemData.GetFloat("Roll_Cooltime") - (Time.time - lastRollTime)); } }

    // 쿨타임이 차면 이동 방향으로 구르기
    public bool TryRoll(Vector3 _direction)
    {
        if (RollRemainTime > 0f)
            return false;

        lastRollTime = Time.time;
        RollDirection = _direction.normalized;
        ChangeState(State.Dash);
        return true;
    }

    // 공격할 몬스터가 없이 멈춰 있을 때만 피격 모션 재생 (피격 모션이 공격을 계속 끊지 않도록)
    public override void TakeDamage(BigDouble damageAmount, CharacterBase eventInstigator, AttackType attackType)
    {
        base.TakeDamage(damageAmount, eventInstigator, attackType);

        if (State == State.Idle && FindTarget() == null)
            ChangeState(State.Hit);
    }

    void HandleDrag()
    {
        if (Controller == null)
            return;

        if (Controller.InputDirection.magnitude < 0.1f)
            return;

        if (State == State.Dead || State == State.Dash || State == State.Skill)
            return;

        Target = null;
        ChangeState(State.Move);
    }

    void HandleDragEnd()
    {
        if (State == State.Dead)
            return;

        if (State != State.Move)
            return;

        ChangeState(State.Idle);
    }
}
