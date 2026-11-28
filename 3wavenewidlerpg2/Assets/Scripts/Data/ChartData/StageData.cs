using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using LitJson;

[Serializable]
public class StageData : ChartDataBase
{
    public int Stage_Id;
    public int World_Id;
    public int Stage_Number;
    public int[] Monster_Id;
    public double Monster_Hp;
    public double Monster_Attack;
    public double Monster_Penetrate;
    public int[] Monster_Spawn_Count;
    public int Boss_Monster_Id;
    public double Boss_Monster_Hp;
    public double Boss_Monster_Attack;
    public double Boss_Monster_Penetrate;
    public double Gold_Value;
    public double Exp_Value;
    public double Hood_Exp_Value;      // 일반 몬스터 포식 보상
    public double Boss_Hood_Exp_Value; // 스테이지 보스 마무리 포식 보상
    public int DropItem_Id;
    public double DropItem_Value;
    public float DropItem_Rate;
    public int First_Clear_Goods_Id;
    public double First_Clear_Value;

    public override void SetData(JsonData jsonData)
    {
        base.SetData(jsonData);
        this.Stage_Id = GameUtils.GameUtils.TryParse<int>("Stage_Id", jsonData);
        this.World_Id = GameUtils.GameUtils.TryParse<int>("World_Id", jsonData);
        this.Stage_Number = GameUtils.GameUtils.TryParse<int>("Stage_Number", jsonData);
        this.Monster_Id = ParseIntArray("Monster_Id", jsonData);
        this.Monster_Hp = GameUtils.GameUtils.TryParse<double>("Monster_Hp", jsonData);
        this.Monster_Attack = GameUtils.GameUtils.TryParse<double>("Monster_Attack", jsonData);
        this.Monster_Penetrate = GameUtils.GameUtils.TryParse<double>("Monster_Penetrate", jsonData);
        this.Monster_Spawn_Count = ParseIntArray("Monster_Spawn_Count", jsonData);

        this.Boss_Monster_Id = GameUtils.GameUtils.TryParse<int>("Boss_Monster_Id", jsonData);
        this.Boss_Monster_Hp = GameUtils.GameUtils.TryParse<double>("Boss_Monster_Hp", jsonData);
        this.Boss_Monster_Attack = GameUtils.GameUtils.TryParse<double>("Boss_Monster_Attack", jsonData);
        this.Boss_Monster_Penetrate = GameUtils.GameUtils.TryParse<double>("Boss_Monster_Penetrate", jsonData);
        this.Gold_Value = GameUtils.GameUtils.TryParse<double>("Gold_Value", jsonData);
        this.Exp_Value = GameUtils.GameUtils.TryParse<double>("Exp_Value", jsonData);
        this.Hood_Exp_Value = GameUtils.GameUtils.TryParse<double>("Hood_Exp_Value", jsonData);
        this.Boss_Hood_Exp_Value = GameUtils.GameUtils.TryParse<double>("Boss_Hood_Exp_Value", jsonData);
        this.DropItem_Id = GameUtils.GameUtils.TryParse<int>("DropItem_Id", jsonData);
        this.DropItem_Value = GameUtils.GameUtils.TryParse<double>("DropItem_Value", jsonData);
        this.DropItem_Rate = GameUtils.GameUtils.TryParse<float>("DropItem_Rate", jsonData);
        this.First_Clear_Goods_Id = GameUtils.GameUtils.TryParse<int>("First_Clear_Goods_Id", jsonData);
        this.First_Clear_Value = GameUtils.GameUtils.TryParse<double>("First_Clear_Value", jsonData);
    }

    // "1, 2, 3" 형식
    int[] ParseIntArray(string _key, JsonData jsonData)
    {
        string[] splits = GameUtils.GameUtils.TryParse<string>(_key, jsonData).Split(',');
        int[] values = new int[splits.Length];
        for (int i = 0; i < splits.Length; i++)
            values[i] = GameUtils.GameUtils.TryParse<int>(splits[i]);
        return values;
    }
}

[Serializable]
public class SerializableStageData : SerializableChartData
{
    Dictionary<int, StageData> datas = new();
    public override void SetChartData()
    {
        JsonData jsonData = ChartManager.Instance.GetChartJsonData("StageChart");
        if (jsonData != null)
        {
            datas.Clear();
            for (int i = 0; i < jsonData.Count; i++)
            {
                StageData stageData = new StageData();
                stageData.SetData(jsonData[i]);
                datas.Add(stageData.Stage_Id, stageData);
            }
        }
        IsSetChartData = true;
    }

    public bool HasStageData(int _stageId)
    {
        if (IsSetChartData == false)
            SetChartData();

        return datas.ContainsKey(_stageId);
    }

    public StageData GetStageData(int _stageId)
    {
        if (IsSetChartData == false)
            SetChartData();

        return datas[_stageId];
    }
}
