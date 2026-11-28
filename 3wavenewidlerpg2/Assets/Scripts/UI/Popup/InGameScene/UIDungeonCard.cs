using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Spine.Unity;

// 컨텐츠 창 던전 탭 카드 한 장 (kit_cocoa LAYOUT_dungeon.md '던전 탭')
public class UIDungeonCard : MonoBehaviour
{
    const int TicketMax = 2;    // 티켓은 차감 없이 칸만 표시 (가안 매일 2장)

    [SerializeField] Image bg = null;
    [SerializeField] SkeletonGraphic monster = null;
    [SerializeField] Image frame = null;
    [SerializeField] Sprite frameLockSprite = null;
    [SerializeField] TMP_Text nameText = null;
    [SerializeField] TMP_Text timeText = null;
    [SerializeField] TMP_Text progressText = null;
    [SerializeField] CanvasGroup rewardChip = null;
    [SerializeField] TMP_Text rewardStepText = null;
    [SerializeField] Image rewardIcon = null;
    [SerializeField] TMP_Text rewardText = null;
    [SerializeField] GameObject ticket = null;
    [SerializeField] TMP_Text ticketText = null;
    [SerializeField] Button sweepButton = null;
    [SerializeField] TMP_Text sweepText = null;
    [SerializeField] Button enterButton = null;
    [SerializeField] GameObject lockGroup = null;
    [SerializeField] TMP_Text lockText = null;
    [SerializeField] Sprite sweepOnSprite = null;       // hud_pill
    [SerializeField] Sprite buttonOffSprite = null;

    // 소탕 = 깬 최고 단계 보상만 (단계는 안 오름), 입장 = 다음 단계
    public void Setup(int _index, List<DungeonData> _steps, int _clearedStep, Action _onEnter, Action _onSweep)
    {
        DungeonData first = _steps[0];
        bool isLocked = first.Recommended_Stage > UserManager.Instance.MaxStageId;
        bg.sprite = ResourceManager.Instance.GetSprite(AtlasType.Bg, $"bg_dungeon_{first.Dungeon_Type}");
        bg.enabled = bg.sprite != null;
        UIContentPopup.SetupMonster(monster, first.Monster_Id, UIContentPopup.CardMonsterHeight);
        nameText.text = string.Format(StringManager.Instance.GetStringText("Name_DungeonNumber"), _index + 1);
        enterButton.onClick.AddListener(() => _onEnter());
        DungeonData next = _steps[Mathf.Min(_clearedStep, _steps.Count - 1)];
        timeText.text = string.Format(StringManager.Instance.GetStringText("Msg_Seconds"), next.Limit_Time);

        // 잠긴 던전: 그림 어둡게, 몬스터 실루엣, 잠금 안내만
        bg.color = isLocked ? UIContentPopup.LockBgColor : Color.white;
        monster.color = isLocked ? UIContentPopup.SilhouetteColor : Color.white;
        if (isLocked)
            frame.sprite = frameLockSprite;
        nameText.color = isLocked ? UIContentPopup.SubColor : UIContentPopup.CreamColor;
        rewardChip.gameObject.SetActive(!isLocked);
        ticket.SetActive(!isLocked);
        sweepButton.gameObject.SetActive(!isLocked);
        enterButton.gameObject.SetActive(!isLocked);
        lockGroup.SetActive(isLocked);
        if (isLocked)
        {
            StageData stage = DataManager.Instance.StageData.GetStageData(first.Recommended_Stage);
            progressText.text = StringManager.Instance.GetStringText("Msg_Preparing");
            progressText.color = UIContentPopup.SubColor;
            lockText.text = string.Format(StringManager.Instance.GetStringText("Msg_DungeonUnlock"), $"{stage.World_Id}-{stage.Stage_Number}");
            return;
        }

        bool isCleared = _clearedStep > 0;
        progressText.text = isCleared ? string.Format(StringManager.Instance.GetStringText("Msg_StepCleared"), _clearedStep) : StringManager.Instance.GetStringText("Msg_NotCleared");
        progressText.color = isCleared ? UIContentPopup.HighlightColor : UIContentPopup.SubColor;

        // 깬 최고 단계 = 소탕 보상, 안 깼으면 1단계 보상을 흐리게
        DungeonData reward = _steps[Mathf.Max(_clearedStep, 1) - 1];
        rewardChip.alpha = isCleared ? 1f : 0.5f;
        rewardStepText.text = string.Format(StringManager.Instance.GetStringText("Msg_StepReward"), reward.Step);
        rewardIcon.gameObject.SetActive(reward.Reward_Item_Id != 0);
        rewardText.gameObject.SetActive(reward.Reward_Item_Id != 0);
        if (reward.Reward_Item_Id != 0)
        {
            rewardIcon.sprite = InGameManager.Instance.UIInGameScene.GetGoodsIcon(reward.Reward_Item_Id);
            rewardText.text = string.Format("{0:N0}", reward.Reward_Item_Value);
        }

        ticketText.text = $"{TicketMax}/{TicketMax}";
        sweepButton.image.sprite = isCleared ? sweepOnSprite : buttonOffSprite;
        sweepText.color = isCleared ? UIContentPopup.CreamColor : UIContentPopup.DarkColor;
        if (isCleared)
            sweepButton.onClick.AddListener(() => _onSweep());
    }
}
