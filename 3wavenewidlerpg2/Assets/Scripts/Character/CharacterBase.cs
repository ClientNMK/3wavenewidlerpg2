using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Spine.Unity;

public class DotDamageData
{
    public CharacterBase eventInstigator;
    public BigDouble damageAmount;
    public AttackType attackType;
    public float duraiton;
    public float timer;
    public DotDamageData() { }
    public DotDamageData(CharacterBase _eventInstigator, BigDouble _damageAmount, AttackType _attackType, float _duraiton)
    {
        eventInstigator = _eventInstigator;
        damageAmount = _damageAmount;
        attackType = _attackType;
        timer = duraiton = _duraiton;
    }

    public DotDamageData(CharacterBase _eventInstigator, double _damageAmount, AttackType _attackType, float _duraiton)
        : this(_eventInstigator, new BigDouble(_damageAmount), _attackType, _duraiton)
    {
    }
}

public enum CharacterType
{
    MainCharacter,
    Monster,
    OtherCharacter,
}

public enum AttackType
{
    Normal,
    Heavy,
    Skill,
    Knockback_Skill,
    DefenceIgnore,
    DoubleAttack,
}

public class CharacterBase : MonoBehaviour
{
    public Transform CharacterTr = null;
    public Transform Bone_HpTr = null;
    public Transform Bone_HeadTr = null;
    public Transform Bone_BodyTr = null;
    public Transform SkillTr = null;
    public Transform Bone_ProjectileTr = null;
    public SkeletonAnimation SkeletonAnim = null;
    public Collider2D Collider = null;
    public MeshRenderer SkeletonRenderer;
    public SpriteRenderer ShadowRenderer;
    public HpBar HpBar = null;

    #region FSM
    public FSM[] FSMMain = new FSM[(int)State.Dead + 1];
    protected FSM curFSM = null;
    public State State = State.Idle;
    #endregion

    #region Material
    const string colorPropertyName = "_Color";
    const string blackPropertyName = "_Black";
    protected Material material;
    protected MaterialPropertyBlock materialPB;
    protected static readonly int materialPID = Shader.PropertyToID(colorPropertyName);
    protected static readonly int materialTintPID = Shader.PropertyToID(blackPropertyName);
    #endregion

    #region Callback
    public Action<CharacterBase> OnDeadEnter = null;
    public Action<CharacterBase> OnDeadComplete = null;
    public Action<CharacterBase, BigDouble> OnAddHp = null;
    public Action<CharacterBase, BigDouble> OnTakeDamage = null;
    #endregion

    public Status Status = new Status();
    public CharacterBase Target = null;
    public CharacterType CharacterType = CharacterType.MainCharacter;
    public BigDouble CurHp;

    public float tickTimer = 0f;
    public float tickTimeLength = 1.0f;
    public AttackType LastAttackType = AttackType.Normal;
    public List<DotDamageData> DotDamageList = new();

    protected bool bInitialized = false;
    virtual public void Init()
    {
        if (bInitialized)
            return;

        bInitialized = true;
    }
    virtual public void Setup()
    {
        if (!bInitialized)
            Init();

        SetupSpine();
        SetupStatus(true);
        RefreshHpBar();

        ChangeState(State.Idle);
    }

    virtual public void SetupSpine()
    {
        SkeletonAnim.skeleton.SetToSetupPose();
        SkeletonAnim.skeleton.SetBonesToSetupPose();
    }
    virtual public void SetupStatus(bool bRefill = false)
    {

    }
    virtual public void SetupStatus(Status _status)
    {
        Status = _status;
    }

    virtual public  void StopCurState()
    {
        if (curFSM != null)
            curFSM.Exit();
    }
    virtual public  void ChangeState(State _State)
    {
        this.State = _State;
        if (curFSM != null)
            curFSM.Exit();
        curFSM = FSMMain[(int)State];
        if (curFSM != null)
            curFSM.Enter();
    }

    public virtual void SetFlip(int value)
    {
        if (SkeletonAnim != null)
            SkeletonAnim.Skeleton.ScaleX = value;

        // 그림자 좌우 오프셋도 같이 반전 (메인 캐릭터는 발이 기준점보다 오른쪽)
        Vector3 shadowPosition = ShadowRenderer.transform.localPosition;
        shadowPosition.x = Mathf.Abs(shadowPosition.x) * value;
        ShadowRenderer.transform.localPosition = shadowPosition;
    }
    public virtual void SetScale(float scale)
    {
        if (CharacterTr != null)
            CharacterTr.localScale = Vector3.one * scale;
    }

    public virtual Color SetMaterialColor(Color color)
    {
        if (materialPB == null)
            materialPB = new MaterialPropertyBlock();

        materialPB.SetColor(materialPID, color);
        SkeletonRenderer.SetPropertyBlock(materialPB);
        return color;
    }
    public virtual Color SetMaterialTintColor(Color color)
    {
        if (materialPB == null)
            materialPB = new MaterialPropertyBlock();

        materialPB.SetColor(materialTintPID, color);
        SkeletonRenderer.SetPropertyBlock(materialPB);
        return color;
    }

    virtual public void SetActiveMeshRenderer(bool bActive)
    {
        if (this.SkeletonRenderer != null)
            this.SkeletonRenderer.enabled = bActive;
        if (this.ShadowRenderer != null)
            this.ShadowRenderer.enabled = bActive;
    }

    public virtual void UpdateSkeletonAnimSortingOrder()
    {
        float y = transform.position.y;
        int sortingOrder = Mathf.RoundToInt(-y * 100);
        if (SkeletonRenderer != null)
            SkeletonRenderer.sortingOrder = sortingOrder;
    }
    virtual public void SetPosition(Vector3 _position)
    {
        this.transform.position = _position;
        this.UpdateSkeletonAnimSortingOrder();
    }
    virtual public void AddHp(double _addValue)
    {
        AddHp(new BigDouble(_addValue));
    }
    virtual public void AddHp(BigDouble _addValue)
    {
        if (State == State.Dead)
            return;

        CurHp += _addValue;

        BigDouble totalHp = GetTotalHp();

        if (_addValue > BigDouble.Zero)
        {
            if (CurHp >= totalHp)
                CurHp = totalHp;
        }
        else
        {
            if (CurHp >= totalHp)
            {
                CurHp = totalHp;
                CurHp += _addValue;
            }
        }

        if (CurHp < BigDouble.Zero)
            CurHp = BigDouble.Zero;

        RefreshHpBar();

        if (OnAddHp != null)
            OnAddHp(this, _addValue);
    }

    void RefreshHpBar()
    {
        HpBar.SetRatio((float)(CurHp.ToDouble() / GetTotalHp().ToDouble()));
    }

    virtual public void TakeDotDamage(double damageAmount, CharacterBase eventInstigator, AttackType attackType, float duration)
    {
        TakeDotDamage(new BigDouble(damageAmount), eventInstigator, attackType, duration);
    }

    virtual public void TakeDotDamage(BigDouble damageAmount, CharacterBase eventInstigator, AttackType attackType, float duration)
    {
        DotDamageList.Add(new DotDamageData(eventInstigator, damageAmount, attackType, duration));
    }

    virtual public void TakeDamage(double damageAmount, CharacterBase eventInstigator, AttackType attackType)
    {
        TakeDamage(new BigDouble(damageAmount), eventInstigator, attackType);
    }
    virtual public void TakeDamage(BigDouble damageAmount, CharacterBase eventInstigator, AttackType attackType)
    {
        if (CurHp > BigDouble.Zero)
        {
            if (State == State.Dead || eventInstigator.State == State.Dead || State == State.Pause || State == State.Victory)
                return;

            AddHp(-damageAmount);

            if (CurHp <= BigDouble.Zero)
            {
                LastAttackType = attackType;
                CurHp = BigDouble.Zero;
                ChangeState(State.Dead);
            }
        }

        if (OnTakeDamage != null)
            OnTakeDamage(this, damageAmount);
    }

    // 1초마다 최대 체력의 Recovery_Hp_Per 만큼 회복
    void HpRecovery()
    {
        if (Status.Recovery_Hp_Per <= 0 || CurHp >= GetTotalHp())
            return;

        AddHp(GetTotalHp() * Status.Recovery_Hp_Per);
    }

    void DotDamage(float _deltaTime)
    {
        if (Status == null || State == State.Dead || State == State.Pause)
            return;

        if (DotDamageList.Count <= 0)
            return;

        for (int i = 0; i < DotDamageList.Count; i++)
        {
            var dotDamage = DotDamageList[i];
            if (dotDamage.eventInstigator == null || dotDamage.eventInstigator.State == State.Dead || dotDamage.eventInstigator.State == State.Pause)
            {
                DotDamageList.Remove(dotDamage);
                i--;
                continue;
            }
            TakeDamage(dotDamage.damageAmount, dotDamage.eventInstigator, dotDamage.attackType);

            dotDamage.timer -= _deltaTime;
            if (dotDamage.timer <= 0)
            {
                DotDamageList.Remove(dotDamage);
                i--;
                continue;
            }
        }
    }
    public virtual BigDouble GetTotalHp()
    {
        return Status.Hp;
    }
    virtual public void Release()
    {
        if (SkeletonRenderer != null)
            SkeletonRenderer.sortingOrder = 0;

        Target = null;

        OnDeadEnter = null;
        OnDeadComplete = null;
        OnAddHp = null;
        OnTakeDamage = null;

        DotDamageList.Clear();
    }

    virtual protected void FixedUpdate()
    {
        if (curFSM != null)
            curFSM.Update();

        Tick(Time.fixedDeltaTime);
    }

    virtual protected void Tick(float _deltaTime)
    {
        tickTimer += _deltaTime;
        if (tickTimer >= tickTimeLength)
        {
            tickTimer -= tickTimeLength;
            HpRecovery();
            DotDamage(_deltaTime);
        }
    }
}
