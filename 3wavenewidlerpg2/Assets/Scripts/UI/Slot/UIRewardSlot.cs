using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 보상 칸 하나: 칸 + 아이콘 + 아래 수량 (등급별 칸 부품은 아직 없음)
public class UIRewardSlot : MonoBehaviour
{
    [SerializeField] RectTransform frame = null;
    [SerializeField] Image icon = null;
    [SerializeField] TMP_Text amountText = null;
    [SerializeField] GameObject firstTag = null;    // "첫 클리어" 꼬리표 (보스 클리어 연출 칸만)

    public void Setup(Sprite _icon, double _value, float _size)
    {
        frame.sizeDelta = new Vector2(_size, _size);
        icon.sprite = _icon;
        icon.rectTransform.sizeDelta = Vector2.one * _size * 0.5f;
        amountText.text = _value.ToString("N0");
    }

    public void SetFirstTag(bool _active)
    {
        firstTag.SetActive(_active);
    }
}
