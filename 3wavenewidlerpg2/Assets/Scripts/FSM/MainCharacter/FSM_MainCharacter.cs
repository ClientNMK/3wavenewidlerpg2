using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    // 스킬 2 (SkillChart Resource Skill_002): 같은 동작에 X베기 대신 지연 일섬
    const string DelayedCutResource = "Skill_002";
    const string DelayedCutKey = "delayed_cut";
    const float DelayedCutTime = 0.2f;          // 이펙트 frame_006에 터짐 = 타격
    const int FrontSortingOrder = 32100;        // 캐릭터·몬스터 앞
    // 스킬 3 (Skill_003): attack3 찍는 순간 칼바람 회오리 (ENGINE_REQUEST 4-3, 시간은 이펙트 시작 기준, 거리는 스프라이트 px)
    const string BladeGaleResource = "Skill_003";
    const string Attack3AnimName = "attack3";
    const string BladeGaleKey = "blade_gale";
    const float PixelSize = 1f / 64f;
    const float GaleStartOffset = 50f * PixelSize;     // 캐릭터 앞 바닥
    const float GaleChaseSpeed = 2.5f;                 // 0.1~1.0초 동안 목표 몬스터를 따라감 (칸/초)
    const float GaleMoveStart = 0.1f;
    const float GaleMoveEnd = 1.0f;
    const float GaleRange = 90f * PixelSize;           // 끌어오기·다단 히트 범위 (회오리 그림 크기)
    const float GaleBlastRange = 170f * PixelSize;     // 폭발 범위 (바닥 고리 반지름)
    const float GaleHoldRange = 20f * PixelSize;       // 중심 ±20px까지 끌어옴
    const float GalePullSpeed = 4f;                    // 끌어오는 속도 (칸/초)
    const float GalePullStart = 0.25f;
    const float GalePullEnd = 0.95f;
    const float GaleHitStart = 0.25f;
    const float GaleHitInterval = 0.08f;
    const int GaleHitCount = 10;
    const float GaleBlastTime = 1.2f;                  // frame_036 폭발 = 마무리 타격
    const float GaleBlastStopTime = 0.1f;              // 캐릭터·몬스터만 정지 (이펙트는 계속)
    // 차트 Duration에 맞춰 회오리가 도는 중간 프레임(8~29, 다단 히트 구간)을 반복해 늘림 (시트 54장 = 1.8초)
    const float GaleSheetTime = 1.8f;
    const int GaleLoopStart = 8;
    const int GaleLoopEnd = 29;
    const float GaleBlastPush = 40f * PixelSize;       // 바깥쪽으로 밀림
    const float HideTime = 0.2f;
    const float VanishSpeed = 3f;           // vanish 0.28초 → 약 0.09초
    const float BehindRangeRate = 0.8f;     // 몬스터 중심에서 평타 사거리의 이 비율만큼 뒤에 나타남 (사거리 안이라 항상 맞음)

    MainCharacter MainCharacter = null;
    float hideTimer = 0f;
    bool isHiding = false;
    int remainCount = 0;            // Skill_Count만큼 vanish → backstab 반복
    Monster lastTarget = null;
    readonly List<Monster> galeTargets = new();     // 지금 회오리들이 따라가는 몬스터 (회오리마다 다른 적)
    public MainCharacter_Skill(MainCharacter _MainCharacter)
    {
        MainCharacter = _MainCharacter;
        MainCharacter.SkeletonAnim.AnimationState.Event += this.HandleAnimEvent;
        MainCharacter.SkeletonAnim.AnimationState.Complete += this.HandleAnimComplete;
    }

    public override void Enter()
    {
        remainCount = MainCharacter.CurSkillSlot.SkillData.Skill_Count;
        lastTarget = null;
        if (IsBladeGale)
            Attack3();
        else
            Vanish();
    }

    bool IsBladeGale { get { return MainCharacter.CurSkillSlot.SkillData.Resource.Equals(BladeGaleResource); } }

    // 스킬 3은 전용 동작 없이 attack3 (평타와 같은 공격 속도)
    void Attack3()
    {
        MainCharacter.SkeletonAnim.AnimationState.SetAnimation(0, Attack3AnimName, false);
        MainCharacter.SkeletonAnim.timeScale = (float)MainCharacter.Status.TotalAttackSpeed;
        MainCharacter.SetFlip(MainCharacter.Target.transform.position.x < MainCharacter.transform.position.x ? -1 : 1);
    }

    void Vanish()
    {
        isHiding = false;
        MainCharacter.SkeletonAnim.AnimationState.SetAnimation(0, VanishAnimName, false);
        MainCharacter.SkeletonAnim.timeScale = VanishSpeed;
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
        // 반복이면 방금 벤 적 말고 가장 가까운 다른 적 뒤로 (다른 적이 없으면 같은 적)
        if (MainCharacter.Target == lastTarget)
            MainCharacter.Target = InGameManager.Instance.Monsters.Where(x => x != lastTarget)
                .OrderBy(x => Vector2.Distance(x.transform.position, MainCharacter.transform.position)).FirstOrDefault() ?? lastTarget;
        lastTarget = (Monster)MainCharacter.Target;

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

        if (IsBladeGale)
        {
            if (e.Data.Name.Equals("slash"))
                MainCharacter.StartCoroutine(CoBladeGale(MainCharacter.CurSkillSlot.SkillData));
            return;
        }
        bool isDelayedCut = MainCharacter.CurSkillSlot.SkillData.Resource.Equals(DelayedCutResource);
        if (e.Data.Name.Equals("slash") && !isDelayedCut)
        {
            // 캐릭터와 같은 방향으로 앞쪽을 베는 X
            MainCharacter.PlayEffect(SlashKey, MainCharacter.transform.position, MainCharacter.SkeletonAnim.Skeleton.ScaleX, MainCharacter.SkeletonRenderer.sortingOrder + 1);
        }
        else if (e.Data.Name.Equals("damage") && isDelayedCut)
            MainCharacter.StartCoroutine(CoDelayedCut(MainCharacter.CurSkillSlot.SkillData));
        else if (e.Data.Name.Equals("damage"))
        {
            // 휘두르는 순간 (attack3 타격감 수치)
            SkillData skill = MainCharacter.CurSkillSlot.SkillData;
            MainCharacter.HitTargets(MainCharacter.Status.Attack * skill.Damage, skill.Max_Hits, 0.12f, 6f, 3f, true);
        }
    }

    // 휘두르는 순간 선이 그어지고 몬스터는 멈춤 → 0.2초에 터지며 임팩트 프레임 → 흰 번쩍임 2프레임 뒤 피격
    IEnumerator CoDelayedCut(SkillData _skill)
    {
        Effect cut = MainCharacter.PlayEffect(DelayedCutKey, MainCharacter.transform.position, MainCharacter.SkeletonAnim.Skeleton.ScaleX, FrontSortingOrder);
        List<Monster> targets = MainCharacter.FindAttackTargets(_skill.Max_Hits);
        foreach (Monster target in targets)
            target.SkeletonAnim.timeScale = 0f;
        yield return new WaitForSeconds(DelayedCutTime);
        yield return InGameManager.Instance.CoImpactFrame(cut);

        targets.RemoveAll(x => x.State == State.Dead);
        foreach (Monster target in targets)
        {
            target.SetMaterialColor(Color.white);
            target.SetMaterialTintColor(Color.white);
        }
        yield return new WaitForSecondsRealtime(2f / 30f);
        foreach (Monster target in targets)
        {
            target.SetMaterialTintColor(new Color(0f, 0f, 0f, 0f));
            target.SkeletonAnim.timeScale = 1f;
        }
        MainCharacter.HitTargets(MainCharacter.Status.Attack * _skill.Damage, _skill.Max_Hits, 0f, 6f, 0f, true);
    }

    // 회오리가 앞으로 가며 범위 안 몬스터를 끌어오고 10번 베고, 1.2초에 터지며 밀어냄
    IEnumerator CoBladeGale(SkillData _skill)
    {
        float flip = MainCharacter.SkeletonAnim.Skeleton.ScaleX;
        Vector3 start = MainCharacter.transform.position + Vector3.right * flip * GaleStartOffset;
        Effect gale = MainCharacter.PlayEffect(BladeGaleKey, start, flip, FrontSortingOrder);
        float loopTime = (GaleLoopEnd - GaleLoopStart + 1) / 30f;
        int loopCount = Mathf.Max(0, Mathf.RoundToInt((_skill.Duration - GaleSheetTime) / loopTime));
        gale.SetLoop(GaleLoopStart, GaleLoopEnd, loopCount);
        float extraTime = loopCount * loopTime;     // 이동·끌어오기·다단 히트·폭발이 이만큼 늦어짐
        BigDouble damage = MainCharacter.Status.Attack * _skill.Damage;
        List<Monster> flashed = new();
        Monster chase = null;
        int hitCount = 0;
        for (float timer = 0f; timer < GaleBlastTime + extraTime; timer += Time.deltaTime)
        {
            // 따라가던 몬스터가 죽으면 다른 몬스터로
            if (chase == null || chase.State == State.Dead)
            {
                galeTargets.Remove(chase);
                chase = FindGaleTarget(gale.transform.position);
                if (chase != null)
                    galeTargets.Add(chase);
            }
            if (chase != null && timer >= GaleMoveStart && timer <= GaleMoveEnd + extraTime)
                gale.transform.position = Vector3.MoveTowards(gale.transform.position, chase.transform.position, GaleChaseSpeed * Time.deltaTime);
            Vector3 center = gale.transform.position;
            SetWhite(flashed, false);   // 흰 번쩍임은 1프레임
            flashed.Clear();
            if (timer >= GalePullStart && timer <= GalePullEnd + extraTime)
                foreach (Monster monster in GetGaleTargets(center, GaleRange, int.MaxValue))
                {
                    Vector3 toCenter = center - monster.transform.position;
                    float pull = Mathf.Min(GalePullSpeed * Time.deltaTime, toCenter.magnitude - GaleHoldRange);
                    if (pull > 0f)
                        monster.SetPosition(monster.transform.position + toCenter.normalized * pull);
                }
            for (; hitCount < GaleHitCount + Mathf.RoundToInt(extraTime / GaleHitInterval) && timer >= GaleHitStart + GaleHitInterval * hitCount; hitCount++)
                foreach (Monster monster in GetGaleTargets(center, GaleRange, _skill.Max_Hits))
                {
                    MainCharacter.DamageTarget(monster, MainCharacter.RollDamage(damage, out bool isCri), isCri);
                    flashed.Add(monster);
                }
            SetWhite(flashed, true);
            yield return null;
        }
        SetWhite(flashed, false);
        galeTargets.Remove(chase);

        // 폭발: 캐릭터·몬스터 0.1초 정지(이펙트는 정지 중에도 재생) + 흰 번쩍임 → 흔들림, 마무리 타격, 바깥으로 밀림
        Vector3 blastCenter = gale.transform.position;
        List<Monster> targets = GetGaleTargets(blastCenter, GaleBlastRange, _skill.Max_Hits);
        SetWhite(targets, true);
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(GaleBlastStopTime);
        Time.timeScale = 1f;
        SetWhite(targets, false);
        InGameManager.Instance.PlayImpactShake();
        foreach (Monster monster in targets)
        {
            MainCharacter.DamageTarget(monster, MainCharacter.RollDamage(damage, out bool isCri), isCri);
            if (monster.State != State.Dead)
                monster.SetPosition(monster.transform.position + (monster.transform.position - blastCenter).normalized * GaleBlastPush);
        }
    }

    // 다른 회오리가 따라가지 않는 가장 가까운 몬스터 (다 따라가고 있으면 가장 가까운 몬스터)
    Monster FindGaleTarget(Vector3 _position)
    {
        IEnumerable<Monster> byDistance = InGameManager.Instance.Monsters.OrderBy(x => Vector2.Distance(x.transform.position, _position));
        return byDistance.FirstOrDefault(x => !galeTargets.Contains(x)) ?? byDistance.FirstOrDefault();
    }

    // 회오리 중심에서 범위 안, 가까운 순으로 _count마리
    List<Monster> GetGaleTargets(Vector3 _center, float _range, int _count)
    {
        return InGameManager.Instance.Monsters.Where(x => Vector2.Distance(x.transform.position, _center) <= _range)
            .OrderBy(x => Vector2.Distance(x.transform.position, _center)).Take(_count).ToList();
    }

    // 흰 실루엣 (Tint Black의 어두운 색을 흰색으로)
    void SetWhite(List<Monster> _monsters, bool _isWhite)
    {
        foreach (Monster monster in _monsters)
            monster.SetMaterialTintColor(_isWhite ? Color.white : new Color(0f, 0f, 0f, 0f));
    }

    private void HandleAnimComplete(TrackEntry trackEntry)
    {
        if (MainCharacter.State != State.Skill)
            return;

        if (trackEntry.Animation.Name.Equals(Attack3AnimName))
        {
            if (--remainCount > 0)
                Attack3();
            else
                MainCharacter.ChangeState(State.Idle);
            return;
        }

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
            if (--remainCount > 0)
                Vanish();
            else
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
    const string RunAnimName = "run";
    const string SprintAnimName = "sprint";    // 이동 속도가 SystemChart Sprint_Move_Speed 이상이면
    // 기본 재생일 때 발이 안 미끄러지는 속도 (스프라이트 px/초, ENGINE_REQUEST '발 미끄러짐 맞추기')
    const float RunPixelsPerSecond = 125f;      // 한 바퀴 80px / 0.64초
    const float SprintPixelsPerSecond = 250f;   // 한 바퀴 120px / 0.48초
    const float SpritePixelsPerUnit = 64f;      // SkeletonData scale 1/64

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
        bool isSprint = moveSpeed >= DataManager.Instance.SystemData.GetFloat("Sprint_Move_Speed");
        string animName = isSprint ? SprintAnimName : RunAnimName;
        if (!MainCharacter.SkeletonAnim.AnimationName.Equals(animName))
            MainCharacter.SkeletonAnim.AnimationState.SetAnimation(0, animName, true);
        // 재생 속도 = 이동 속도 ÷ 기본 재생일 때 속도 (화면 배율 포함, 이동 속도가 바뀌면 같이 바뀜)
        float stridePixels = isSprint ? SprintPixelsPerSecond : RunPixelsPerSecond;
        MainCharacter.SkeletonAnim.timeScale = moveSpeed / (stridePixels / SpritePixelsPerUnit * MainCharacter.SkeletonAnim.transform.lossyScale.y);

        MainCharacter.transform.position += (vDir * moveSpeed * Time.deltaTime);

        // 원본 아트가 오른쪽을 보고 있으므로 왼쪽 이동 시 반전
        MainCharacter.SetFlip(vDir.x < 0 ? -1 : 1);
        MainCharacter.UpdateSkeletonAnimSortingOrder();
    }
}
