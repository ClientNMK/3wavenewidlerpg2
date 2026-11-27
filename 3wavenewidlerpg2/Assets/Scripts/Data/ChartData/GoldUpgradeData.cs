using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LitJson;
using UnityEngine;

// 골드 강화 차트. 레벨 구간(Level_Start ~ Level_End)마다 한 줄
public class GoldUpgradeData : ChartDataBase
{
    public int Gold_Id;
    public int Step;
    public int Level_Start;
    public int Level_End;
    public int Upgrade_Stat_Id;
    public double Upgrade_Stat_Value;
    public double Upgrade_Stat_Increase_Value;
    public int Upgrade_Goods_Id;
    public double Upgrade_Goods_Value;
    public double Upgrade_Goods_Increase_Value;

    public override void SetData(JsonData jsonData)
    {
        base.SetData(jsonData);
        this.Gold_Id = GameUtils.GameUtils.TryParse<int>("Gold_Id", jsonData);
        this.Step = GameUtils.GameUtils.TryParse<int>("Step", jsonData);
        this.Level_Start = GameUtils.GameUtils.TryParse<int>("Level_Start", jsonData);
        this.Level_End = GameUtils.GameUtils.TryParse<int>("Level_End", jsonData);
        this.Upgrade_Stat_Id = GameUtils.GameUtils.TryParse<int>("Upgrade_Stat_Id", jsonData);
        this.Upgrade_Stat_Value = GameUtils.GameUtils.TryParse<double>("Upgrade_Stat_Value", jsonData);
        this.Upgrade_Stat_Increase_Value = GameUtils.GameUtils.TryParse<double>("Upgrade_Stat_Increase_Value", jsonData);
        this.Upgrade_Goods_Id = GameUtils.GameUtils.TryParse<int>("Upgrade_Goods_Id", jsonData);
        this.Upgrade_Goods_Value = GameUtils.GameUtils.TryParse<double>("Upgrade_Goods_Value", jsonData);
        this.Upgrade_Goods_Increase_Value = GameUtils.GameUtils.TryParse<double>("Upgrade_Goods_Increase_Value", jsonData);
    }
}

public class SerializableGoldUpgradeData : SerializableChartData
{
    private List<GoldUpgradeData> datas = new();
    public override void SetChartData()
    {
        JsonData jsonData = ChartManager.Instance.GetChartJsonData("GoldUpgradeChart");
        if (jsonData != null)
        {
            datas.Clear();
            for (int i = 0; i < jsonData.Count; i++)
            {
                GoldUpgradeData data = new GoldUpgradeData();
                data.SetData(jsonData[i]);
                datas.Add(data);
            }
        }
        IsSetChartData = true;
    }

    public List<int> GetGoldIds()
    {
        if (!IsSetChartData) SetChartData();

        return datas.Select(x => x.Gold_Id).Distinct().ToList();
    }

    // 레벨이 속한 구간. 마지막 구간을 넘으면 마지막 구간
    public GoldUpgradeData GetGoldUpgradeData(int _goldId, int _level)
    {
        if (!IsSetChartData) SetChartData();

        var data = datas.Find(x => x.Gold_Id == _goldId && x.Level_Start <= _level && x.Level_End > _level);
        return data ?? datas.FindLast(x => x.Gold_Id == _goldId);
    }
}
