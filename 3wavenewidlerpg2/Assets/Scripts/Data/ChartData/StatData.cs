using System.Collections;
using System.Collections.Generic;
using System;
using GameUtils;
using UnityEngine;
using LitJson;

[Serializable]
public class StatData : ChartDataBase
{
    public int Stat_Id;
    public string TextCode_Name;
    public StatType Stat_Type;
    public double Stat_Default_Value;
    public StatValueType Value_Type;
    public int Sort;
    public string Key;
    public string Icon;

    public override void SetData(JsonData jsonData)
    {
        base.SetData(jsonData);
        this.Stat_Id = GameUtils.GameUtils.TryParse<int>("Stat_Id", jsonData);
        this.TextCode_Name = GameUtils.GameUtils.TryParse<string>("TextCode_Name", jsonData);
        // 차트에 Stat_Type 컬럼이 없어 Stat_Id를 StatType 번호로 사용
        this.Stat_Type = (StatType)this.Stat_Id;
        this.Stat_Default_Value = GameUtils.GameUtils.TryParse<double>("Stat_Default_Value", jsonData);

        string strValueType = GameUtils.GameUtils.TryParse<string>("Value_Type", jsonData);
        this.Value_Type = strValueType.Equals("Value") ? StatValueType.Value : StatValueType.Percent;
        this.Sort = GameUtils.GameUtils.TryParse<int>("Sort", jsonData);
        this.Key = GameUtils.GameUtils.TryParse<string>("Key", jsonData);
        this.Icon = GameUtils.GameUtils.TryParse<string>("Icon", jsonData);
    }
}

[Serializable]
public class SerializableStatData : SerializableChartData
{
    Dictionary<int, StatData> datas = new();
    public Dictionary<int, StatData> Datas
    {
        get
        {
            if (!IsSetChartData)
                SetChartData();
            return datas;
        }
    }
    public override void SetChartData()
    {
        JsonData jsonData = ChartManager.Instance.GetChartJsonData("StatChart");
        if (jsonData != null)
        {
            datas.Clear();
            for (int i = 0; i < jsonData.Count; i++)
            {
                StatData statData = new StatData();
                statData.SetData(jsonData[i]);
                datas.Add(statData.Stat_Id, statData);
            }
        }
        IsSetChartData = true;
    }

    public StatData GetStatData(StatType _statType)
    {
        int statId = (int)_statType;
        return GetStatData(statId);
    }
    public StatData GetStatData(int _statId)
    {
        if (IsSetChartData == false)
            SetChartData();

        if (datas.ContainsKey(_statId))
            return datas[_statId];

        return null;
    }
}
