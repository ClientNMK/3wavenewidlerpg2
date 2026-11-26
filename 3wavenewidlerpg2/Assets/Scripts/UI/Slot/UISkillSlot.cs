using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 스킬 슬롯 한 칸: 아이콘, 쿨타임 덮개·남은 초, 누르면 자동 사용 ON/OFF
public class UISkillSlot : MonoBehaviour
{
    [SerializeField] Button button = null;
    [SerializeField] Image icon = null;
    [SerializeField] Image coolImage = null;
    [SerializeField] TMP_Text coolText = null;
    [SerializeField] TMP_Text autoText = null;

    MainCharacter.SkillSlot slot = null;

    public void Setup(MainCharacter.SkillSlot _slot)
    {
        slot = _slot;
        bool isEmpty = slot.SkillId == 0;
        button.interactable = !isEmpty;
        icon.gameObject.SetActive(!isEmpty);
        autoText.gameObject.SetActive(!isEmpty);
        coolImage.fillAmount = 0f;
        coolText.text = string.Empty;
        if (!isEmpty)
            button.onClick.AddListener(ToggleAuto);
        RefreshAuto();
    }

    void ToggleAuto()
    {
        slot.IsAuto = !slot.IsAuto;
        RefreshAuto();
    }

    // OFF면 아이콘을 어둡게
    void RefreshAuto()
    {
        autoText.text = slot.IsAuto ? "ON" : "OFF";
        icon.color = slot.IsAuto ? Color.white : Color.gray;
    }

    private void Update()
    {
        if (slot.SkillId == 0)
            return;

        float remainTime = slot.RemainTime;
        coolImage.fillAmount = remainTime / slot.SkillData.Cooltime;
        coolText.text = remainTime > 0f ? remainTime.ToString("0.0") : string.Empty;
    }
}
