using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using LitJson;

public enum MissionResetType
{
    Daily = 1,
    Weekly,
    Repeat,
}

public enum MissionProgressType
{
    Login = 1,
    PlayTime,           // 초
    MonsterKill,
    DungeonPlay,        // 컨텐츠 중 던전만
    WatchAd,
    DailyComplete,      // 일일 미션 N개 완료
}

// 미션 차트 중 현재 사용하는 컬럼만 읽음 (Mission_Type = 탭: 1 일일, 2 주간, 3 반복)
[Serializable]
public class MissionData : ChartDataBase
{
    public int Mission_Id;
    public string TextCode_Desc;
    public int Mission_Type;
    public MissionResetType Reset_Type;
    public MissionProgressType Progress_Type;
    public double Complete_Value;
    public int Reward_Id;
    public double Reward_Value;

    public override void SetData(JsonData jsonData)
    {
        base.SetData(jsonData);
        this.Mission_Id = GameUtils.GameUtils.TryParse<int>("Mission_Id", jsonData);
        this.TextCode_Desc = GameUtils.GameUtils.TryParse<string>("TextCode_Desc", jsonData);
        this.Mission_Type = GameUtils.GameUtils.TryParse<int>("Mission_Type", jsonData);
        this.Reset_Type = GameUtils.GameUtils.TryParse<MissionResetType>("Reset_Type", jsonData);
        this.Progress_Type = GameUtils.GameUtils.TryParse<MissionProgressType>("Progress_Type", jsonData);
        this.Complete_Value = GameUtils.GameUtils.TryParse<double>("Complete_Value", jsonData);
        this.Reward_Id = GameUtils.GameUtils.TryParse<int>("Reward_Id", jsonData);
        this.Reward_Value = GameUtils.GameUtils.TryParse<double>("Reward_Value", jsonData);
    }
}

[Serializable]
public class SerializableMissionData : SerializableChartData
{
    readonly List<MissionData> datas = new();     // 차트 순서
    public override void SetChartData()
    {
        JsonData jsonData = ChartManager.Instance.GetChartJsonData("MissionChart");
        if (jsonData != null)
        {
            datas.Clear();
            for (int i = 0; i < jsonData.Count; i++)
            {
                MissionData data = new MissionData();
                data.SetData(jsonData[i]);
                datas.Add(data);
            }
        }
        IsSetChartData = true;
    }

    public List<MissionData> GetMissions()
    {
        if (IsSetChartData == false)
            SetChartData();

        return datas;
    }
}
