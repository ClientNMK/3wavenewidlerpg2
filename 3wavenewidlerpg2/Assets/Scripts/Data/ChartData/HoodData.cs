using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;

// 후드 레벨 차트. 레벨 구간(Start_Level 이상 ~ End_Level 미만)마다 한 줄, Resorce = 후드 Spine 스킨 이름
public class HoodData : ChartDataBase
{
    public int Data_Id;
    public int Start_Level;
    public int End_Level;
    public int Upgrade_Goods_Id;
    public double Upgrade_Goods_Value;
    public double Upgrade_Goods_Increase_Value;
    public int Upgrade_Stat_Id;
    public double Upgrade_Stat_Value;
    public double Upgrade_Stat_Increase_Value;
    public string Resorce;

    public override void SetData(JsonData jsonData)
    {
        base.SetData(jsonData);
        this.Data_Id = GameUtils.GameUtils.TryParse<int>("Data_Id", jsonData);
        this.Start_Level = GameUtils.GameUtils.TryParse<int>("Start_Level", jsonData);
        this.End_Level = GameUtils.GameUtils.TryParse<int>("End_Level", jsonData);
        this.Upgrade_Goods_Id = GameUtils.GameUtils.TryParse<int>("Upgrade_Goods_Id", jsonData);
        this.Upgrade_Goods_Value = GameUtils.GameUtils.TryParse<double>("Upgrade_Goods_Value", jsonData);
        this.Upgrade_Goods_Increase_Value = GameUtils.GameUtils.TryParse<double>("Upgrade_Goods_Increase_Value", jsonData);
        this.Upgrade_Stat_Id = GameUtils.GameUtils.TryParse<int>("Upgrade_Stat_Id", jsonData);
        this.Upgrade_Stat_Value = GameUtils.GameUtils.TryParse<double>("Upgrade_Stat_Value", jsonData);
        this.Upgrade_Stat_Increase_Value = GameUtils.GameUtils.TryParse<double>("Upgrade_Stat_Increase_Value", jsonData);
        this.Resorce = GameUtils.GameUtils.TryParse<string>("Resorce", jsonData);
    }

    // 이 레벨에서 다음 레벨까지 먹일 재화
    public double GetCost(int _level)
    {
        return Upgrade_Goods_Value + Upgrade_Goods_Increase_Value * (_level - Start_Level);
    }

    // 이 레벨의 스탯 보너스 (기본값에 더함)
    public double GetStatValue(int _level)
    {
        return Upgrade_Stat_Value + Upgrade_Stat_Increase_Value * (_level - Start_Level);
    }
}

public class SerializableHoodData : SerializableChartData
{
    private List<HoodData> datas = new();
    public override void SetChartData()
    {
        JsonData jsonData = ChartManager.Instance.GetChartJsonData("HoodChart");
        if (jsonData != null)
        {
            datas.Clear();
            for (int i = 0; i < jsonData.Count; i++)
            {
                HoodData data = new HoodData();
                data.SetData(jsonData[i]);
                datas.Add(data);
            }
        }
        IsSetChartData = true;
    }

    public int MaxLevel
    {
        get
        {
            if (!IsSetChartData) SetChartData();
            return datas[datas.Count - 1].End_Level;
        }
    }

    // 레벨이 속한 구간. 최대 레벨이면 마지막 구간
    public HoodData GetHoodData(int _level)
    {
        if (!IsSetChartData) SetChartData();

        return datas.Find(x => x.Start_Level <= _level && _level < x.End_Level) ?? datas[datas.Count - 1];
    }
}
