using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Spine.Unity;

public class UISystemLoadingPanel : UIPanel
{
    readonly string[] tipKeys = {
        "Msg_Tip_1", "Msg_Tip_2", "Msg_Tip_3",
        "Msg_Tip_4", "Msg_Tip_5", "Msg_Tip_6"
    };

    [SerializeField] Image bgImage = null;
    [SerializeField] SkeletonGraphic spine = null;
    [SerializeField] Image[] lineImages = null;
    [SerializeField] TMP_Text[] tipTexts = null;
    //[SerializeField] RawImage infiniteImage = null;

    Color bgColor = Color.black;
    Color spineColor = Color.white;
    Color lineImageColor = new Color(220, 220, 220, 255);
    Color textColor = new Color(220, 220, 220, 255);
    //Color infiniteImageColor = new Color(10f/255f, 10f/255f, 10f/255f, 255f/255f);
    public void CloseLoadingPanel(System.Action _start ,float _delayTime, System.Action _complete = null)
    {
        if (this.gameObject.activeSelf == false)
            ShowPanel();

        SetResourcesAlpha(1.0f);
        if (_start != null)
            _start();

        if (_delayTime > 0)
        {
            StopAllCoroutines();
            StartCoroutine(CoClosePanel(_delayTime, _complete));
        }
        else
        {
            ClosePanel();
            if (_complete != null)
                _complete();
        }
    }

    public void SetResourcesAlpha(float alpha)
    {
        Color color = bgColor;
        color.a = alpha;
        bgImage.color = color;

        color = spineColor;
        color.a = alpha;
        spine.color = color;

        color = lineImageColor;
        color.a = alpha;
        for (int i = 0; i < lineImages.Length; i++)
        {
            lineImages[i].color = color;
        }

        color = textColor;
        color.a = alpha;
        for (int i = 0; i < tipTexts.Length; i++)
        {
            tipTexts[i].color = color;
        }

        //color = infiniteImageColor;
        //color.a = alpha;
        //infiniteImage.color = color;
    }

    public override void ShowPanel()
    {
        int randIdx = GameUtils.GameUtils.GetRandomRange(0, tipKeys.Length);
        if (randIdx >= 0 && randIdx < tipKeys.Length && tipTexts != null && tipTexts.Length > 0)
        {
            tipTexts[0].text = StringManager.Instance.GetStringText(tipKeys[randIdx]);
        }

        base.ShowPanel();
    }
    IEnumerator CoClosePanel(float _delayTime, System.Action _complete = null)
    {
        SetResourcesAlpha(1.0f);
        float timer = 0f;
        while (true)
        {
            timer += Time.deltaTime;
            if (timer >= _delayTime)
                break;

            if (_delayTime - timer <= 0.5f)
            {
                float alpha = (_delayTime - timer) / 0.5f;
                if (alpha < 0.01f) alpha = 0.0f;
                SetResourcesAlpha(alpha);
            }

            if (_delayTime - timer <= 0.10f)
            {
                if (_complete != null)
                    _complete();
                _complete = null;
            }
            yield return null;
        }
        SetResourcesAlpha(0);
        if (_complete != null)
            _complete();

        ClosePanel();
        yield return null;
    }
}
