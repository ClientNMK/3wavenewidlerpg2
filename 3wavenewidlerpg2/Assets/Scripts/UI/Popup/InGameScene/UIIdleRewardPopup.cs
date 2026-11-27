using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 방치(오프라인) 보상 (kit_cocoa LAYOUT_idle.md). 받으면 골드·경험치가 위로 날아가 도착할 때 지급
public class UIIdleRewardPopup : UIPopup
{
    [SerializeField] CanvasGroup content = null;
    [SerializeField] TMP_Text timeText = null;
    [SerializeField] Image timeFill = null;
    [SerializeField] TMP_Text goldPerHourText = null;
    [SerializeField] TMP_Text goldGainText = null;
    [SerializeField] RectTransform goldIcon = null;
    [SerializeField] TMP_Text expPerHourText = null;
    [SerializeField] TMP_Text expGainText = null;
    [SerializeField] RectTransform expIcon = null;
    [SerializeField] Button receiveButton = null;
    [SerializeField] Button doubleButton = null;        // 광고 보고 2배 (광고 연결 전에는 바로 2배)
    [SerializeField] Button closeButton = null;
    [SerializeField] Image flyIconTemplate = null;

    double goldGain = 0;
    double expGain = 0;
    bool isReceived = false;

    public override void Init()
    {
        if (bInitialized)
            return;

        PopupType = PopupType.Modal;
        receiveButton.onClick.AddListener(() => Receive(1));
        doubleButton.onClick.AddListener(() => Receive(2));
        closeButton.onClick.AddListener(() => Receive(1));
        bInitialized = true;
    }

    public void Setup(int _minutes, int _maxMinutes, double _goldPerMinute, double _expPerMinute)
    {
        isReceived = false;
        content.alpha = 1f;
        goldGain = _goldPerMinute * _minutes;
        expGain = _expPerMinute * _minutes;
        timeText.text = string.Format(StringManager.Instance.GetStringText("Msg_IdleTimeValue"), _minutes / 60, _minutes % 60, _maxMinutes / 60);
        timeFill.fillAmount = (float)_minutes / _maxMinutes;
        goldPerHourText.text = string.Format("+{0:N0}", _goldPerMinute * 60);
        goldGainText.text = string.Format("+{0:N0}", goldGain);
        expPerHourText.text = string.Format("+{0:N0}", _expPerMinute * 60);
        expGainText.text = string.Format("+{0:N0}", expGain);
    }

    // 바깥 터치는 무시, 뒤로 가기·닫기는 그냥 받기
    public override void OnClickTouchPanel() { }
    public override void OnClickBackButton()
    {
        Receive(1);
    }

    void Receive(int _multiplier)
    {
        if (isReceived)
            return;
        isReceived = true;
        StartCoroutine(CoReceive(_multiplier));
    }

    IEnumerator CoReceive(int _multiplier)
    {
        content.alpha = 0f;
        UIGoodsFly.Fly(this, flyIconTemplate, goldIcon.position, InGameManager.GoldGoodsId, goldGain * _multiplier);
        UIGoodsFly.Fly(this, flyIconTemplate, expIcon.position, InGameManager.ExpGoodsId, expGain * _multiplier);
        yield return new WaitForSecondsRealtime(UIGoodsFly.TotalTime);
        PopupManager.Instance.ClosePopup(this);
    }
}
