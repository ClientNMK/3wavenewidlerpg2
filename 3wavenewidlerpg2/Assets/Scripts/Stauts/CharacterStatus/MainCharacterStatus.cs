using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class MainCharacterStatus : CharacterStatus
{
    // 스탯 출처(장비, 성장 등)가 생기면 StatManagedData 필드를 추가하고 아래 합산에 더한다.
    public void ConvertStatDataToStatValue(StatType statType)
    {
        double totalValue =
            DefaultStatData.GetValue(statType);

        ConvertStatDataToStatValue(statType, totalValue);
    }
}
