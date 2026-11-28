using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 코스튬 포인트 단계 줄 한 개 (LAYOUT_costume.md '단계 줄'): 받기 가능 / 받음(적용 중) / 다음 단계(진행 바) / 잠김
public class UICostumePointRow : MonoBehaviour
{
    const float LockedAlpha = 0.55f;
    const float StatY = -32f;
    const float NextStatY = -18f;   // 다음 단계는 진행 바 자리만큼 올림
    static readonly Color CreamColor = new Color32(0xff, 0xf3, 0xdc, 0xff);
    static readonly Color SubColor = new Color32(0xc2, 0xa8, 0x88, 0xff);
    static readonly Color DarkColor = new Color32(0x2a, 0x1a, 0x14, 0xff);

    [SerializeField] CanvasGroup group = null;
    [SerializeField] GameObject select = null;
    [SerializeField] Image tagImage = null;
    [SerializeField] TMP_Text tagText = null;
    [SerializeField] Sprite tagSprite = null;           // tag_best
    [SerializeField] Sprite tagLockSprite = null;       // card_chip
    [SerializeField] TMP_Text statText = null;
    [SerializeField] Button claimButton = null;
    [SerializeField] GameObject applied = null;
    [SerializeField] GameObject progress = null;
    [SerializeField] Image fill = null;
    [SerializeField] TMP_Text progressText = null;
    [SerializeField] GameObject lockIcon = null;

    public void Setup(CostumePointData _level, int _point, bool _isClaimed, bool _canClaim, bool _isNext, Action _onClaim)
    {
        bool isLocked = !_isClaimed && !_canClaim && !_isNext;
        group.alpha = isLocked ? LockedAlpha : 1f;
        select.SetActive(_canClaim);
        tagImage.sprite = isLocked ? tagLockSprite : tagSprite;
        tagText.text = $"{_level.Point}p";
        tagText.color = isLocked ? SubColor : DarkColor;
        statText.text = UICostumePopup.StatText(_level.Stat_Id, _level.Stat_Value);
        statText.color = isLocked ? SubColor : CreamColor;
        statText.rectTransform.anchoredPosition = new Vector2(statText.rectTransform.anchoredPosition.x, _isNext ? NextStatY : StatY);
        claimButton.gameObject.SetActive(_canClaim);
        applied.SetActive(_isClaimed);
        progress.SetActive(_isNext);
        fill.fillAmount = (float)_point / _level.Point;
        progressText.text = $"{_point}/{_level.Point}";
        lockIcon.SetActive(isLocked);
        claimButton.onClick.AddListener(() => _onClaim());
    }
}
