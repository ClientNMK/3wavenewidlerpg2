using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Spine.Unity;

// 의상 창: 캐릭터 / 후드 탭 (kit_cocoa LAYOUT_costume.md). 칸을 누르면 위 미리보기·효과가 그 의상으로 바뀜
public class UICostumePopup : UIPopup
{
    const int Columns = 5;
    const float CellPitchX = 134f;
    const float CellPitchY = 144f;
    const float PreviewBodyHeight = 200f;
    static readonly Vector2 PreviewFoot = new(480f, -424f);     // 창 왼쪽 위 기준 몸 아래 가운데
    static readonly Color TabOnTextColor = new Color32(0x2a, 0x1a, 0x14, 0xff);
    static readonly Color TabOffTextColor = new Color32(0xc2, 0xa8, 0x88, 0xff);
    static readonly Color PointTextColor = new Color32(0xff, 0xe7, 0xa0, 0xff);
    static readonly Color UnequipTextColor = new Color32(0xff, 0xf3, 0xdc, 0xff);
    static readonly Color EquipValueColor = new Color32(0x9b, 0xd6, 0x5a, 0xff);
    public static readonly Color ShadowColor = new(0f, 0f, 0f, 0.45f);       // 없는 의상 그림
    // 일반·고급·희귀·영웅·전설·신화 (Grade 1~6)
    public static readonly Color[] GradeColors =
    {
        new Color32(0x9a, 0x8a, 0x7a, 0xff), new Color32(0x7c, 0xc4, 0x6a, 0xff), new Color32(0x5f, 0xa8, 0xe8, 0xff),
        new Color32(0xb4, 0x7c, 0xf0, 0xff), new Color32(0xf2, 0xb5, 0x44, 0xff), new Color32(0xe8, 0x60, 0x4e, 0xff),
    };

    [SerializeField] Button closeButton = null;
    [SerializeField] Button[] tabButtons = null;            // 0 캐릭터, 1 후드
    [SerializeField] Sprite tabOnSprite = null;
    [SerializeField] Sprite tabOffSprite = null;
    [SerializeField] SkeletonDataAsset[] skeletons = null;  // 탭 순서
    [SerializeField] Button pointButton = null;
    [SerializeField] TMP_Text pointText = null;
    [SerializeField] Image pointFill = null;
    [SerializeField] TMP_Text pointInfoText = null;
    [SerializeField] GameObject pointDot = null;
    [SerializeField] SkeletonGraphic preview = null;
    [SerializeField] Image gradeTag = null;
    [SerializeField] TMP_Text gradeText = null;
    [SerializeField] TMP_Text nameText = null;
    [SerializeField] TMP_Text descText = null;
    [SerializeField] TMP_Text givePointText = null;
    [SerializeField] TMP_Text haveText = null;
    [SerializeField] TMP_Text equipEffectText = null;
    [SerializeField] RectTransform cellContent = null;
    [SerializeField] UICostumeCell cellTemplate = null;
    [SerializeField] Button equipButton = null;
    [SerializeField] TMP_Text equipButtonText = null;
    [SerializeField] Sprite buttonOnSprite = null;
    [SerializeField] Sprite buttonOffSprite = null;
    [SerializeField] Sprite unequipSprite = null;           // hud_pill

    int tab = 0;
    CostumeData selected = null;

    InGameCostume Costume { get { return InGameManager.Instance.Costume; } }

    public override void Init()
    {
        if (bInitialized)
            return;

        touchPanel.onClick.AddListener(OnClickTouchPanel);
        closeButton.onClick.AddListener(OnClickCloseButton);
        for (int i = 0; i < tabButtons.Length; i++)
        {
            int index = i;
            tabButtons[i].onClick.AddListener(() => SelectTab(index));
        }
        pointButton.onClick.AddListener(() => PopupManager.Instance.ShowPopup<UICostumePointPopup>().OnCloseOnce = RefreshPoint);
        equipButton.onClick.AddListener(() =>
        {
            if (Costume.IsEquipped(selected))
                Costume.Unequip(selected);
            else
                Costume.Equip(selected);
            Refresh();
        });
        bInitialized = true;
    }

    public override void Setup()
    {
        base.Setup();
        SelectTab(0);
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

    List<CostumeData> TabCostumes { get { return DataManager.Instance.CostumeData.GetCostumes().FindAll(x => x.IsHood == (tab == 1)); } }

    // 처음 고른 칸은 입은 의상, 없으면 첫 칸
    void SelectTab(int _tab)
    {
        tab = _tab;
        for (int i = 0; i < tabButtons.Length; i++)
        {
            tabButtons[i].image.sprite = i == tab ? tabOnSprite : tabOffSprite;
            tabButtons[i].GetComponentInChildren<TMP_Text>().color = i == tab ? TabOnTextColor : TabOffTextColor;
        }
        selected = TabCostumes.Find(Costume.IsEquipped) ?? TabCostumes[0];
        Refresh();
        RefreshPoint();
    }

    void Refresh()
    {
        foreach (Transform child in cellContent)
            Destroy(child.gameObject);
        List<CostumeData> costumes = TabCostumes;
        for (int i = 0; i < costumes.Count; i++)
        {
            CostumeData costume = costumes[i];
            UICostumeCell cell = Instantiate(cellTemplate, cellContent);
            ((RectTransform)cell.transform).anchoredPosition += new Vector2(i % Columns * CellPitchX, -(i / Columns) * CellPitchY);
            cell.Setup(costume, skeletons[tab], Costume.IsOwned(costume), Costume.IsEquipped(costume), costume == selected, () =>
            {
                selected = costume;
                Refresh();
            });
        }
        cellContent.sizeDelta = new Vector2(cellContent.sizeDelta.x, (costumes.Count + Columns - 1) / Columns * CellPitchY);

        bool isOwned = Costume.IsOwned(selected);
        bool isEquipped = Costume.IsEquipped(selected);
        SetSkeleton(preview, skeletons[tab], selected.Skin_Name, true);
        preview.rectTransform.anchoredPosition = PreviewFoot + GameUtils.GameUtils.FitSkeletonBody(preview, PreviewBodyHeight, false);
        preview.color = isOwned ? Color.white : ShadowColor;
        gradeTag.color = GradeColors[selected.Grade - 1];
        gradeText.text = StringManager.Instance.GetStringText($"Name_Grade_{selected.Grade}");
        nameText.text = StringManager.Instance.GetStringText(selected.TextCode_Name);
        descText.text = StringManager.Instance.GetStringText(selected.TextCode_Desc);
        givePointText.text = string.Format(StringManager.Instance.GetStringText("Msg_CostumePointGive"), selected.Costume_Point);
        haveText.text = StatText(selected.Have_Stat_Id, selected.Have_Stat_Value);
        bool hasEquipStat = selected.Equip_Stat_Id > 0;
        equipEffectText.text = hasEquipStat ? StatText(selected.Equip_Stat_Id, selected.Equip_Stat_Value) : StringManager.Instance.GetStringText("Name_None");
        equipEffectText.color = hasEquipStat ? EquipValueColor : TabOffTextColor;

        // 착용 / 해제(입고 있는 의상) / 미보유
        equipButton.interactable = isOwned;
        equipButton.image.sprite = isEquipped ? unequipSprite : isOwned ? buttonOnSprite : buttonOffSprite;
        equipButtonText.text = StringManager.Instance.GetStringText(isEquipped ? "Name_Unequip" : isOwned ? "Name_Equip" : "Msg_NotOwned");
        equipButtonText.color = isEquipped ? UnequipTextColor : TabOnTextColor;
    }

    // 포인트 바: 다음 단계까지 채움, 받을 보상이 있으면 개수, 없으면 다음 단계 보상
    void RefreshPoint()
    {
        int point = Costume.Point;
        int claimable = Costume.ClaimableCount;
        CostumePointData next = DataManager.Instance.CostumeData.GetPoints().Find(x => x.Point > point);
        pointText.text = $"{point}p";
        pointFill.fillAmount = next == null ? 1f : (float)point / next.Point;
        pointDot.SetActive(claimable > 0);
        pointInfoText.text = claimable > 0 ? string.Format(StringManager.Instance.GetStringText("Msg_CostumeClaimable"), claimable)
            : next == null ? string.Empty : string.Format(StringManager.Instance.GetStringText("Msg_CostumeNext"), next.Point, StatText(next.Stat_Id, next.Stat_Value));
        pointInfoText.color = claimable > 0 ? PointTextColor : TabOffTextColor;
    }

    // 스켈레톤을 바꾸고 idle 첫 프레임을 바로 적용 (몸 크기를 재려면 첨부 그림이 정해져 있어야 함)
    public static void SetSkeleton(SkeletonGraphic _graphic, SkeletonDataAsset _asset, string _skin, bool _loop)
    {
        _graphic.skeletonDataAsset = _asset;
        _graphic.initialSkinName = _skin;
        _graphic.Initialize(true);
        _graphic.AnimationState.SetAnimation(0, "idle", _loop);
        _graphic.AnimationState.Apply(_graphic.Skeleton);
    }

    // "공격력 +50", 퍼센트 스탯은 0.01 = 1%
    public static string StatText(int _statId, double _value)
    {
        StatData stat = DataManager.Instance.StatData.GetStatData(_statId);
        string value = stat.Value_Type == StatValueType.Percent ? string.Format("{0:0.##}%", _value * 100) : _value.ToString("N0");
        return $"{StringManager.Instance.GetStringText(stat.TextCode_Name)} +{value}";
    }
}
