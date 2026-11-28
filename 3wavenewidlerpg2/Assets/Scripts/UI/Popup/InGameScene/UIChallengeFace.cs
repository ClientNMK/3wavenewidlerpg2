using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 컨텐츠 창 도전 탭 보스 얼굴 한 칸 (kit_cocoa LAYOUT_dungeon.md '보스 얼굴 줄')
public class UIChallengeFace : MonoBehaviour
{
    static readonly Color FaceBgColor = new Color32(0x3a, 0x25, 0x1c, 0xff);
    static readonly Color LockFaceBgColor = new Color32(0x1e, 0x14, 0x10, 0xff);

    public Button Button = null;
    [SerializeField] Image faceBg = null;
    [SerializeField] Image face = null;                 // Resources/Portrait/portrait_{몬스터 Resource}
    [SerializeField] Image ring = null;
    [SerializeField] Sprite ringLockSprite = null;
    [SerializeField] GameObject select = null;
    [SerializeField] GameObject lockIcon = null;
    [SerializeField] TMP_Text stateText = null;

    public void Setup(int _monsterId, bool _isLocked, int _clearedStep)
    {
        face.sprite = Resources.Load<Sprite>("Portrait/portrait_" + DataManager.Instance.MonsterData.GetMonsterData(_monsterId).Resource);
        face.color = _isLocked ? new Color(0f, 0f, 0f, 0.55f) : Color.white;
        faceBg.color = _isLocked ? LockFaceBgColor : FaceBgColor;
        if (_isLocked)
            ring.sprite = ringLockSprite;
        lockIcon.SetActive(_isLocked);
        stateText.text = _isLocked ? StringManager.Instance.GetStringText("Name_Locked")
            : _clearedStep > 0 ? string.Format(StringManager.Instance.GetStringText("Msg_Step"), _clearedStep)
            : StringManager.Instance.GetStringText("Msg_BeforeChallenge");
    }

    // 고른 보스는 face_ring_sel, 글자 밝게
    public void SetSelect(bool _isOn)
    {
        select.SetActive(_isOn);
        ring.enabled = !_isOn;
        stateText.color = _isOn ? UIContentPopup.CreamColor : UIContentPopup.SubColor;
    }
}
