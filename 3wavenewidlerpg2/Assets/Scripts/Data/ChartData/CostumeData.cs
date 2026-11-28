using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using LitJson;

// 코스튬 차트 (10001~ 소녀, 11001~ 후드)
[Serializable]
public class CostumeData : ChartDataBase
{
    public int Data_Id;
    public int Grade;
    public string TextCode_Name;
    public string TextCode_Desc;
    public int Have_Stat_Id;        // 보유 효과: 갖고만 있어도
    public double Have_Stat_Value;
    public int Equip_Stat_Id;       // 착용 효과: 입었을 때만
    public double Equip_Stat_Value;
    public string Skin_Name;        // Spine 스킨 이름
    public int Costume_Point;

    public bool IsHood { get { return Data_Id / 1000 == 11; } }

    public override void SetData(JsonData jsonData)
    {
        base.SetData(jsonData);
        this.Data_Id = GameUtils.GameUtils.TryParse<int>("Data_Id", jsonData);
        this.Grade = GameUtils.GameUtils.TryParse<int>("Grade", jsonData);
        this.TextCode_Name = GameUtils.GameUtils.TryParse<string>("TextCode_Name", jsonData);
        this.TextCode_Desc = GameUtils.GameUtils.TryParse<string>("TextCode_Desc", jsonData);
        this.Have_Stat_Id = GameUtils.GameUtils.TryParse<int>("Have_Stat_Id", jsonData);
        this.Have_Stat_Value = GameUtils.GameUtils.TryParse<double>("Have_Stat_Value", jsonData);
        this.Equip_Stat_Id = GameUtils.GameUtils.TryParse<int>("Equip_Stat_Id", jsonData);
        this.Equip_Stat_Value = GameUtils.GameUtils.TryParse<double>("Equip_Stat_Value", jsonData);
        this.Skin_Name = GameUtils.GameUtils.TryParse<string>("Skin_Name", jsonData);
        this.Costume_Point = GameUtils.GameUtils.TryParse<int>("Costume_Point", jsonData);
    }
}

// 코스튬 포인트 단계: Point(누적)에 닿으면 받기, 받은 단계 Stat_Value는 모두 더함
[Serializable]
public class CostumePointData : ChartDataBase
{
    public int Level;
    public int Stat_Id;
    public double Stat_Value;
    public int Point;

    public override void SetData(JsonData jsonData)
    {
        base.SetData(jsonData);
        this.Level = GameUtils.GameUtils.TryParse<int>("Level", jsonData);
        this.Stat_Id = GameUtils.GameUtils.TryParse<int>("Stat_Id", jsonData);
        this.Stat_Value = GameUtils.GameUtils.TryParse<double>("Stat_Value", jsonData);
        this.Point = GameUtils.GameUtils.TryParse<int>("Point", jsonData);
    }
}

[Serializable]
public class SerializableCostumeData : SerializableChartData
{
    readonly List<CostumeData> costumes = new();
    readonly List<CostumePointData> points = new();
    public override void SetChartData()
    {
        JsonData jsonData = ChartManager.Instance.GetChartJsonData("CostumeChart");
        if (jsonData != null)
        {
            costumes.Clear();
            for (int i = 0; i < jsonData.Count; i++)
            {
                CostumeData data = new CostumeData();
                data.SetData(jsonData[i]);
                costumes.Add(data);
            }
        }
        jsonData = ChartManager.Instance.GetChartJsonData("CostumePointChart");
        if (jsonData != null)
        {
            points.Clear();
            for (int i = 0; i < jsonData.Count; i++)
            {
                CostumePointData data = new CostumePointData();
                data.SetData(jsonData[i]);
                // 차트 아래 빈 줄은 Level이 0으로 읽힘
                if (data.Level > 0)
                    points.Add(data);
            }
        }
        IsSetChartData = true;
    }

    public List<CostumeData> GetCostumes()
    {
        if (IsSetChartData == false)
            SetChartData();

        return costumes;
    }

    public List<CostumePointData> GetPoints()
    {
        if (IsSetChartData == false)
            SetChartData();

        return points;
    }
}
