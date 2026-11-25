using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITopNoticeMessagePanel : UIPanel
{
    [SerializeField] UI_Tweener startTweener = null;
    [SerializeField] RectTransform contentPanel = null;

    Vector2 startPos;
    Vector2 endPos;

    float time = 0;
    public override void Init()
    {
        if (bInitialized)
            return;
        startPos = new Vector2(0, 60);
        endPos = new Vector2(0, -20);

        bInitialized = true;
    }
    public void ShowMessage()
    {
        time = 0f;
        SetActiveMessagePanel(true);
        startTweener.From = startPos;
        startTweener.To = endPos;
        startTweener.StartTween(()=>
        {
            StartCo();
        });
    }
    void CloseMessage()
    {
        startTweener.From = endPos;
        startTweener.To = startPos;
        startTweener.StartTween(() =>
        {
            SetActiveMessagePanel(false);
            ClosePanel();
        });
    }
    public void StartCo()
    {
        StopAllCoroutines();
        StartCoroutine(CoCheckTimer());
    }
    void SetActiveMessagePanel(bool _bActive)
    {
        if (contentPanel != null && contentPanel.gameObject.activeSelf != _bActive)
        {
            contentPanel.gameObject.SetActive(_bActive);
        }
    }
    IEnumerator CoCheckTimer()
    {
        while (true)
        {
            time += Time.deltaTime;
            if (time >= 2f)
            {
                CloseMessage();
                yield break;
            }
            yield return null;
        }
    }
}
