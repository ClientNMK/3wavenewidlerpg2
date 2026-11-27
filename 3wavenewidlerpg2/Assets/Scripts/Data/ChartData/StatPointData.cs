using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LitJson;
using UnityEngine;

// 스탯 포인트(SP) 차트. StatPoint_Id별 레벨 구간마다 한 줄
public class StatPointData : ChartDataBase
{
    public int StatPoint_Id;
    public int Step;
    public int Upgrade_Stat_Id;
    public double Upgrade_Stat_Value;
    public double Upgrade_Stat_Increase_Value;
    public int Upgrade_Goods_Id;
    public double Upgrade_Goods_Value;
    public int Level_Start;
    public int Level_End;
    public int Open_CharLevel;
    public int Max_Level;

    public override void SetData(JsonData jsonData)
    {
        base.SetData(jsonData);
        this.StatPoint_Id = GameUtils.GameUtils.TryParse<int>("StatPoint_Id", jsonData);
        this.Step = GameUtils.GameUtils.TryParse<int>("Step", jsonData);
        this.Upgrade_Stat_Id = GameUtils.GameUtils.TryParse<int>("Upgrade_Stat_Id", jsonData);
        this.Upgrade_Stat_Value = GameUtils.GameUtils.TryParse<double>("Upgrade_Stat_Value", jsonData);
        this.Upgrade_Stat_Increase_Value = GameUtils.GameUtils.TryParse<double>("Upgrade_Stat_Increase_Value", jsonData);
        this.Upgrade_Goods_Id = GameUtils.GameUtils.TryParse<int>("Upgrade_Goods_Id", jsonData);
        this.Upgrade_Goods_Value = GameUtils.GameUtils.TryParse<double>("Upgrade_Goods_Value", jsonData);
        this.Level_Start = GameUtils.GameUtils.TryParse<int>("Level_Start", jsonData);
        this.Level_End = GameUtils.GameUtils.TryParse<int>("Level_End", jsonData);
        this.Open_CharLevel = GameUtils.GameUtils.TryParse<int>("Open_CharLevel", jsonData);
        this.Max_Level = GameUtils.GameUtils.TryParse<int>("Max_Level", jsonData);
    }
}

public class SerializableStatPointData : SerializableChartData
{
    private List<StatPointData> datas = new();
    public override void SetChartData()
    {
        JsonData jsonData = ChartManager.Instance.GetChartJsonData("StatPointChart");
        if (jsonData != null)
        {
            datas.Clear();
            for (int i = 0; i < jsonData.Count; i++)
            {
                StatPointData data = new StatPointData();
                data.SetData(jsonData[i]);
                datas.Add(data);
            }
        }
        IsSetChartData = true;
    }

    public List<int> GetStatPointIds()
    {
        if (!IsSetChartData) SetChartData();

        return datas.Select(x => x.StatPoint_Id).Distinct().ToList();
    }

    // 레벨이 속한 구간. 마지막 구간을 넘으면 마지막 구간
    public StatPointData GetStatPointData(int _statPointId, int _level)
    {
        if (!IsSetChartData) SetChartData();

        var data = datas.Find(x => x.StatPoint_Id == _statPointId && x.Level_Start <= _level && x.Level_End > _level);
        return data ?? datas.FindLast(x => x.StatPoint_Id == _statPointId);
    }
}
