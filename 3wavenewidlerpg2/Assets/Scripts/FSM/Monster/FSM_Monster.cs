using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Spine;

public class Monster_Idle : FSM
{
    Monster Monster = null;
    public Monster_Idle(Monster _monster) { Monster = _monster; }

    public override void Enter()
    {
        Monster.SkeletonAnim.AnimationState.SetAnimation(0, "idle", true);
        Monster.SkeletonAnim.timeScale = 1.0f;
        Monster.UpdateSkeletonAnimSortingOrder();
    }

    public override void Update()
    {
        // 일반 몬스터는 공격 없이 공격 범위에서 멈춰 섬
        if (Monster.IsTargetInRange(Monster.MonsterData.Monster_Detection_Range)
            && !(Monster.MonsterData.Monster_Type == MonsterType.NormalMonster && Monster.IsTargetInRange(Monster.MonsterData.Monster_Attack_Range)))
            Monster.ChangeState(State.Move);
    }
}

public class Monster_Move : FSM
{
    Monster Monster = null;
    public Monster_Move(Monster _monster) { Monster = _monster; }

    public override void Enter()
    {
        Monster.SkeletonAnim.AnimationState.SetAnimation(0, "move", true);
        Monster.SkeletonAnim.timeScale = 1.0f;
    }

    public override void Update()
    {
        if (!Monster.IsTargetInRange(Monster.MonsterData.Monster_Detection_Range))
        {
            Monster.ChangeState(State.Idle);
            return;
        }

        if (Monster.IsTargetInRange(Monster.MonsterData.Monster_Attack_Range))
        {
            Monster.ChangeState(Monster.MonsterData.Monster_Type == MonsterType.NormalMonster ? State.Idle : Monster.IsSkillReady ? State.Skill : State.Attack);
            return;
        }

        Vector3 vDir = (Monster.Target.transform.position - Monster.transform.position).normalized;
        Monster.transform.position += vDir * (float)Monster.Status.TotalMoveSpeed * Time.fixedDeltaTime;

        // 원본 아트가 오른쪽을 보고 있으므로 왼쪽 이동 시 반전
        Monster.SetFlip(vDir.x < 0 ? -1 : 1);
        Monster.UpdateSkeletonAnimSortingOrder();
    }
}

public class Monster_Dead : FSM
{
    const string AnimName = "dead";

    Monster Monster = null;
    public Monster_Dead(Monster _monster)
    {
        Monster = _monster;
        Monster.SkeletonAnim.AnimationState.Complete += this.HandleAnimComplete;
    }

    public override void Enter()
    {
        Monster.OnDeadEnter(Monster);
        Monster.HpBar.gameObject.SetActive(false);
        Monster.SkeletonAnim.AnimationState.SetAnimation(0, AnimName, false);
        Monster.SkeletonAnim.timeScale = 1.0f;
    }

    // 사망 모션이 끝나면 풀에 반환
    private void HandleAnimComplete(TrackEntry trackEntry)
    {
        if (Monster.State != State.Dead || trackEntry.Animation.Name.Equals(AnimName) == false)
            return;

        ObjectPoolManager.Instance.ReturnObject(Monster.MonsterData.Resource, Monster.gameObject);
    }
}

public class Monster_Attack : FSM
{
    const string AnimName = "attack";

    Monster Monster = null;
    public Monster_Attack(Monster _monster)
    {
        Monster = _monster;
        Monster.SkeletonAnim.AnimationState.Event += this.HandleAnimEvent;
        Monster.SkeletonAnim.AnimationState.Complete += this.HandleAnimComplete;
    }

    public override void Enter()
    {
        Attack();
    }

    void Attack()
    {
        Monster.SkeletonAnim.AnimationState.SetAnimation(0, AnimName, false);
        Monster.SkeletonAnim.timeScale = (float)Monster.Status.TotalAttackSpeed;
        Monster.SetFlip(Monster.Target.transform.position.x < Monster.transform.position.x ? -1 : 1);
    }

    void FireProjectile()
    {
        string key = $"{Monster.MonsterData.Resource}_projectile_{Monster.MonsterData.Skin_Name}";
        Projectile projectile = ObjectPoolManager.Instance.GetObject<Projectile>("Projectile", key);
        projectile.key = key;
        projectile.transform.position = Monster.Bone_ProjectileTr.position;
        projectile.Fire(Monster.Status.Attack, Monster, Monster.Target);
    }

    private void HandleAnimEvent(TrackEntry trackEntry, Spine.Event e)
    {
        if (Monster.State != State.Attack)
            return;

        if (e.Data.Name.Equals("projectile"))
            FireProjectile();
        // 근접 공격 (뿔 보스 창 찌르기)
        else if (e.Data.Name.Equals("hit") && Monster.IsTargetInRange(Monster.MonsterData.Monster_Attack_Range))
            Monster.Target.TakeDamage(Monster.Status.Attack, Monster, AttackType.Normal);
    }

    private void HandleAnimComplete(TrackEntry trackEntry)
    {
        if (Monster.State != State.Attack || trackEntry.Animation.Name.Equals(AnimName) == false)
            return;

        if (Monster.IsSkillReady)
            Monster.ChangeState(State.Skill);
        else if (Monster.IsTargetInRange(Monster.MonsterData.Monster_Attack_Range))
            Attack();
        else
            Monster.ChangeState(State.Idle);
    }
}

// 뿔 보스 땅 찍기 (ArtSource monster_dungeon_2001 HANDOFF 'skill 이펙트 엔진 연결'), 끝나면 idle
// 페이즈1: 주인공 발밑 돌가시, 페이즈2(체력 Challenge_2Phase_Hp_Per 이하): 꽂은 자리에서 8갈래 돌가시
public class Monster_Skill : FSM
{
    const string AnimName = "skill";
    const float PixelSize = 1f / 64f;
    const float SlamOffset = 50f * PixelSize;   // 창이 꽂히는 자리 = 보스 앞 50px 바닥
    const float SpikeRiseTime = 0.3f;           // 가시 이펙트 시작 후 솟는 순간 = 타격
    const float SpikeHitRange = 0.5f;           // 솟는 가시에 맞는 거리 (칸)
    const float SpikeInterval = 0.07f;          // 페이즈2 k번째 가시는 꽂은 뒤 0.07초 × k
    const int SpikeDirections = 8;
    const string RangeKey = "skill_range";      // 가시가 솟을 때까지 바닥에 타격 범위 (지름 2칸 그림 → SpikeHitRange 배율)
    const int RangeSortingOrder = -9998;        // 배경(-10000)·그림자(-9999) 위, 캐릭터 아래
    static readonly string[] SpikeKeys = { "spike_s1", "spike_s2", "spike_s3", "spike_s4" };   // 2개씩, 멀수록 큰 시트

    Monster Monster = null;
    bool isHit = false;     // 스킬 한 번에 한 번만 맞음

    public Monster_Skill(Monster _monster)
    {
        Monster = _monster;
        Monster.SkeletonAnim.AnimationState.Event += this.HandleAnimEvent;
        Monster.SkeletonAnim.AnimationState.Complete += this.HandleAnimComplete;
    }

    bool IsPhase2 { get { return Monster.CurHp.ToDouble() / Monster.GetTotalHp().ToDouble() <= DataManager.Instance.SystemData.GetFloat("Challenge_2Phase_Hp_Per"); } }

    public override void Enter()
    {
        isHit = false;
        Monster.SkillReadyTime = Time.time + DataManager.Instance.SystemData.GetFloat("Challenge_Skill_Cooltime");
        Monster.SkeletonAnim.AnimationState.SetAnimation(0, AnimName, false);
        Monster.SkeletonAnim.timeScale = 1.0f;
        Monster.SetFlip(Monster.Target.transform.position.x < Monster.transform.position.x ? -1 : 1);
    }

    private void HandleAnimEvent(TrackEntry trackEntry, Spine.Event e)
    {
        if (Monster.State != State.Skill)
            return;

        if (e.Data.Name.Equals("spike") && !IsPhase2)
            PlaySpike("earth_spike", Monster.Target.transform.position);
        else if (e.Data.Name.Equals("slam"))
        {
            float flip = Monster.SkeletonAnim.Skeleton.ScaleX;
            Vector3 slamPosition = Monster.transform.position + Vector3.right * flip * SlamOffset;
            // 먼지가 보스 발을 덮게 보스 앞에
            PlayEffect("ground_slam", slamPosition, flip, Monster.SkeletonRenderer.sortingOrder + 1);
            if (IsPhase2)
                Monster.StartCoroutine(CoPhase2Spikes(slamPosition));
        }
    }

    // k번째 가시 = 꽂은 자리 + (cos, sin × 0.5) × (18 + 40k)px, 화면 밖으로 나간 방향은 멈춤
    IEnumerator CoPhase2Spikes(Vector3 _origin)
    {
        bool[] isStopped = new bool[SpikeDirections];
        for (int k = 1; isStopped.Contains(false); k++)
        {
            yield return new WaitForSeconds(SpikeInterval);
            float distance = (18f + 40f * k) * PixelSize;
            for (int d = 0; d < SpikeDirections; d++)
            {
                float angle = d * Mathf.PI * 2f / SpikeDirections;
                Vector3 position = _origin + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle) * 0.5f) * distance;
                isStopped[d] |= !InGameManager.Instance.BattleArea.Contains(position);
                if (!isStopped[d])
                    PlaySpike(SpikeKeys[Mathf.Min((k - 1) / 2, SpikeKeys.Length - 1)], position);
            }
        }
    }

    // 가시는 화면 위쪽일수록 뒤 (보스 발보다 위는 보스 뒤, 아래는 앞)
    void PlaySpike(string _key, Vector3 _position)
    {
        Effect range = ObjectPoolManager.Instance.GetObject<Effect>("Effect", RangeKey);
        range.key = RangeKey;
        range.Play(_position, Vector3.one * SpikeHitRange, RangeSortingOrder);
        PlayEffect(_key, _position, 1f, Mathf.RoundToInt(-_position.y * 100));
        Monster.StartCoroutine(CoSpikeHit(_position));
    }

    IEnumerator CoSpikeHit(Vector3 _position)
    {
        yield return new WaitForSeconds(SpikeRiseTime);
        if (isHit || Monster.State == State.Dead || Monster.Target.State == State.Dead
            || Vector2.Distance(_position, Monster.Target.transform.position) > SpikeHitRange)
            yield break;

        isHit = true;
        Monster.Target.TakeDamage(Monster.Status.Attack * DataManager.Instance.SystemData.GetFloat("Challenge_Skill_Per"), Monster, AttackType.Normal);
    }

    void PlayEffect(string _key, Vector3 _position, float _flip, int _sortingOrder)
    {
        string key = $"{Monster.MonsterData.Resource}_{_key}";
        Effect effect = ObjectPoolManager.Instance.GetObject<Effect>("Effect", key);
        effect.key = key;
        effect.Play(_position, new Vector3(_flip, 1f, 1f), _sortingOrder);
    }

    private void HandleAnimComplete(TrackEntry trackEntry)
    {
        if (Monster.State != State.Skill || trackEntry.Animation.Name.Equals(AnimName) == false)
            return;

        Monster.ChangeState(State.Idle);
    }
}
