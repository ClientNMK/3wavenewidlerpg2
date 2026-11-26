using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;

public class MainCharacter_Idle : FSM
{
    MainCharacter MainCharacter = null;
    TrackEntry trackEntry = null;

    public MainCharacter_Idle(MainCharacter _MainCharacter) { MainCharacter = _MainCharacter; }
    public override void Enter()
    {
        SetAnimation();
    }
    public void SetAnimation()
    {
        trackEntry = MainCharacter.SkeletonAnim.AnimationState.GetCurrent(0);
        if (trackEntry == null || trackEntry.Animation == null || trackEntry.Animation.Name.Equals("idle") == false)
        {
            MainCharacter.SkeletonAnim.AnimationState.SetAnimation(0, "idle", true);
            MainCharacter.SkeletonAnim.timeScale = 1.0f;
            MainCharacter.UpdateSkeletonAnimSortingOrder();
        }
    }

    public override void Update()
    {
        MainCharacter.Target = MainCharacter.FindTarget();
        if (MainCharacter.Target == null || MainCharacter.TryUseSkill())
            return;

        MainCharacter.ChangeState(MainCharacter.IsTargetInAttackRange() ? State.Attack : State.Move);
    }
}
public class MainCharacter_Attack : FSM
{
    // 공격별 베기 이펙트와 타격 연출 (HANDOFF 기준, 밀림·흔들림은 px)
    class AttackInfo
    {
        public string AnimName;
        public string SlashKey;
        public float HitStopTime;
        public float Knockback;
        public float Shake;
    }
    static readonly AttackInfo[] Attacks =
    {
        new AttackInfo { AnimName = "attack", SlashKey = "attack_slash", HitStopTime = 0.1f, Knockback = 5f, Shake = 2f },
        new AttackInfo { AnimName = "attack2", SlashKey = null, HitStopTime = 0.1f, Knockback = 5f, Shake = 2f },   // 궤적이 그림에 포함
        new AttackInfo { AnimName = "attack3", SlashKey = "attack3_slash", HitStopTime = 0.12f, Knockback = 6f, Shake = 3f },
    };
    MainCharacter MainCharacter = null;
    AttackInfo attack = null;
    public MainCharacter_Attack(MainCharacter _MainCharacter)
    {
        MainCharacter = _MainCharacter;
        MainCharacter.SkeletonAnim.AnimationState.Event += this.HandleAnimEvent;
        MainCharacter.SkeletonAnim.AnimationState.Complete += this.HandleAnimComplete;
    }
    public override void Enter()
    {
        Attack();
    }

    void Attack()
    {
        attack = Attacks[Random.Range(0, Attacks.Length)];
        MainCharacter.SkeletonAnim.AnimationState.SetAnimation(0, attack.AnimName, false);
        MainCharacter.SkeletonAnim.timeScale = (float)MainCharacter.Status.TotalAttackSpeed;
        MainCharacter.SetFlip(MainCharacter.Target.transform.position.x < MainCharacter.transform.position.x ? -1 : 1);
    }

    private void HandleAnimEvent(TrackEntry trackEntry, Spine.Event e)
    {
        if (MainCharacter.State != State.Attack)
            return;

        if (e.Data.Name.Equals("slash"))
            MainCharacter.PlayEffect(attack.SlashKey, MainCharacter.transform.position, MainCharacter.SkeletonAnim.Skeleton.ScaleX, MainCharacter.SkeletonRenderer.sortingOrder + 1);
        else if (e.Data.Name.Equals("damage"))
            MainCharacter.HitTargets(MainCharacter.Status.Attack, (int)MainCharacter.Status.Attack_Number, attack.HitStopTime, attack.Knockback, attack.Shake, true);
    }

    private void HandleAnimComplete(TrackEntry trackEntry)
    {
        if (MainCharacter.State != State.Attack || trackEntry.Animation.Name.Equals(attack.AnimName) == false)
            return;

        MainCharacter.Target = MainCharacter.FindTarget();
        if (MainCharacter.Target == null)
            MainCharacter.ChangeState(State.Idle);
        else if (MainCharacter.TryUseSkill())
            return;
        else if (MainCharacter.IsTargetInAttackRange())
            Attack();
        else
            MainCharacter.ChangeState(State.Move);
    }
}
// 스킬: vanish → 잠깐 숨음 → 목표 몬스터 뒤로 순간이동 → 몬스터를 보고 backstab 가로 베기 (ENGINE_REQUEST 4장)
public class MainCharacter_Skill : FSM
{
    const string VanishAnimName = "vanish";
    const string BackstabAnimName = "backstab";
    const string SlashKey = "xslash";
    const float HideTime = 0.2f;
    const float BehindRangeRate = 0.8f;     // 몬스터 중심에서 평타 사거리의 이 비율만큼 뒤에 나타남 (사거리 안이라 항상 맞음)

    MainCharacter MainCharacter = null;
    float hideTimer = 0f;
    bool isHiding = false;
    public MainCharacter_Skill(MainCharacter _MainCharacter)
    {
        MainCharacter = _MainCharacter;
        MainCharacter.SkeletonAnim.AnimationState.Event += this.HandleAnimEvent;
        MainCharacter.SkeletonAnim.AnimationState.Complete += this.HandleAnimComplete;
    }

    public override void Enter()
    {
        isHiding = false;
        MainCharacter.SkeletonAnim.AnimationState.SetAnimation(0, VanishAnimName, false);
        MainCharacter.SkeletonAnim.timeScale = 1.0f;
    }

    public override void Exit()
    {
        MainCharacter.HpBar.gameObject.SetActive(true);
        MainCharacter.ShadowRenderer.enabled = true;
    }

    public override void Update()
    {
        if (!isHiding)
            return;

        hideTimer -= Time.fixedDeltaTime;
        if (hideTimer <= 0f)
            Appear();
    }

    // 목표 몬스터를 지나 등 뒤로 순간이동, 몬스터를 보고 backstab (나타나는 효과는 backstab 그림에 포함)
    void Appear()
    {
        isHiding = false;
        MainCharacter.Target = MainCharacter.FindTarget();
        if (MainCharacter.Target == null)
        {
            MainCharacter.ChangeState(State.Idle);
            return;
        }

        Vector3 dir = (MainCharacter.Target.transform.position - MainCharacter.transform.position).normalized;
        MainCharacter.transform.position = MainCharacter.Target.transform.position + dir * (float)MainCharacter.Status.Attack_Range * BehindRangeRate;
        // 몬스터를 바라봄: 몬스터가 왼쪽이면 반전
        MainCharacter.SetFlip(MainCharacter.Target.transform.position.x < MainCharacter.transform.position.x ? -1 : 1);
        MainCharacter.UpdateSkeletonAnimSortingOrder();
        MainCharacter.HpBar.gameObject.SetActive(true);
        MainCharacter.ShadowRenderer.enabled = true;
        MainCharacter.SkeletonAnim.AnimationState.SetAnimation(0, BackstabAnimName, false);
        MainCharacter.SkeletonAnim.timeScale = (float)MainCharacter.Status.TotalAttackSpeed;    // 평타와 같은 공격 속도
    }

    private void HandleAnimEvent(TrackEntry trackEntry, Spine.Event e)
    {
        if (MainCharacter.State != State.Skill)
            return;

        if (e.Data.Name.Equals("slash"))
        {
            // 캐릭터와 같은 방향으로 앞쪽을 베는 X
            MainCharacter.PlayEffect(SlashKey, MainCharacter.transform.position, MainCharacter.SkeletonAnim.Skeleton.ScaleX, MainCharacter.SkeletonRenderer.sortingOrder + 1);
        }
        else if (e.Data.Name.Equals("damage"))
        {
            // 휘두르는 순간 (attack3 타격감 수치)
            SkillData skill = MainCharacter.CurSkillSlot.SkillData;
            MainCharacter.HitTargets(MainCharacter.Status.Attack * skill.Damage, skill.Max_Hits, 0.12f, 6f, 3f, true);
        }
    }

    private void HandleAnimComplete(TrackEntry trackEntry)
    {
        if (MainCharacter.State != State.Skill)
            return;

        if (trackEntry.Animation.Name.Equals(VanishAnimName))
        {
            // 마지막 프레임이 빈 그림이라 캐릭터가 안 보이는 동안 체력바·그림자도 숨김
            MainCharacter.HpBar.gameObject.SetActive(false);
            MainCharacter.ShadowRenderer.enabled = false;
            hideTimer = HideTime;
            isHiding = true;
        }
        else if (trackEntry.Animation.Name.Equals(BackstabAnimName))
        {
            MainCharacter.ChangeState(State.Idle);
        }
    }
}
public class MainCharacter_Dead : FSM
{
    const string AnimName = "dead";

    MainCharacter MainCharacter = null;
    public MainCharacter_Dead(MainCharacter _MainCharacter)
    {
        MainCharacter = _MainCharacter;
        MainCharacter.SkeletonAnim.AnimationState.Complete += this.HandleAnimComplete;
    }

    public override void Enter()
    {
        MainCharacter.SkeletonAnim.AnimationState.SetAnimation(0, AnimName, false);
        MainCharacter.SkeletonAnim.timeScale = 1.0f;
    }

    private void HandleAnimComplete(TrackEntry trackEntry)
    {
        if (MainCharacter.State != State.Dead || trackEntry.Animation.Name.Equals(AnimName) == false)
            return;

        MainCharacter.OnDeadComplete(MainCharacter);
    }
}
public class MainCharacter_Hit : FSM
{
    const string AnimName = "hurt";

    MainCharacter MainCharacter = null;
    public MainCharacter_Hit(MainCharacter _MainCharacter)
    {
        MainCharacter = _MainCharacter;
        MainCharacter.SkeletonAnim.AnimationState.Complete += this.HandleAnimComplete;
    }
    public override void Enter()
    {
        MainCharacter.SkeletonAnim.AnimationState.SetAnimation(0, AnimName, false);
        MainCharacter.SkeletonAnim.timeScale = 1.0f;
    }

    private void HandleAnimComplete(TrackEntry trackEntry)
    {
        if (MainCharacter.State != State.Hit || trackEntry.Animation.Name.Equals(AnimName) == false)
            return;

        MainCharacter.ChangeState(State.Idle);
    }
}
public class MainCharacter_Roll : FSM
{
    const string AnimName = "dash";
    // dash 0.06초 x 9프레임 중 frame_002~006 동안만 이동
    const float MoveStartTime = 0.12f;
    const float MoveEndTime = 0.42f;
    const float AfterimageInterval = 0.05f;
    const string AfterimageKey = "Afterimage";

    MainCharacter MainCharacter = null;
    Vector3 startPosition = Vector3.zero;
    float distance = 0f;
    float afterimageTimer = 0f;
    public MainCharacter_Roll(MainCharacter _MainCharacter)
    {
        MainCharacter = _MainCharacter;
        MainCharacter.SkeletonAnim.AnimationState.Complete += this.HandleAnimComplete;
    }
    public override void Enter()
    {
        MainCharacter.SkeletonAnim.AnimationState.SetAnimation(0, AnimName, false);
        MainCharacter.SkeletonAnim.timeScale = 1.0f;
        MainCharacter.SetFlip(MainCharacter.RollDirection.x < 0 ? -1 : 1);
        startPosition = MainCharacter.transform.position;
        distance = GetRollDistance();
        afterimageTimer = 0f;
    }

    // 구르는 방향에 몬스터가 있으면 공격 범위 앞에서 멈추도록 거리 제한
    float GetRollDistance()
    {
        float distance = DataManager.Instance.SystemData.GetFloat("Roll_Distance");
        float range = (float)MainCharacter.Status.Attack_Range;
        foreach (Monster monster in InGameManager.Instance.Monsters)
        {
            Vector3 toMonster = monster.transform.position - MainCharacter.transform.position;
            float forward = Vector3.Dot(toMonster, MainCharacter.RollDirection);
            float side = Vector3.Cross(MainCharacter.RollDirection, toMonster).magnitude;
            if (forward > 0f && side < range)
                distance = Mathf.Min(distance, Mathf.Max(0f, forward - range));
        }
        return distance;
    }

    public override void Update()
    {
        float time = MainCharacter.SkeletonAnim.AnimationState.GetCurrent(0).TrackTime;
        if (time < MoveStartTime)
            return;

        // 처음에 빠르고 끝에서 느려지는 ease-out
        float t = Mathf.Clamp01((time - MoveStartTime) / (MoveEndTime - MoveStartTime));
        float eased = 1f - (1f - t) * (1f - t);
        MainCharacter.transform.position = startPosition + MainCharacter.RollDirection * distance * eased;
        MainCharacter.UpdateSkeletonAnimSortingOrder();

        // 잔상은 이동하는 동안만
        if (t >= 1f)
            return;

        afterimageTimer -= Time.fixedDeltaTime;
        if (afterimageTimer <= 0f)
        {
            afterimageTimer = AfterimageInterval;
            Afterimage afterimage = ObjectPoolManager.Instance.GetObject<Afterimage>("Effect", AfterimageKey);
            afterimage.key = AfterimageKey;
            afterimage.Play(MainCharacter);
        }
    }

    private void HandleAnimComplete(TrackEntry trackEntry)
    {
        if (MainCharacter.State != State.Dash || trackEntry.Animation.Name.Equals(AnimName) == false)
            return;

        MainCharacter.ChangeState(MainCharacter.Controller.IsDragController ? State.Move : State.Idle);
    }
}
public class MainCharacter_Move : FSM
{
    const string MoveAnimName = "run";

    MainCharacter MainCharacter = null;
    public MainCharacter_Move(MainCharacter _MainCharacter)
    {
        MainCharacter = _MainCharacter;
    }
    public override void Enter()
    {
        Move();
    }

    public void Move()
    {
        if (MainCharacter.State == State.Dead)
        {
            MainCharacter.ChangeState(State.Dead);
            return;
        }

        if (MainCharacter.Controller != null && MainCharacter.Controller.IsDragController)
            Move(MainCharacter.Controller.InputDirection);
    }

    // 조이스틱 조작이 없으면 화면 안의 몬스터를 향해 자동 이동
    public override void Update()
    {
        if (MainCharacter.Controller.IsDragController)
            return;

        MainCharacter.Target = MainCharacter.FindTarget();
        if (MainCharacter.Target == null)
        {
            MainCharacter.ChangeState(State.Idle);
            return;
        }

        if (MainCharacter.TryUseSkill())
            return;

        if (MainCharacter.IsTargetInAttackRange())
        {
            MainCharacter.ChangeState(State.Attack);
            return;
        }

        Vector3 toTarget = MainCharacter.Target.transform.position - MainCharacter.transform.position;

        // 대시는 자동 전투에서, 평타 사거리 + 대시 거리 절반보다 멀 때 (대시는 사거리 앞에서 멈춤)
        float dashDistance = (float)MainCharacter.Status.Attack_Range + DataManager.Instance.SystemData.GetFloat("Roll_Distance") * 0.5f;
        if (toTarget.magnitude > dashDistance && MainCharacter.TryRoll(toTarget))
            return;

        Move(toTarget.normalized);
    }

    private void Move(Vector3 vDir)
    {
        float moveSpeed = (float)MainCharacter.Status.TotalMoveSpeed;

        if (!MainCharacter.SkeletonAnim.AnimationName.Equals(MoveAnimName))
        {
            float defaultMoveSpeed = 0.7f;
            float rate = moveSpeed * defaultMoveSpeed;
            float timescale = rate;
            if (timescale < 1) timescale = 1.0f;

            MainCharacter.SkeletonAnim.AnimationState.SetAnimation(0, MoveAnimName, true);
            MainCharacter.SkeletonAnim.timeScale = timescale;
        }

        MainCharacter.transform.position += (vDir * moveSpeed * Time.deltaTime);

        // 원본 아트가 오른쪽을 보고 있으므로 왼쪽 이동 시 반전
        MainCharacter.SetFlip(vDir.x < 0 ? -1 : 1);
        MainCharacter.UpdateSkeletonAnimSortingOrder();
    }
}
