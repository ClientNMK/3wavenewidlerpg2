using System.Collections;
using System.Collections.Generic;
using System;
using GameUtils;
using UnityEngine;
using LitJson;

[Serializable]
public class StringData : ChartDataBase
{
    public string String_Id;
    public string Kr;
    public string En;
    // public string Indonesia;
    public string Jp;
    public string Cn;

    public override void SetData(JsonData jsonData)
    {
        base.SetData(jsonData);
        this.String_Id = GameUtils.GameUtils.TryParse<string>("String_Id", jsonData);
        this.Kr = GameUtils.GameUtils.TryParse<string>("Kr", jsonData);
        if (string.IsNullOrEmpty(this.Kr) == false) this.Kr = this.Kr.Replace("\\n", "\n");

        this.En = GameUtils.GameUtils.TryParse<string>("En", jsonData);
        if (string.IsNullOrEmpty(this.En) == false) this.En = this.En.Replace("\\n", "\n");

         //this.Indonesia = CGameUtils.TryParse<string>("Indonesia", jsonData);

        this.Jp = GameUtils.GameUtils.TryParse<string>("Ja", jsonData);
        this.Jp = this.Jp.Replace("\\n", "\n");

        this.Cn = GameUtils.GameUtils.TryParse<string>("TW", jsonData);
        this.Cn = this.Cn.Replace("\\n", "\n");
    }
}


[Serializable]
public class SerializableStringData : SerializableChartData
{
    public Dictionary<string, StringData> datas = new();

    public bool IsSetLocalString = false;
    public bool IsSetChartString = false;
    public void SetLocalStringData()
    {
        datas.Clear();
        TextAsset csvFile = Resources.Load<TextAsset>("LocalString/LocalStringChart");
        if (csvFile != null && string.IsNullOrEmpty(csvFile.text) == false)
        {
            var lines = csvFile.text.Split('\n');
            foreach (var line in lines)
            {
                string[] values = GameUtils.GameUtils.SplitCSVFile(line);
                if (values.Length < 5) continue;
                if (string.IsNullOrEmpty(values[0])) continue;

                datas.Add(values[0], new StringData
                {
                    String_Id = values[0],
                    Kr = values[1].Replace("\\n", "\n"),
                    En = values[2].Replace("\\n", "\n"),
                    // Indonesia = values[3].Replace("||", "\n"),
                    Jp = values[3].Replace("\\n", "\n"),
                    Cn = values[4].Replace("\\n", "\n"),
                });
            }
        }
        IsSetLocalString = true;
    }
    public void SetStringData()
    {
        JsonData jsonData = ChartManager.Instance.GetChartJsonData("StringChart");
        if (jsonData != null)
        {
            datas.Clear();
            for (int i = 0; i < jsonData.Count; i++)
            {
                StringData stringData = new StringData();
                stringData.SetData(jsonData[i]);
                if (datas.ContainsKey(stringData.String_Id))
                    continue;

                datas.Add(stringData.String_Id, stringData);
            }
        }
        IsSetChartString = true;
    }

    public StringData GetStringData(string keyName)
    {
        if (datas.ContainsKey(keyName))
            return datas[keyName];
        return null;
    }
}