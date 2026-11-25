using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class CharacterStatus
{
    [Header("------ Stats ------")]
    // 캐릭터 스탯 : 기본 스탯 + 추가된 스탯
    public Status TotalStatus = new Status();

    [Header("------ Default Stats ------")]
    // 기본 스탯
    [HideInInspector] public StatManagedData DefaultStatData = new();

    bool bInitialized = false;

    public void Init()
    {
        if (bInitialized)
            return;

        TotalStatus = new Status();

        bInitialized = true;
    }

    public void SetDefaultStatData(DefaultStatData defaultStatData)
    {
        if (!bInitialized)
            Init();

        DefaultStatData = defaultStatData.DefaultStat;
        foreach (var item in defaultStatData.DefaultStat.DicStatListDatas)
        {
            StatType statType = item.Key;
            if (statType == StatType.None || statType == StatType.Max)
                continue;
            double statusValue = defaultStatData.DefaultStat.GetValue(statType);
            TotalStatus.SetupStatusValue(statType, statusValue);
        }
    }

    public double GetStatValue(StatType statusType)
    {
        double value = TotalStatus.GetStatusValue(statusType);
        return value;
    }

    public void ConvertStatDataToStatValue(StatType statusType, double value)
    {
        TotalStatus.SetupStatusValue(statusType, value);
    }

    public CharacterStatus CopyTo(CharacterStatus copyTarget)
    {
        if (copyTarget == null)
            copyTarget = new CharacterStatus();

        for (int i = (int)StatType.Attack; i < (int)StatType.Max; i++)
        {
            if (Enum.IsDefined(typeof(StatType), i) == false)
                continue;
            StatType statusType = (StatType)i;
            if (statusType == StatType.None || statusType == StatType.Max) continue;
            this.ConvertStatDataToStatValue(statusType, copyTarget.GetStatValue(statusType));
        }

        return this;
    }
}