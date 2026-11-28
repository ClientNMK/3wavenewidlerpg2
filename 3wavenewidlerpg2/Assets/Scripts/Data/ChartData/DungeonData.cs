using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using LitJson;

// 던전 차트 중 현재 사용하는 컬럼만 읽음 (Dungeon_Type 1000번대 = 던전, 2000번대 = 도전)
[Serializable]
public class DungeonData : ChartDataBase
{
    public int Dungeon_Type;
    public int Step;
    public int Monster_Id;
    public double Monster_Hp;
    public double Monster_Attack;
    public double Monster_Penetrate;
    public float Limit_Time;
    public int Reward_Item_Id;
    public double Reward_Item_Value;
    public int FirstClear_Reward_Id;
    public double FirstClear_Reward_Value;
    public int Recommended_Stage;

    public override void SetData(JsonData jsonData)
    {
        base.SetData(jsonData);
        this.Dungeon_Type = GameUtils.GameUtils.TryParse<int>("Dungeon_Type", jsonData);
        this.Step = GameUtils.GameUtils.TryParse<int>("Step", jsonData);
        this.Monster_Id = GameUtils.GameUtils.TryParse<int>("Monster_Id", jsonData);
        this.Monster_Hp = GameUtils.GameUtils.TryParse<double>("Monster_Hp", jsonData);
        this.Monster_Attack = GameUtils.GameUtils.TryParse<double>("Monster_Attack", jsonData);
        this.Monster_Penetrate = GameUtils.GameUtils.TryParse<double>("Monster_Penetrate", jsonData);
        this.Limit_Time = GameUtils.GameUtils.TryParse<float>("Limit_Time", jsonData);
        this.Reward_Item_Id = GameUtils.GameUtils.TryParse<int>("Reward_Item_Id", jsonData);
        this.Reward_Item_Value = GameUtils.GameUtils.TryParse<double>("Reward_Item_Value", jsonData);
        this.FirstClear_Reward_Id = GameUtils.GameUtils.TryParse<int>("FirstClear_Reward_Id", jsonData);
        this.FirstClear_Reward_Value = GameUtils.GameUtils.TryParse<double>("FirstClear_Reward_Value", jsonData);
        this.Recommended_Stage = GameUtils.GameUtils.TryParse<int>("Recommended_Stage", jsonData);
    }
}

[Serializable]
public class SerializableDungeonData : SerializableChartData
{
    readonly List<List<DungeonData>> dungeons = new();     // Dungeon_Type별, 각 Step 순
    public override void SetChartData()
    {
        JsonData jsonData = ChartManager.Instance.GetChartJsonData("DungeonChart");
        if (jsonData != null)
        {
            dungeons.Clear();
            for (int i = 0; i < jsonData.Count; i++)
            {
                DungeonData data = new DungeonData();
                data.SetData(jsonData[i]);
                List<DungeonData> steps = dungeons.Find(x => x[0].Dungeon_Type == data.Dungeon_Type);
                if (steps == null)
                    dungeons.Add(steps = new());
                steps.Add(data);
            }
        }
        IsSetChartData = true;
    }

    // _isChallenge면 2000번대(도전), 아니면 1000번대(던전)
    public List<List<DungeonData>> GetDungeons(bool _isChallenge)
    {
        if (IsSetChartData == false)
            SetChartData();

        return dungeons.FindAll(x => x[0].Dungeon_Type / 1000 == (_isChallenge ? 2 : 1));
    }
}
