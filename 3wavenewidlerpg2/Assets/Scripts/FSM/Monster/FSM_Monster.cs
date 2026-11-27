using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
            Monster.ChangeState(Monster.MonsterData.Monster_Type == MonsterType.NormalMonster ? State.Idle : State.Attack);
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
    }

    private void HandleAnimComplete(TrackEntry trackEntry)
    {
        if (Monster.State != State.Attack || trackEntry.Animation.Name.Equals(AnimName) == false)
            return;

        if (Monster.IsTargetInRange(Monster.MonsterData.Monster_Attack_Range))
            Attack();
        else
            Monster.ChangeState(State.Idle);
    }
}
