using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;

// 캐릭터 레벨 차트. 레벨 구간(Level_Start 이상 ~ Level_End 미만)마다 한 줄
public class CharLevelData : ChartDataBase
{
    public int Level_Id;
    public int Level_Start;
    public int Level_End;
    public int LevelUp_Goods_Id;
    public double LevelUp_Goods_Value;
    public double LevelUp_Goods_Increase_Value;
    public int LevelUp_Reward_Goods_Id;
    public double LevelUp_Reward_Goods_Value;

    public override void SetData(JsonData jsonData)
    {
        base.SetData(jsonData);
        this.Level_Id = GameUtils.GameUtils.TryParse<int>("Level_Id", jsonData);
        this.Level_Start = GameUtils.GameUtils.TryParse<int>("Level_Start", jsonData);
        this.Level_End = GameUtils.GameUtils.TryParse<int>("Level_End", jsonData);
        this.LevelUp_Goods_Id = GameUtils.GameUtils.TryParse<int>("LevelUp_Goods_Id", jsonData);
        this.LevelUp_Goods_Value = GameUtils.GameUtils.TryParse<double>("LevelUp_Goods_Value", jsonData);
        this.LevelUp_Goods_Increase_Value = GameUtils.GameUtils.TryParse<double>("LevelUp_Goods_Increase_Value", jsonData);
        this.LevelUp_Reward_Goods_Id = GameUtils.GameUtils.TryParse<int>("LevelUp_Reward_Goods_Id", jsonData);
        this.LevelUp_Reward_Goods_Value = GameUtils.GameUtils.TryParse<double>("LevelUp_Reward_Goods_Value", jsonData);
    }

    // 이 레벨에서 다음 레벨까지 필요한 경험치
    public double GetNeedExp(int _level)
    {
        return LevelUp_Goods_Value + LevelUp_Goods_Increase_Value * (_level - Level_Start);
    }
}

public class SerializableCharLevelData : SerializableChartData
{
    private List<CharLevelData> datas = new();
    public override void SetChartData()
    {
        JsonData jsonData = ChartManager.Instance.GetChartJsonData("CharLevelChart");
        if (jsonData != null)
        {
            datas.Clear();
            for (int i = 0; i < jsonData.Count; i++)
            {
                CharLevelData data = new CharLevelData();
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
            return datas[datas.Count - 1].Level_End;
        }
    }

    public CharLevelData GetCharLevelData(int _level)
    {
        if (!IsSetChartData) SetChartData();

        return datas.Find(x => x.Level_Start <= _level && _level < x.Level_End);
    }
}
