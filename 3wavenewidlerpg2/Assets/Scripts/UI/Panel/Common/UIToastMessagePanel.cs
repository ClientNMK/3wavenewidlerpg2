using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIToastMessagePanel : UIPanel
{
    [SerializeField] RectTransform messagePanel = null;
    [SerializeField] TMP_Text messageText = null;

    [SerializeField] UI_Tweener tweener = null;
    [SerializeField] CanvasGroup canvasGroup = null;
    [SerializeField] AnimationCurve animCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    float time = 0;
    bool bShowMessage = false;

    public override void Init()
    {
        if (bInitialized)
            return;

        bShowMessage = false;
        SetActiveMessagePanel(false);

        bInitialized = true;
    }

    public void ShowMessage(string _strText)
    {
        SetActiveMessagePanel(true);
        messageText.text = _strText;
        canvasGroup.alpha = 1;
        time = 0;
        tweener.StartTween(() =>
        {
            SetActiveMessagePanel(false);
            bShowMessage = false;

            ClosePanel();
        });
        StopAllCoroutines();
        StartCoroutine(Cor_MoveProcess());
    }

    void SetActiveMessagePanel(bool _bActive)
    {
        if (messagePanel != null && messagePanel.gameObject.activeSelf != _bActive)
        {
            messagePanel.gameObject.SetActive(_bActive);
        }
    }
    private IEnumerator Cor_MoveProcess()
    {
        while (true)
        {
            time += Time.deltaTime / 1;
            float result = Mathf.Lerp(1, 0, animCurve.Evaluate(time));
            canvasGroup.alpha = result;
            if (time >= 1f)
            {
                yield break;
            }
            yield return null;
        }
    }
    //IEnumerator CoCheckTimer()
    //{
    //    bShowMessage = true;
    //    Vector3 scale = new Vector3(1, 0, 1);
    //    messagePanel.transform.localScale = scale;
    //    while (true)
    //    {
    //        scale.y = animCurve.Evaluate(fCurShowTime / fShowTimeLength);
    //        messagePanel.transform.localScale = scale;

    //        fCurShowTime += Time.deltaTime;
    //        if (fCurShowTime >= fShowTimeLength)
    //            break;
    //        yield return null;
    //    }

    //    messagePanel.transform.localScale = scale = Vector3.one;
    //    SetActiveMessagePanel(false);
    //    bShowMessage = false;
    //    fCurShowTime = 0;

    //    ClosePanel();
    //}
}
