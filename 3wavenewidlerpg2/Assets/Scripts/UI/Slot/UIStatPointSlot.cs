using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 스탯 포인트(SP) 한 줄 (StatPoint_Id 하나)
public class UIStatPointSlot : MonoBehaviour
{
    [SerializeField] Image badgeFill = null;
    [SerializeField] Image icon = null;
    [SerializeField] TMP_Text nameText = null;
    [SerializeField] TMP_Text valueText = null;
    [SerializeField] TMP_Text costText = null;
    [SerializeField] Button upgradeButton = null;

    int statPointId = 0;
    int CurLevel { get { return InGameBuffManager.Instance.StatPointLevels.GetValueOrDefault(statPointId); } }
    public System.Action OnUpgrade = null;      // SP가 줄면 다른 줄·SP 표시 갱신

    public void Setup(int _statPointId, Color _badgeColor, Sprite _icon)
    {
        statPointId = _statPointId;
        badgeFill.color = _badgeColor;
        icon.sprite = _icon;
        upgradeButton.onClick.AddListener(OnClickUpgradeButton);
        Refresh();
    }

    public void Refresh()
    {
        int level = CurLevel;
        StatPointData data = DataManager.Instance.StatPointData.GetStatPointData(statPointId, level);
        StatData statData = DataManager.Instance.StatData.GetStatData(data.Upgrade_Stat_Id);
        nameText.text = string.Format("{0}  <color=#C2A888><size=80%>Lv.{1}</size></color>", StringManager.Instance.GetStringText(statData.TextCode_Name), level);

        // 퍼센트 스탯은 0.01 = 1%
        double curValue = InGameBuffManager.Instance.BuffStatus.GetStatValue((StatType)data.Upgrade_Stat_Id);
        double nextValue = statData.Stat_Default_Value + data.Upgrade_Stat_Value + data.Upgrade_Stat_Increase_Value * (level + 1 - data.Level_Start);
        bool isPercent = statData.Value_Type == StatValueType.Percent;
        bool isLock = InGameManager.Instance.Level < data.Open_CharLevel;
        bool isMax = level >= data.Max_Level;
        upgradeButton.interactable = !isLock && !isMax && InGameManager.Instance.StatPoint >= data.Upgrade_Goods_Value;

        valueText.text = isMax ? Format(curValue, isPercent) : string.Format("{0} > {1}", Format(curValue, isPercent), Format(nextValue, isPercent));
        if (isLock)
            costText.text = string.Format("Lv.{0}", data.Open_CharLevel);
        else if (isMax)
            costText.text = "MAX";
        else
            costText.text = string.Format("SP {0:N0}", data.Upgrade_Goods_Value);
    }

    string Format(double _value, bool _isPercent)
    {
        return _isPercent ? string.Format("{0:0.##}%", _value * 100) : _value.ToString("N0");
    }

    void OnClickUpgradeButton()
    {
        int level = CurLevel;
        StatPointData data = DataManager.Instance.StatPointData.GetStatPointData(statPointId, level);
        if (!InGameManager.Instance.TryUseStatPoint(data.Upgrade_Goods_Value))
            return;

        InGameBuffManager.Instance.SetStatPointLevel(statPointId, level + 1);
        OnUpgrade?.Invoke();
    }
}
