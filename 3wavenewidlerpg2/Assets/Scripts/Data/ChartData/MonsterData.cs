using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using LitJson;

public enum MonsterType
{
    NormalMonster = 1,
    BossMonster,
}

public enum MonsterDetectionType
{
    DetectTracking = 1,     // 영역내에 들어오면 쫓아간다.
    DetectRunAway,          // 영역내에 들어오면 도망간다.
}

[Serializable]
public class MonsterData : ChartDataBase
{
    public int Data_Id;
    public string TextCode_Name;
    public string Resource;
    public string Skin_Name;
    public MonsterType Monster_Type;
    public float Monster_Scale;
    public float Monster_Move_Speed;
    public MonsterDetectionType Monster_Detection_Type;
    public float Monster_Detection_Range;
    public float Monster_Attack_Range;
    public float Monster_Attack_Speed;

    public override void SetData(JsonData jsonData)
    {
        base.SetData(jsonData);
        this.Data_Id = GameUtils.GameUtils.TryParse<int>("Data_Id", jsonData);
        this.TextCode_Name = GameUtils.GameUtils.TryParse<string>("TextCode_Name", jsonData);
        this.Resource = GameUtils.GameUtils.TryParse<string>("Resource", jsonData);
        this.Skin_Name = GameUtils.GameUtils.TryParse<string>("Skin_Name", jsonData);
        this.Monster_Type = GameUtils.GameUtils.TryParse<MonsterType>("Monster_Type", jsonData);
        this.Monster_Scale = GameUtils.GameUtils.TryParse<float>("Monster_Scale", jsonData);
        this.Monster_Move_Speed = GameUtils.GameUtils.TryParse<float>("Monster_Move_Speed", jsonData);
        this.Monster_Detection_Type = GameUtils.GameUtils.TryParse<MonsterDetectionType>("Monster_Detection_Type", jsonData);
        this.Monster_Detection_Range = GameUtils.GameUtils.TryParse<float>("Monster_Detection_Range", jsonData);
        this.Monster_Attack_Range = GameUtils.GameUtils.TryParse<float>("Monster_Attack_Range", jsonData);
        this.Monster_Attack_Speed = GameUtils.GameUtils.TryParse<float>("Monster_Attack_Speed", jsonData);
    }
}

[Serializable]
public class SerializableMonsterData : SerializableChartData
{
    Dictionary<int, MonsterData> datas = new();
    public override void SetChartData()
    {
        JsonData jsonData = ChartManager.Instance.GetChartJsonData("MonsterChart");
        if (jsonData != null)
        {
            datas.Clear();
            for (int i = 0; i < jsonData.Count; i++)
            {
                MonsterData monsterData = new MonsterData();
                monsterData.SetData(jsonData[i]);
                datas.Add(monsterData.Data_Id, monsterData);
            }
        }
        IsSetChartData = true;
    }

    public MonsterData GetMonsterData(int _dataId)
    {
        if (IsSetChartData == false)
            SetChartData();

        return datas[_dataId];
    }
}
