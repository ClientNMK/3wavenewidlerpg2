using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 번호는 StatChart의 Stat_Id와 같아야 한다.
public enum StatType
{
    None = 0,

    Attack = 1,                 // 공격력
    Hp = 101,                   // 최대 체력
    Defence = 201,              // 방어력
    Penetrate = 301,            // 관통
    Cri = 401,                  // 치명타 확률
    Cri_Dmg = 402,              // 치명타 피해 배율
    Move_Speed = 501,           // 이동 속도
    Move_Speed_Per,             // 이동 속도 증가율
    Attack_Speed,               // 공격 속도
    Attack_Speed_Per,           // 공격 속도 증가율
    Attack_Range,               // 공격 범위
    Attack_Number,              // 한 번에 공격하는 마릿수
    Damage_Min_Per,             // 최소 데미지 배율 (공격력 대비)
    Damage_Max_Per,             // 최대 데미지 배율 (공격력 대비)
    Predation_Per = 601,        // 평타 타격 시 포식 상태이상 확률
    Recovery_Hp_Per = 602,      // 1초마다 최대 체력 비례 회복 (0.01 = 1%)

    // 1001~ 후드 스탯: 메인 캐릭터 스탯에 넣지 않음
    Hood_Predation_Hp_Per = 1001,   // 후드가 포식할 수 있는 일반 몬스터 체력 비율 (최대 체력 대비)

    Max,
}
public enum StatValueType
{
    Value,
    Percent,
}

[Serializable]
public class Status
{
    public BigDouble Attack;
    public BigDouble Hp;
    public double Defence;
    public double Penetrate;
    public double Cri;
    public double Cri_Dmg;
    public double Move_Speed;
    public double Move_Speed_Per;
    public double Attack_Speed;
    public double Attack_Speed_Per;
    public double Attack_Range;
    public double Attack_Number;
    public double Damage_Min_Per;
    public double Damage_Max_Per;
    public double Predation_Per;
    public double Recovery_Hp_Per;

    public double TotalMoveSpeed { get { return Move_Speed * (1 + Move_Speed_Per); } }
    public double TotalAttackSpeed { get { return Attack_Speed * (1 + Attack_Speed_Per); } }

    public void SetupStatusValue(StatType statType, double value)
    {
        switch (statType)
        {
            case StatType.Attack:
                Attack = new BigDouble(value);
                break;
            case StatType.Hp:
                Hp = new BigDouble(value);
                break;
            case StatType.Defence:
                Defence = value;
                break;
            case StatType.Penetrate:
                Penetrate = value;
                break;
            case StatType.Cri:
                Cri = value;
                break;
            case StatType.Cri_Dmg:
                Cri_Dmg = value;
                break;
            case StatType.Move_Speed:
                Move_Speed = value;
                break;
            case StatType.Move_Speed_Per:
                Move_Speed_Per = value;
                break;
            case StatType.Attack_Speed:
                Attack_Speed = value;
                break;
            case StatType.Attack_Speed_Per:
                Attack_Speed_Per = value;
                break;
            case StatType.Attack_Range:
                Attack_Range = value;
                break;
            case StatType.Attack_Number:
                Attack_Number = value;
                break;
            case StatType.Damage_Min_Per:
                Damage_Min_Per = value;
                break;
            case StatType.Damage_Max_Per:
                Damage_Max_Per = value;
                break;
            case StatType.Predation_Per:
                Predation_Per = value;
                break;
            case StatType.Recovery_Hp_Per:
                Recovery_Hp_Per = value;
                break;
        }
    }

    public double GetStatusValue(StatType statType)
    {
        switch (statType)
        {
            case StatType.Attack:
                return Attack.ToDouble();
            case StatType.Hp:
                return Hp.ToDouble();
            case StatType.Defence:
                return Defence;
            case StatType.Penetrate:
                return Penetrate;
            case StatType.Cri:
                return Cri;
            case StatType.Cri_Dmg:
                return Cri_Dmg;
            case StatType.Move_Speed:
                return Move_Speed;
            case StatType.Move_Speed_Per:
                return Move_Speed_Per;
            case StatType.Attack_Speed:
                return Attack_Speed;
            case StatType.Attack_Speed_Per:
                return Attack_Speed_Per;
            case StatType.Attack_Range:
                return Attack_Range;
            case StatType.Attack_Number:
                return Attack_Number;
            case StatType.Damage_Min_Per:
                return Damage_Min_Per;
            case StatType.Damage_Max_Per:
                return Damage_Max_Per;
            case StatType.Predation_Per:
                return Predation_Per;
            case StatType.Recovery_Hp_Per:
                return Recovery_Hp_Per;
        }
        return 0;
    }
}

[Serializable]
public class StatDetailData
{
    public string key;
    public double value;
    public StatDetailData(string key, double value)
    {
        this.key = key;
        this.value = value;
    }
}

[Serializable]
public class StatListData
{
    public List<StatDetailData> StatDetailDataList = new List<StatDetailData>();
    public double TotalValue;

    public void SetStat(string key, double value)
    {
        if (string.IsNullOrEmpty(key))
            return;

        StatDetailData statDetailData = StatDetailDataList.Find(x => x.key.Equals(key));
        if (statDetailData == null)
        {
            statDetailData = new StatDetailData(key, value);
            StatDetailDataList.Add(statDetailData);
            TotalValue += value;
        }
        else
        {
            double diffValue = value - statDetailData.value;
            statDetailData.value = value;
            TotalValue += diffValue;
        }
    }

    public void RemoveStat(string key)
    {
        if (string.IsNullOrEmpty(key))
            return;

        StatDetailData statDetailData = StatDetailDataList.Find(x => x.key.Equals(key));
        if (statDetailData == null)
            return;

        double value = statDetailData.value;
        TotalValue -= value;
        if (-0.0000001 < TotalValue && TotalValue < 0.0000001) TotalValue = 0;
        StatDetailDataList.Remove(statDetailData);
    }

    public double GetValue(string key)
    {
        if (string.IsNullOrEmpty(key))
            return 0;

        StatDetailData statDetailData = StatDetailDataList.Find(x => x.key.Equals(key));
        if (statDetailData != null)
            return statDetailData.value;

        return 0;
    }

    public void Refresh()
    {
        TotalValue = 0;
        for (int i = 0; i < StatDetailDataList.Count; i++)
        {
            TotalValue += StatDetailDataList[i].value;
        }
    }

    public void Release()
    {
        StatDetailDataList.Clear();
        TotalValue = 0;
    }

}

[Serializable]
public class StatManagedData
{
    public Status Status = new();
    public Dictionary<StatType, StatListData> DicStatListDatas = new();

    public StatManagedData()
    {
        Status = new();
        DicStatListDatas = new();
    }

    public void SetStatDetailData(StatType statType, string key, double value)
    {
        StatListData statListData = GetStatListData(statType);
        if (statListData == null)
        {
            statListData = new StatListData();
            if (DicStatListDatas.ContainsKey(statType) == false)
                DicStatListDatas.Add(statType, statListData);
            else
               DicStatListDatas[statType] = statListData;
        }

        statListData.SetStat(key, value);
        Status.SetupStatusValue(statType, GetValue(statListData));
    }

    public void SetForceStatDetailData(StatType statType, double value)
    {
        Status.SetupStatusValue(statType, value);
    }

    public void RemoveStatDetailData(StatType statType, string key)
    {
        StatListData statListData = GetStatListData(statType);
        if (statListData != null)
        {
            statListData.RemoveStat(key);
            Status.SetupStatusValue(statType, GetValue(statListData));
        }
    }

    public double GetValue(StatListData data)
    {
        if (data != null)
            return data.TotalValue;

        return 0;
    }
    public double GetValue(StatType statType)
    {
        StatListData data = GetStatListData(statType);
        if (data != null)
            return data.TotalValue;

        return 0;
    }

    public StatListData GetStatListData(StatType statType)
    {
        if (DicStatListDatas.ContainsKey(statType))
            return DicStatListDatas[statType];

        return null;
    }

    internal void RemoveStatDetailData(object statType, string key)
    {
        throw new NotImplementedException();
    }

    public void Release(StatType statType)
    {
        StatListData statListData = GetStatListData(statType);
        if (statListData != null)
        {
            statListData.Release();
            double statValue = GetValue(statListData);
            Status.SetupStatusValue(statType, statValue);

            if (statValue <= 0 && DicStatListDatas.ContainsKey(statType))
                DicStatListDatas.Remove(statType);
        }
    }
    public void Release()
    {
        DicStatListDatas.Clear();
    }
}

