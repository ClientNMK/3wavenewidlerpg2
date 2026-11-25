using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public enum PopupType
{
    Non_Modal = 1,
    Modal,
}

public class UIPopup : UIBase
{
    public string PopupName { get; set; } = "";

    [SerializeField] protected PopupType PopupType = PopupType.Non_Modal;
    [Header("===== TouchPanel =====")]
    [SerializeField] protected Button touchPanel = null;

    [Header("===== Anim =====")]
    [SerializeField] protected AnimationCurve animCurve = null;
    [SerializeField] protected RectTransform animTarget = null;

    protected bool isSort = true;
    protected bool isNotClosing = false;

    public Action OnAnimComplete = null;
    public Action OnCloseOnce = null;

    int animScrollVersion = 0;
    virtual public void Init()
    {
        if (bInitialized)
            return;

        bInitialized = true;

        if (OnInitialized != null)
            OnInitialized();
        OnInitialized = null;
    }
    virtual public void Setup()
    {
        if (!bInitialized)
            Init();
    }

    virtual public void ShowPopup()
    {
        isSort = true;
        PopupManager.Instance.SetCanvas(this);
        this.gameObject.SetActive(true);
        if (animTarget != null)
        {
            StopAllCoroutines();
            StartCoroutine(CoShowPopup());
        }
        Setup();
    }
    virtual public void ClosePopup()
    {
        this.gameObject.SetActive(false);
        OnCloseOnce?.Invoke();
        OnCloseOnce = null;
    }

    virtual public void OnClickTouchPanel()
    {
        OnClickBackButton();
    }
    virtual public void OnClickBackButton()
    {
        OnClickCloseButton();
    }
    virtual public void OnClickCloseButton()
    {
        PopupManager.Instance.ClosePopup(this);
    }

    virtual public IEnumerator CoShowPopup()
    {
        // 스케일 0 구간에서는 ScrollRect의 범위 계산이 틀어져 콘텐츠가 밀리거나 코드로 지정한 위치가 초기화된다.
        // 연출 동안만 ScrollRect를 끄고, 끝나면 다시 켠다.
        foreach (var scrollRect in animTarget.GetComponentsInChildren<ScrollRect>(true))
            scrollRect.enabled = false;

        float fTime = 0f;
        float fDuration = 0.5f;
        // 팝업의 StopAllCoroutines나 연출 중 닫기에 영향받지 않도록 PopupManager에서 복구.
        PopupManager.Instance.StartCoroutine(CoRestoreAnimScrollRects(fDuration, ++animScrollVersion));
        float fSpeed = 1.0f / fDuration;
        while (true)
        {
            animTarget.localScale = Vector3.one * animCurve.Evaluate(fTime * fSpeed);

            fTime += Time.unscaledDeltaTime;
            if (fTime >= fDuration)
                break;
            yield return null;
        }
        OnAnimComplete?.Invoke();
        animTarget.localScale = Vector3.one;
        yield return null;
    }

    IEnumerator CoRestoreAnimScrollRects(float _delay, int _version)
    {
        yield return new WaitForSecondsRealtime(_delay);
        // 팝업 파괴 또는 다시 열려 새 연출이 진행 중이면 복구하지 않는다.
        if (this == null || _version != animScrollVersion)
            yield break;

        // 연출 중 Setup에서 복제된 항목(꺼진 상태로 복제됨)도 포함하도록 다시 찾는다.
        foreach (var scrollRect in animTarget.GetComponentsInChildren<ScrollRect>(true))
        {
            scrollRect.enabled = true;
            scrollRect.StopMovement();
        }
    }

    public bool IsSort { get { return isSort; } }
    public bool IsNotClosing { get { return isNotClosing; } }
    
}
