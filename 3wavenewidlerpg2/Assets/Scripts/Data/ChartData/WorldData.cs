using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using LitJson;

// 월드 차트 중 현재 사용하는 컬럼만 읽음
[Serializable]
public class WorldData : ChartDataBase
{
    public int World_Id;
    public string World_Resource;

    public override void SetData(JsonData jsonData)
    {
        base.SetData(jsonData);
        this.World_Id = GameUtils.GameUtils.TryParse<int>("World_Id", jsonData);
        this.World_Resource = GameUtils.GameUtils.TryParse<string>("World_Resource", jsonData);
    }
}

[Serializable]
public class SerializableWorldData : SerializableChartData
{
    Dictionary<int, WorldData> datas = new();
    public override void SetChartData()
    {
        JsonData jsonData = ChartManager.Instance.GetChartJsonData("WorldChart");
        if (jsonData != null)
        {
            datas.Clear();
            for (int i = 0; i < jsonData.Count; i++)
            {
                WorldData worldData = new WorldData();
                worldData.SetData(jsonData[i]);
                datas.Add(worldData.World_Id, worldData);
            }
        }
        IsSetChartData = true;
    }

    public WorldData GetWorldData(int _worldId)
    {
        if (IsSetChartData == false)
            SetChartData();

        return datas[_worldId];
    }
}
