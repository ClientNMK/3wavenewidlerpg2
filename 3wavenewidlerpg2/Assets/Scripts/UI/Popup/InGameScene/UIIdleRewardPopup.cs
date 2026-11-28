using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 방치(오프라인) 보상 (kit_cocoa LAYOUT_idle.md). 받으면 골드·경험치가 위로 날아가 도착할 때 지급
public class UIIdleRewardPopup : UIPopup
{
    // 상자 움직임 (LAYOUT_idle.md '상자 움직임', 좌표 ×2)
    const float BobHeight = 8f;
    const float BobPeriod = 2f;             // 떠오름·빛 숨쉬기 같이
    const float WiggleCycle = 3f;
    const float WiggleTime = 0.5f;          // 3초마다 0.5초 들썩
    static readonly float[] WiggleAngles = { 0f, 6f, -6f, 4f, -3f, 0f };    // CSS -6° → +6° … (Unity는 반시계가 +)
    const float SparklePeriod = 1.6f;       // 두 개는 반 주기 어긋남

    [SerializeField] CanvasGroup content = null;
    [SerializeField] RectTransform chest = null;        // 피벗 (0.5, 0.1) = 들썩 회전 기준
    [SerializeField] Image chestGlow = null;
    [SerializeField] Image[] sparkles = null;
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
    Vector2 chestPosition = Vector2.zero;

    public override void Init()
    {
        if (bInitialized)
            return;

        PopupType = PopupType.Modal;
        chestPosition = chest.anchoredPosition;
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

    private void Update()
    {
        float time = Time.unscaledTime;
        float wave = 0.5f - 0.5f * Mathf.Cos(time / BobPeriod * Mathf.PI * 2f);
        chest.anchoredPosition = chestPosition + Vector2.up * wave * BobHeight;
        float wiggle = Mathf.Max(0f, time % WiggleCycle - (WiggleCycle - WiggleTime)) / WiggleTime * (WiggleAngles.Length - 1);
        int key = Mathf.Min((int)wiggle, WiggleAngles.Length - 2);
        chest.localEulerAngles = new Vector3(0f, 0f, Mathf.Lerp(WiggleAngles[key], WiggleAngles[key + 1], wiggle - key));
        chestGlow.rectTransform.localScale = Vector3.one * Mathf.Lerp(0.92f, 1.05f, wave);
        chestGlow.color = new Color(1f, 1f, 1f, Mathf.Lerp(0.8f, 1f, wave));
        for (int i = 0; i < sparkles.Length; i++)
        {
            float twinkle = Mathf.Sin((time + i * SparklePeriod * 0.5f) % SparklePeriod / SparklePeriod * Mathf.PI);
            sparkles[i].rectTransform.localScale = Vector3.one * twinkle;
            sparkles[i].color = new Color(1f, 1f, 1f, twinkle);
        }
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
