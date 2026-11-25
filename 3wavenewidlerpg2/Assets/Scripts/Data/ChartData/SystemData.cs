using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using LitJson;

public class SystemData : ChartDataBase
{
    public string System_Id;
    public string System_Values;
    public override void SetData(JsonData jsonData)
    {
        base.SetData(jsonData);
        this.System_Id = GameUtils.GameUtils.TryParse<string>("System_Id", jsonData);
        this.System_Values = GameUtils.GameUtils.TryParse<string>("System_Values", jsonData);
    }
}

[Serializable]
public class SerializableSystemData : SerializableChartData
{
    public List<SystemData> datas = new();
    public override void SetChartData()
    {
        datas.Clear();
        JsonData jsonData = ChartManager.Instance.GetChartJsonData("SystemChart");
        if (jsonData != null)
        {
            for (int i = 0; i < jsonData.Count; i++)
            {
                SystemData data = new SystemData();
                data.SetData(jsonData[i]);
                datas.Add(data);
            }
        }
        IsSetChartData = true;
    }

    public float GetFloat(string _systemId)
    {
        if (IsSetChartData == false)
            SetChartData();

        return GameUtils.GameUtils.TryParse<float>(datas.Find(x => x.System_Id.Equals(_systemId)).System_Values);
    }
}
