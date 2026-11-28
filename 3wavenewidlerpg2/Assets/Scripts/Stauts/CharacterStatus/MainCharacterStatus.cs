using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class MainCharacterStatus : CharacterStatus
{
    public StatManagedData GoldUpgradeStatData = new StatManagedData();

    public void SetGoldUpgradeStatData(GoldUpgradeData _goldUpgradeData, int _curLevel)
    {
        StatType statType = (StatType)_goldUpgradeData.Upgrade_Stat_Id;
        int diffLevel = _curLevel - _goldUpgradeData.Level_Start;
        double statValue = _goldUpgradeData.Upgrade_Stat_Value + (_goldUpgradeData.Upgrade_Stat_Increase_Value * diffLevel);
        GoldUpgradeStatData.SetStatDetailData(statType, _goldUpgradeData.Gold_Id.ToString(), statValue);
        ConvertStatDataToStatValue(statType);
    }

    public StatManagedData StatPointStatData = new StatManagedData();

    public void SetStatPointStatData(StatPointData _statPointData, int _curLevel)
    {
        StatType statType = (StatType)_statPointData.Upgrade_Stat_Id;
        int diffLevel = _curLevel - _statPointData.Level_Start;
        double statValue = _statPointData.Upgrade_Stat_Value + (_statPointData.Upgrade_Stat_Increase_Value * diffLevel);
        StatPointStatData.SetStatDetailData(statType, _statPointData.StatPoint_Id.ToString(), statValue);
        ConvertStatDataToStatValue(statType);
    }

    public StatManagedData CostumeStatData = new StatManagedData();

    // 코스튬 보유·착용 효과, 포인트 단계 (_key마다 따로, 빼려면 0)
    public void SetCostumeStatData(StatType _statType, string _key, double _value)
    {
        CostumeStatData.SetStatDetailData(_statType, _key, _value);
        ConvertStatDataToStatValue(_statType);
    }

    // 스탯 출처(장비, 성장 등)가 생기면 StatManagedData 필드를 추가하고 아래 합산에 더한다.
    public void ConvertStatDataToStatValue(StatType statType)
    {
        double totalValue =
            DefaultStatData.GetValue(statType)
            + GoldUpgradeStatData.GetValue(statType)
            + StatPointStatData.GetValue(statType)
            + CostumeStatData.GetValue(statType);

        ConvertStatDataToStatValue(statType, totalValue);
    }
}
