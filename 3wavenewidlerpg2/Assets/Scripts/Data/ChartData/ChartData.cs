using System.Collections;
using System.Collections.Generic;
using System;
using GameUtils;
using UnityEngine;
using LitJson;


[Serializable]
public class ChartDataBase
{
    virtual public void SetData(JsonData jsonData)
    {

    }
}


[Serializable]
public class SerializableChartData
{
    public bool IsSetChartData = false;
    virtual public void SetChartData()
    {
        IsSetChartData = true;
    }
}