using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 코스튬 포인트 창 (LAYOUT_costume.md ③). 받은 단계 스탯은 바로 적용
public class UICostumePointPopup : UIPopup
{
    const float RowTop = 12f;       // 받기 강조 테두리가 위로 넘치는 만큼 띄움
    const float RowPitch = 104f;

    [SerializeField] Button closeButton = null;
    [SerializeField] TMP_Text totalText = null;
    [SerializeField] RectTransform rowContent = null;
    [SerializeField] UICostumePointRow rowTemplate = null;
    [SerializeField] Button claimAllButton = null;
    [SerializeField] Sprite buttonOnSprite = null;
    [SerializeField] Sprite buttonOffSprite = null;

    InGameCostume Costume { get { return InGameManager.Instance.Costume; } }

    public override void Init()
    {
        if (bInitialized)
            return;

        touchPanel.onClick.AddListener(OnClickTouchPanel);
        closeButton.onClick.AddListener(OnClickCloseButton);
        claimAllButton.onClick.AddListener(() => Claim(DataManager.Instance.CostumeData.GetPoints().FindAll(Costume.CanClaim)));
        bInitialized = true;
    }

    public override void Setup()
    {
        base.Setup();
        Refresh();
    }

    // 다음 단계 = 아직 닿지 않은 첫 단계, 그 뒤는 잠김
    void Refresh()
    {
        int point = Costume.Point;
        totalText.text = $"{point}p";
        foreach (Transform child in rowContent)
            Destroy(child.gameObject);
        List<CostumePointData> levels = DataManager.Instance.CostumeData.GetPoints();
        CostumePointData next = levels.Find(x => x.Point > point);
        for (int i = 0; i < levels.Count; i++)
        {
            CostumePointData level = levels[i];
            UICostumePointRow row = Instantiate(rowTemplate, rowContent);
            ((RectTransform)row.transform).anchoredPosition = new Vector2(((RectTransform)row.transform).anchoredPosition.x, -(RowTop + i * RowPitch));
            row.Setup(level, point, Costume.IsClaimed(level), Costume.CanClaim(level), level == next, () => Claim(new List<CostumePointData> { level }));
        }
        rowContent.sizeDelta = new Vector2(rowContent.sizeDelta.x, RowTop + levels.Count * RowPitch);

        bool canClaim = Costume.ClaimableCount > 0;
        claimAllButton.interactable = canClaim;
        claimAllButton.image.sprite = canClaim ? buttonOnSprite : buttonOffSprite;
    }

    void Claim(List<CostumePointData> _levels)
    {
        foreach (CostumePointData level in _levels)
            Costume.Claim(level);
        Refresh();
    }
}
