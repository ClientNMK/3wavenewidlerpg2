using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MainCharacter : CharacterBase
{
    public JoystickController Controller = null;
    public Hood Hood = null;
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
        InitSkillSlots();

        bInitialized = true;
    }

    public override void SetupStatus(bool bRefill = false)
    {
        if (GameManager.Instance.gameType == GameType.InGame)
            Status = InGameBuffManager.Instance.BuffStatus.TotalStatus;

        if (bRefill)
            CurHp = GetTotalHp();
    }

    // 전투 영역(멀리 볼 때 화면) 안이거나 공격 범위 안에 있는 몬스터 중 가장 가까운 몬스터 (후드가 삼키는 중인 몬스터 제외)
    public Monster FindTarget()
    {
        Monster target = null;
        float minDist = float.MaxValue;
        foreach (Monster monster in InGameManager.Instance.Monsters)
        {
            if (monster.State == State.Pause)
                continue;

            float dist = Vector2.Distance(transform.position, monster.transform.position);
            if (!InGameManager.Instance.BattleArea.Contains(monster.transform.position) && dist > Status.Attack_Range)
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
            .Where(x => x.State != State.Pause && Vector2.Distance(center, x.transform.position) <= splashRange)
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

            // 맞을 때마다 공격력 대비 최소~최대 배율 사이 랜덤 데미지
            BigDouble damage = _damage * Random.Range((float)Status.Damage_Min_Per, (float)Status.Damage_Max_Per);
            if (TryPredation(target, damage))
                continue;

            target.TakeDamage(damage, this, AttackType.Normal);
            target.PlayHitReaction(-side, _hitStopTime, _knockback);

            // 여러 번 맞아도 겹치지 않게 체력바 위에서 좌우로 살짝 흩뿌림
            DamageText damageText = ObjectPoolManager.Instance.GetObject<DamageText>("Effect", DamageTextKey);
            damageText.key = DamageTextKey;
            damageText.Play(new Vector3(target.transform.position.x + Random.Range(-0.15f, 0.15f), target.HpBar.transform.position.y + 0.15f, 0f), damage);
        }
        InGameManager.Instance.HitStop(_hitStopTime, _shake);
    }

    // 평타로 포식 상태이상(Predation_Per)이 뜨면, 이번 타격 후 체력이 후드 스탯 비율 이하가 될 일반 몬스터를 데미지 대신 후드가 잡아먹음 (보스 제외)
    bool TryPredation(Monster _target, BigDouble _damage)
    {
        if (State != State.Attack || Hood.IsDevouring || InGameManager.Instance.IsBossChallenge || Random.value >= Status.Predation_Per)
            return false;

        double hoodHpPer = DataManager.Instance.StatData.GetStatData(StatType.Hood_Predation_Hp_Per).Stat_Default_Value;
        if (_target.CurHp - _damage > _target.GetTotalHp() * hoodHpPer)
            return false;

        Hood.Devour(_target);
        return true;
    }

    // 스킬 슬롯 (SkillId 0 = 빈 칸)
    public class SkillSlot
    {
        public int SkillId = 0;
        public float LastUseTime = 0f;
        public bool IsAuto = true;      // 슬롯 버튼으로 켜고 끔

        public SkillData SkillData { get { return DataManager.Instance.SkillData.GetSkillData(SkillId); } }
        public float RemainTime { get { return Mathf.Max(0f, SkillData.Cooltime - (Time.time - LastUseTime)); } }
    }

    const int EquipSkillId = 20000;     // 스킬 장착 기능 전까지 1번 슬롯 고정
    public List<SkillSlot> SkillSlots = new();
    public SkillSlot CurSkillSlot = null;   // 지금 사용 중인 스킬

    void InitSkillSlots()
    {
        int slotMax = (int)DataManager.Instance.SystemData.GetFloat("Skill_Slot_Max");
        for (int i = 0; i < slotMax; i++)
            SkillSlots.Add(new SkillSlot());
        SkillSlots[0].SkillId = EquipSkillId;
    }

    // 자동 전투 중 앞 슬롯부터, 자동 사용이 켜져 있고 쿨타임이 찬 스킬 사용
    public bool TryUseSkill()
    {
        foreach (SkillSlot slot in SkillSlots)
        {
            if (slot.SkillId == 0 || !slot.IsAuto || slot.RemainTime > 0f)
                continue;

            slot.LastUseTime = Time.time;
            CurSkillSlot = slot;
            ChangeState(State.Skill);
            return true;
        }
        return false;
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
