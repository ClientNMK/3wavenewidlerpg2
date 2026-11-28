using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GameUtils;

// 미션 창 미션 줄 한 개 (kit_cocoa LAYOUT_mission.md '미션 줄')
public class UIMissionRow : MonoBehaviour
{
    static readonly Color CreamColor = new Color32(0xff, 0xf3, 0xdc, 0xff);
    static readonly Color SubColor = new Color32(0xc2, 0xa8, 0x88, 0xff);
    static readonly Color FullColor = new Color32(0x9b, 0xd6, 0x5a, 0xff);
    static readonly Color ProgressColor = new Color32(0xee, 0x7a, 0x5c, 0xff);
    const float ClaimedAlpha = 0.55f;
    const float ClaimedChipAlpha = 0.6f;

    [SerializeField] CanvasGroup group = null;
    [SerializeField] GameObject select = null;          // 받기 가능일 때 강조
    [SerializeField] Image kindIcon = null;
    [SerializeField] TMP_Text nameText = null;
    [SerializeField] Image fill = null;
    [SerializeField] TMP_Text progressText = null;
    [SerializeField] CanvasGroup rewardChip = null;
    [SerializeField] Image rewardIcon = null;
    [SerializeField] TMP_Text rewardText = null;
    [SerializeField] Button claimButton = null;
    [SerializeField] GameObject claimDot = null;
    [SerializeField] GameObject claimMulti = null;      // 반복 미션이 2배 이상 쌓였을 때 ×N
    [SerializeField] TMP_Text claimMultiText = null;
    [SerializeField] Button moveButton = null;
    [SerializeField] GameObject done = null;

    public void Setup(MissionData _mission, double _progress, int _claimCount, bool _isClaimed, Sprite _kind, Action _onClaim, Action _onMove)
    {
        group.alpha = _isClaimed ? ClaimedAlpha : 1f;
        select.SetActive(_claimCount > 0);
        kindIcon.sprite = _kind;
        nameText.text = StringManager.Instance.GetStringText(_mission.TextCode_Desc);
        nameText.color = _isClaimed ? SubColor : CreamColor;

        // 플레이 시간은 분으로, 일일·주간은 목표에서 멈춤, 반복은 실제 수
        double divide = _mission.Progress_Type == MissionProgressType.PlayTime ? 60d : 1d;
        double goal = _mission.Complete_Value;
        double shown = _isClaimed ? goal : _mission.Reset_Type == MissionResetType.Repeat ? _progress : Math.Min(_progress, goal);
        float rate = Mathf.Clamp01((float)(shown / goal));
        fill.fillAmount = rate;
        fill.color = rate >= 1f ? FullColor : ProgressColor;
        progressText.text = $"{Math.Floor(shown / divide).ToCurrencyString()}/{(goal / divide).ToCurrencyString()}";

        rewardChip.alpha = _isClaimed ? ClaimedChipAlpha : 1f;
        rewardIcon.sprite = InGameManager.Instance.UIInGameScene.GetGoodsIcon(_mission.Reward_Id);
        rewardText.text = (_mission.Reward_Value * Math.Max(_claimCount, 1)).ToCurrencyString();

        claimButton.gameObject.SetActive(_claimCount > 0);
        claimDot.SetActive(_claimCount == 1);
        claimMulti.SetActive(_claimCount > 1);
        claimMultiText.text = $"×{_claimCount}";
        moveButton.gameObject.SetActive(_claimCount == 0 && !_isClaimed);
        done.SetActive(_isClaimed);
        claimButton.onClick.AddListener(() => _onClaim());
        moveButton.onClick.AddListener(() => _onMove());
    }
}
