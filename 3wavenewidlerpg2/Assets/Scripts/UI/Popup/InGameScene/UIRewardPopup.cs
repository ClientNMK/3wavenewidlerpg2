using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 공용 보상 획득 연출 (kit_cocoa LAYOUT_reward.md, 시안 Plan/mockups/review/reward_get_sequence.png)
// Setup(보상 목록) → 칸이 하나씩 튀어나옴 → 누르면 아이콘이 재화 칸으로 날아가고 도착할 때 실제로 지급
public class UIRewardPopup : UIPopup
{
    public struct Reward
    {
        public int GoodsId;
        public double Value;
        public Reward(int _goodsId, double _value) { GoodsId = _goodsId; Value = _value; }
    }

    const float FadeInTime = 0.15f;
    const float SlotInterval = 0.12f;
    const float SlotPopTime = 0.2f;         // 0 → 1.18 → 1.0
    const float RaysDegreePerSec = 60f;     // 1바퀴 6초
    const float SingleSlotSize = 128f;      // 1개 64 논리
    const float MultiSlotSize = 104f;       // 여러 개 52 논리
    const float TitleGap = 60f;             // 보상 칸 위 30 논리
    const float GuideGap = 30f;             // 보상 칸 아래 안내
    public const float DefaultAutoCloseTime = 3f;   // 다 나온 뒤 누르지 않아도 닫힘 (자동 보스 도전 등으로 다음 보상과 겹치지 않게)
    float autoCloseTime = DefaultAutoCloseTime;

    [SerializeField] CanvasGroup contentGroup = null;   // 어두운 판·빛줄기·제목·칸·안내
    [SerializeField] RectTransform rays = null;
    [SerializeField] RectTransform title = null;
    [SerializeField] RectTransform guide = null;        // "화면을 누르면 닫혀요"
    [SerializeField] GridLayoutGroup grid = null;
    [SerializeField] UIRewardSlot slotTemplate = null;
    [SerializeField] Image flyIconTemplate = null;

    readonly List<UIRewardSlot> slots = new();
    List<Reward> rewards = null;
    bool isShowingSlots = false;
    bool isClosing = false;

    public override void Init()
    {
        if (bInitialized)
            return;

        touchPanel.onClick.AddListener(OnClickTouchPanel);
        bInitialized = true;
    }

    public void Setup(List<Reward> _rewards, float _autoCloseTime = DefaultAutoCloseTime)
    {
        rewards = _rewards;
        autoCloseTime = _autoCloseTime;
        isClosing = false;
        contentGroup.alpha = 0f;
        float size = rewards.Count == 1 ? SingleSlotSize : MultiSlotSize;
        grid.cellSize = new Vector2(size, size + 40f);  // 아래 수량 글자 자리

        for (int i = 0; i < rewards.Count; i++)
        {
            if (i >= slots.Count)
                slots.Add(i == 0 ? slotTemplate : Instantiate(slotTemplate, slotTemplate.transform.parent));
            slots[i].Setup(InGameManager.Instance.UIInGameScene.GetGoodsIcon(rewards[i].GoodsId), rewards[i].Value, size);
            slots[i].transform.localScale = Vector3.zero;
            slots[i].gameObject.SetActive(true);
        }
        for (int i = rewards.Count; i < slots.Count; i++)
            slots[i].gameObject.SetActive(false);

        // 칸 묶음(가운데 기준)의 위·아래에 제목·안내
        RectTransform gridRect = (RectTransform)grid.transform;
        LayoutRebuilder.ForceRebuildLayoutImmediate(gridRect);
        float half = gridRect.rect.height * 0.5f;
        title.anchoredPosition = new Vector2(0f, gridRect.anchoredPosition.y + half + TitleGap + title.rect.height * 0.5f);
        guide.anchoredPosition = new Vector2(0f, gridRect.anchoredPosition.y - half - GuideGap);

        StopAllCoroutines();
        StartCoroutine(CoShow());
    }

    private void Update()
    {
        rays.localEulerAngles = new Vector3(0f, 0f, rays.localEulerAngles.z - RaysDegreePerSec * Time.unscaledDeltaTime);
    }

    IEnumerator CoShow()
    {
        isShowingSlots = true;
        for (float t = 0f; t < FadeInTime; t += Time.unscaledDeltaTime)
        {
            contentGroup.alpha = t / FadeInTime;
            yield return null;
        }
        contentGroup.alpha = 1f;

        for (int i = 0; i < rewards.Count; i++)
        {
            StartCoroutine(CoPop(slots[i].transform));
            yield return new WaitForSecondsRealtime(SlotInterval);
        }
        yield return new WaitForSecondsRealtime(SlotPopTime);
        isShowingSlots = false;
        yield return CoAutoClose();
    }

    IEnumerator CoAutoClose()
    {
        yield return new WaitForSecondsRealtime(autoCloseTime);
        OnClickTouchPanel();
    }

    IEnumerator CoPop(Transform _slot)
    {
        for (float t = 0f; t < SlotPopTime; t += Time.unscaledDeltaTime)
        {
            float rate = t / SlotPopTime;
            _slot.localScale = Vector3.one * (rate < 0.6f ? Mathf.Lerp(0f, 1.18f, rate / 0.6f) : Mathf.Lerp(1.18f, 1f, (rate - 0.6f) / 0.4f));
            yield return null;
        }
        _slot.localScale = Vector3.one;
    }

    // 뒤로 가기도 화면 누르기와 같게 (바로 닫으면 보상이 지급되지 않음)
    public override void OnClickBackButton()
    {
        OnClickTouchPanel();
    }

    // 다 나오기 전에 누르면 남은 칸을 한 번에, 다 나온 뒤 누르면 날아가며 닫힘
    public override void OnClickTouchPanel()
    {
        if (isClosing)
            return;

        if (isShowingSlots)
        {
            StopAllCoroutines();
            contentGroup.alpha = 1f;
            for (int i = 0; i < rewards.Count; i++)
                slots[i].transform.localScale = Vector3.one;
            isShowingSlots = false;
            StartCoroutine(CoAutoClose());
            return;
        }
        isClosing = true;
        StopAllCoroutines();
        StartCoroutine(CoFlyAndClose());
    }

    // 보상마다 아이콘이 재화 칸으로 날아가며 닫힘 (도착할 때 지급)
    IEnumerator CoFlyAndClose()
    {
        contentGroup.alpha = 0f;
        for (int i = 0; i < rewards.Count; i++)
            UIGoodsFly.Fly(this, flyIconTemplate, slots[i].transform.position, rewards[i].GoodsId, rewards[i].Value);
        yield return new WaitForSecondsRealtime(UIGoodsFly.TotalTime);
        PopupManager.Instance.ClosePopup(this);
    }
}
