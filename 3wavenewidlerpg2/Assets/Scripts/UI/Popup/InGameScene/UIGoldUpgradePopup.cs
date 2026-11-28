using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 성장 창: 강화 탭(골드 강화, Gold_Id마다 한 줄) / 레벨 탭(스탯 포인트, StatPoint_Id마다 한 줄)
public class UIGoldUpgradePopup : UIPopup
{
    [Serializable]
    public struct StatBadge
    {
        public StatType StatType;
        public Color Color;
        public Sprite Icon;
    }

    static readonly Color TabOnTextColor = new Color32(0x2a, 0x1a, 0x14, 0xff);
    static readonly Color TabOffTextColor = new Color32(0xc2, 0xa8, 0x88, 0xff);

    [SerializeField] Button closeButton = null;
    [SerializeField] StatBadge[] statBadges = null;             // 능력치별 배지 색·아이콘

    [Header("Tab (0 = 강화, 1 = 레벨, 2 = 후드)")]
    [SerializeField] Button[] tabButtons = null;
    [SerializeField] GameObject[] tabPages = null;
    [SerializeField] Sprite tabOnSprite = null;
    [SerializeField] Sprite tabOffSprite = null;

    [Header("강화")]
    [SerializeField] UIGoldUpgradeSlot slotTemplate = null;     // 첫 줄로 쓰고 나머지는 복제
    [SerializeField] Transform listEnd = null;                  // 목록 끝 닫는 장식 (줄 복제 후 맨 뒤로)
    [SerializeField] Button[] segButtons = null;                // x1, x10, x100
    [SerializeField] Sprite segOnSprite = null;
    [SerializeField] Sprite segOffSprite = null;
    static readonly int[] SegAmounts = { 1, 10, 100 };
    static readonly Color SegOffTextColor = new Color32(0xff, 0xf3, 0xdc, 0xff);
    readonly List<UIGoldUpgradeSlot> slots = new();

    [Header("레벨")]
    [SerializeField] UIStatPointSlot statSlotTemplate = null;
    [SerializeField] Transform statListEnd = null;
    [SerializeField] TMP_Text statPointText = null;
    readonly List<UIStatPointSlot> statSlots = new();

    [Header("후드")]
    [SerializeField] UIHoodPage hoodPage = null;

    public override void Init()
    {
        if (bInitialized)
            return;

        touchPanel.onClick.AddListener(OnClickTouchPanel);
        closeButton.onClick.AddListener(OnClickCloseButton);
        for (int i = 0; i < tabButtons.Length; i++)
        {
            int tab = i;
            tabButtons[i].onClick.AddListener(() => SelectTab(tab));
        }
        for (int i = 0; i < segButtons.Length; i++)
        {
            int seg = i;
            segButtons[i].onClick.AddListener(() => SelectSeg(seg));
        }

        foreach (int goldId in DataManager.Instance.GoldUpgradeData.GetGoldIds())
        {
            StatBadge badge = GetBadge(DataManager.Instance.GoldUpgradeData.GetGoldUpgradeData(goldId, 0).Upgrade_Stat_Id);
            UIGoldUpgradeSlot slot = slots.Count == 0 ? slotTemplate : Instantiate(slotTemplate, slotTemplate.transform.parent);
            slot.Setup(goldId, badge.Color, badge.Icon);
            slots.Add(slot);
        }
        listEnd.SetAsLastSibling();

        foreach (int statPointId in DataManager.Instance.StatPointData.GetStatPointIds())
        {
            StatBadge badge = GetBadge(DataManager.Instance.StatPointData.GetStatPointData(statPointId, 0).Upgrade_Stat_Id);
            UIStatPointSlot slot = statSlots.Count == 0 ? statSlotTemplate : Instantiate(statSlotTemplate, statSlotTemplate.transform.parent);
            slot.Setup(statPointId, badge.Color, badge.Icon);
            slot.OnUpgrade = RefreshStatPoint;
            statSlots.Add(slot);
        }
        statListEnd.SetAsLastSibling();

        bInitialized = true;
    }

    StatBadge GetBadge(int _statId)
    {
        return Array.Find(statBadges, x => x.StatType == (StatType)_statId);
    }

    public override void ShowPopup()
    {
        base.ShowPopup();
        InGameManager.Instance.UIInGameScene.SetTabWindow(this);
    }

    public override void ClosePopup()
    {
        base.ClosePopup();
        InGameManager.Instance.UIInGameScene.SetTabWindow(null);
    }

    // 창이 열린 동안에도 전투는 계속되므로 골드·SP가 바뀌면 버튼 상태 갱신
    double lastGold = -1;
    double lastStatPoint = -1;
    private void Update()
    {
        if (lastGold != InGameManager.Instance.Gold)
        {
            lastGold = InGameManager.Instance.Gold;
            foreach (UIGoldUpgradeSlot slot in slots)
                slot.Refresh();
        }
        if (lastStatPoint != InGameManager.Instance.StatPoint)
        {
            lastStatPoint = InGameManager.Instance.StatPoint;
            RefreshStatPoint();
        }
    }

    // 닫혀 있는 동안 골드·스탯·레벨이 바뀌므로 열 때마다 갱신
    public override void Setup()
    {
        base.Setup();
        SelectTab(0);
        foreach (UIGoldUpgradeSlot slot in slots)
            slot.Refresh();
        RefreshStatPoint();
        hoodPage.Setup();
    }

    // 강화·레벨·후드 탭만 페이지가 있음 (전직·어빌리티는 아직 없음)
    public void SelectTab(int _tab)
    {
        if (_tab >= tabPages.Length)
            return;

        for (int i = 0; i < tabButtons.Length; i++)
        {
            tabButtons[i].image.sprite = i == _tab ? tabOnSprite : tabOffSprite;
            tabButtons[i].GetComponentInChildren<TMP_Text>().color = i == _tab ? TabOnTextColor : TabOffTextColor;
        }
        for (int i = 0; i < tabPages.Length; i++)
            tabPages[i].SetActive(i == _tab);
    }

    // 강화 탭 한 번에 올릴 레벨 수
    void SelectSeg(int _seg)
    {
        for (int i = 0; i < segButtons.Length; i++)
        {
            segButtons[i].image.sprite = i == _seg ? segOnSprite : segOffSprite;
            segButtons[i].GetComponentInChildren<TMP_Text>().color = i == _seg ? TabOnTextColor : SegOffTextColor;
        }
        foreach (UIGoldUpgradeSlot slot in slots)
        {
            slot.Amount = SegAmounts[_seg];
            slot.Refresh();
        }
    }

    void RefreshStatPoint()
    {
        statPointText.text = string.Format("SP {0:N0}", InGameManager.Instance.StatPoint);
        foreach (UIStatPointSlot slot in statSlots)
            slot.Refresh();
    }
}
