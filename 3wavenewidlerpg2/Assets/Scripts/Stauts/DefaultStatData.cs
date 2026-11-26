using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DefaultStatData", menuName = "Scriptable Object/DefaultStatData", order = int.MaxValue)]
public class DefaultStatData : ScriptableObject
{
    public Status status = null;
    public Status Status { get { return status; } }
    public StatManagedData DefaultStat = new StatManagedData();

    public void Init()
    {
        DefaultStat = new StatManagedData();
        // 후드 스탯(1001~)은 메인 캐릭터와 별개
        foreach (var item in DataManager.Instance.StatData.Datas.Values)
            if (item.Stat_Type < StatType.Hood_Predation_Hp_Per)
                Setup(item.Stat_Type, item.Stat_Default_Value);
        ConvertAllStatDataToStatus();
    }
    public void Setup(StatType statusType, double value)
    {
        DefaultStat.SetStatDetailData(statusType, statusType.ToString(), value);
    }

    public void ConvertAllStatDataToStatus()
    {
        status = new Status();
        foreach (var item in DefaultStat.DicStatListDatas)
        {
            StatType statType = item.Key;
            if (statType == StatType.None || statType == StatType.Max)
                continue;
            double statusValue = DefaultStat.GetValue(statType);
            status.SetupStatusValue(statType, statusValue);
        }
    }

}
