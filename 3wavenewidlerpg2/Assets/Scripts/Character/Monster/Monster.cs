using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : CharacterBase
{
    public MonsterData MonsterData = null;

    public override void Init()
    {
        if (bInitialized)
            return;

        CharacterType = CharacterType.Monster;

        FSMMain = new FSM[(int)State.Dead + 1];
        FSMMain[(int)State.Idle] = new Monster_Idle(this);
        FSMMain[(int)State.Move] = new Monster_Move(this);
        FSMMain[(int)State.Attack] = new Monster_Attack(this);
        FSMMain[(int)State.Dead] = new Monster_Dead(this);

        bInitialized = true;
    }

    public void Setup(MonsterData _monsterData, double _attack, double _hp, double _penetrate)
    {
        MonsterData = _monsterData;
        Target = InGameManager.Instance.MainCharacter;

        Status.Attack = new BigDouble(_attack);
        Status.Hp = new BigDouble(_hp);
        Status.Penetrate = _penetrate;
        Status.Move_Speed = MonsterData.Monster_Move_Speed;
        Status.Attack_Speed = MonsterData.Monster_Attack_Speed;

        // 피격 연출 도중 풀에 반환됐을 수 있어 초기화
        SetMaterialColor(Color.white);
        SkeletonAnim.Skeleton.X = 0f;

        SetScale(MonsterData.Monster_Scale);
        HpBar.gameObject.SetActive(true);
        Setup();
    }

    public override void SetupSpine()
    {
        SkeletonAnim.Skeleton.SetSkin(MonsterData.Skin_Name);
        base.SetupSpine();
    }

    public override void SetupStatus(bool bRefill = false)
    {
        if (bRefill)
            CurHp = GetTotalHp();
    }

    const float FlashExtraTime = 0.07f;         // 히트스톱 + 0.07초 동안 번쩍임
    const float KnockbackReturnTime = 0.25f;
    const float PixelSize = 1f / 64f;
    Coroutine hitCoroutine = null;

    public void PlayHitReaction(float _direction, float _hitStopTime, float _knockbackPixels)
    {
        if (hitCoroutine != null)
            StopCoroutine(hitCoroutine);
        hitCoroutine = StartCoroutine(CoHitReaction(_direction, _hitStopTime, _knockbackPixels * PixelSize));
    }

    // 검은 실루엣 번쩍임 + 뒤로 밀렸다가 복귀 (히트스톱 중에도 진행되도록 unscaled 시간 사용)
    IEnumerator CoHitReaction(float _direction, float _hitStopTime, float _knockback)
    {
        float totalTime = _hitStopTime + KnockbackReturnTime;
        float timer = 0f;
        while (timer < totalTime)
        {
            SetMaterialColor(timer < _hitStopTime + FlashExtraTime ? Color.black : Color.white);

            // 정지 중에는 ±1px 떨림
            float offset = timer < _hitStopTime
                ? _knockback + (Time.frameCount % 2 == 0 ? PixelSize : -PixelSize)
                : _knockback * (1f - (timer - _hitStopTime) / KnockbackReturnTime);
            SkeletonAnim.Skeleton.X = _direction * offset;

            timer += Time.unscaledDeltaTime;
            yield return null;
        }
        SetMaterialColor(Color.white);
        SkeletonAnim.Skeleton.X = 0f;
    }

    public bool IsTargetInRange(float _range)
    {
        return Target.State != State.Dead && Vector2.Distance(transform.position, Target.transform.position) <= _range;
    }
}
