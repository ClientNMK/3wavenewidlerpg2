using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 보스 클리어: VICTORY 문장 + 보상을 한 장면에서 보여 주고 자동으로 닫힘 (kit_cocoa LAYOUT_clear.md)
// 닫힐 때 보상은 재화 칸으로 날아가 도착할 때 지급
public class UIBossClear : MonoBehaviour
{
    const float FadeInTime = 0.1f;
    const float BandStart = 0.1f;
    const float BandOpenTime = 0.15f;
    const float EmblemStart = 0.2f;
    const float EmblemPopTime = 0.25f;      // 0 → 1.2 → 1.0
    const float SlotStart = 0.5f;
    const float SlotInterval = 0.12f;
    const float SlotPopTime = 0.2f;         // 0 → 1.18 → 1.0
    const float AutoCloseTime = 2f;         // 다 나온 뒤 바가 차면 닫힘
    const float FadeOutTime = 0.3f;
    const float RaysDegreePerSec = 60f;     // 1바퀴 6초
    const float SparkleSpeed = 4f;
    const float SlotSize = 120f;            // 60 논리, 5개 넘으면 52
    const float SmallSlotSize = 104f;

    [SerializeField] CanvasGroup group = null;
    [SerializeField] RectTransform rays = null;
    [SerializeField] RectTransform band = null;
    [SerializeField] RectTransform emblem = null;
    [SerializeField] RectTransform[] sparkles = null;
    [SerializeField] UIRewardSlot slotTemplate = null;  // 가로 줄(가운데 정렬)에 복제
    [SerializeField] GameObject guide = null;           // "화면을 누르면 바로 받아요" + 자동 닫힘 바
    [SerializeField] Image autoCloseFill = null;
    [SerializeField] Image flyIconTemplate = null;
    [SerializeField] Button touchPanel = null;

    readonly List<UIRewardSlot> slots = new();
    bool isTouched = false;

    private void Awake()
    {
        touchPanel.onClick.AddListener(() => isTouched = true);
    }

    // _rewards: 칸 순서대로 (첫 클리어 보상이 있으면 맨 앞, _firstCount개)
    public IEnumerator CoPlay(List<UIRewardPopup.Reward> _rewards, int _firstCount)
    {
        gameObject.SetActive(true);
        isTouched = false;
        float size = _rewards.Count > 5 ? SmallSlotSize : SlotSize;
        for (int i = 0; i < _rewards.Count; i++)
        {
            if (i >= slots.Count)
                slots.Add(i == 0 ? slotTemplate : Instantiate(slotTemplate, slotTemplate.transform.parent));
            slots[i].Setup(InGameManager.Instance.UIInGameScene.GetGoodsIcon(_rewards[i].GoodsId), _rewards[i].Value, size);
            slots[i].SetFirstTag(i < _firstCount);
            slots[i].gameObject.SetActive(true);
        }
        for (int i = _rewards.Count; i < slots.Count; i++)
            slots[i].gameObject.SetActive(false);
        slotTemplate.gameObject.SetActive(_rewards.Count > 0);
        guide.SetActive(false);

        // 문장·보상이 다 나올 때까지 (누르면 남은 것을 한 번에)
        float showEnd = SlotStart + SlotInterval * Mathf.Max(_rewards.Count - 1, 0) + SlotPopTime;
        for (float t = 0f; t < showEnd && !isTouched; t += Time.unscaledDeltaTime)
        {
            SetShow(t, _rewards.Count);
            yield return null;
        }
        SetShow(showEnd, _rewards.Count);

        // 자동 닫힘 바
        guide.SetActive(true);
        isTouched = false;
        for (float t = 0f; t < AutoCloseTime && !isTouched; t += Time.unscaledDeltaTime)
        {
            autoCloseFill.fillAmount = t / AutoCloseTime;
            Spin(t + showEnd);
            yield return null;
        }

        // 보상이 날아가며 문장·띠·빛줄기 흐려짐
        for (int i = 0; i < _rewards.Count; i++)
        {
            UIGoodsFly.Fly(this, flyIconTemplate, slots[i].transform.position, _rewards[i].GoodsId, _rewards[i].Value);
            slots[i].gameObject.SetActive(false);
        }
        guide.SetActive(false);
        for (float t = 0f; t < FadeOutTime; t += Time.unscaledDeltaTime)
        {
            group.alpha = 1f - t / FadeOutTime;
            yield return null;
        }
        yield return new WaitForSecondsRealtime(Mathf.Max(UIGoodsFly.TotalTime - FadeOutTime, 0f));
        gameObject.SetActive(false);
    }

    void SetShow(float _t, int _slotCount)
    {
        group.alpha = Mathf.Clamp01(_t / FadeInTime);
        band.localScale = new Vector3(Mathf.Clamp01((_t - BandStart) / BandOpenTime), 1f, 1f);
        emblem.localScale = Vector3.one * Pop(_t - EmblemStart, EmblemPopTime, 1.2f);
        for (int i = 0; i < _slotCount; i++)
            slots[i].transform.localScale = Vector3.one * Pop(_t - SlotStart - SlotInterval * i, SlotPopTime, 1.18f);
        Spin(_t);
    }

    // 0 → _peak(앞 60%) → 1.0
    float Pop(float _t, float _time, float _peak)
    {
        if (_t <= 0f)
            return 0f;
        float rate = Mathf.Clamp01(_t / _time);
        return rate < 0.6f ? Mathf.Lerp(0f, _peak, rate / 0.6f) : Mathf.Lerp(_peak, 1f, (rate - 0.6f) / 0.4f);
    }

    // 빛줄기 회전, 반짝임은 문장이 나온 뒤 저마다 다른 박자로 깜빡임
    void Spin(float _t)
    {
        rays.localEulerAngles = new Vector3(0f, 0f, -(_t - BandStart) * RaysDegreePerSec);
        for (int i = 0; i < sparkles.Length; i++)
        {
            float twinkle = _t < EmblemStart ? 0f : 0.5f + 0.5f * Mathf.Sin((_t - EmblemStart) * SparkleSpeed + i * 1.7f);
            sparkles[i].localScale = Vector3.one * twinkle;
        }
    }
}
