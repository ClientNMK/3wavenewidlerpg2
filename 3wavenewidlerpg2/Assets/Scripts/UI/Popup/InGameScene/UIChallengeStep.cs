using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 컨텐츠 창 도전 탭 단계 줄 (kit_cocoa LAYOUT_dungeon.md '단계 줄')
public class UIChallengeStep : MonoBehaviour
{
    public enum StepState { Cleared, Next, Locked }

    static readonly Color ReceivedColor = new Color32(0x9b, 0xd6, 0x5a, 0xff);
    static readonly Color LockTextColor = new Color32(0xc2, 0xa8, 0x88, 0x8c);

    [SerializeField] Image bg = null;
    [SerializeField] Sprite clearedSprite = null;
    [SerializeField] Sprite nextSprite = null;
    [SerializeField] Sprite lockedSprite = null;
    [SerializeField] TMP_Text stepText = null;
    [SerializeField] GameObject received = null;        // "받음" + 체크
    [SerializeField] GameObject select = null;          // 다음 도전 테두리 + "도전" 꼬리표
    [SerializeField] GameObject lockIcon = null;
    [SerializeField] CanvasGroup firstClearReward = null;
    [SerializeField] Image firstClearIcon = null;
    [SerializeField] TMP_Text firstClearText = null;

    public void Setup(DungeonData _data, StepState _state)
    {
        bg.sprite = _state == StepState.Cleared ? clearedSprite : _state == StepState.Next ? nextSprite : lockedSprite;
        stepText.text = string.Format(StringManager.Instance.GetStringText("Msg_Step"), _data.Step);
        stepText.color = _state == StepState.Cleared ? UIContentPopup.CreamColor : _state == StepState.Next ? UIContentPopup.HighlightColor : LockTextColor;
        // 잠긴 줄은 자물쇠 오른쪽으로 글자를 밀어 둠
        stepText.rectTransform.anchoredPosition = new Vector2(_state == StepState.Locked ? 68f : 28f, stepText.rectTransform.anchoredPosition.y);
        received.SetActive(_state == StepState.Cleared);
        select.SetActive(_state == StepState.Next);
        lockIcon.SetActive(_state == StepState.Locked);

        // 모든 줄에 첫 클리어 보상 (아이콘은 기본 icon_gem, 차트에 재화가 있으면 그 아이콘)
        firstClearReward.alpha = _state == StepState.Locked ? 0.5f : 1f;
        if (_data.FirstClear_Reward_Id != 0)
            firstClearIcon.sprite = InGameManager.Instance.UIInGameScene.GetGoodsIcon(_data.FirstClear_Reward_Id);
        firstClearText.text = string.Format("+{0:N0}", _data.FirstClear_Reward_Value);
    }
}
