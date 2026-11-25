using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using LitJson;

// 스킬 차트 중 현재 사용하는 컬럼만 읽음
[Serializable]
public class SkillData : ChartDataBase
{
    public int Data_Id;
    public string TextCode_Name;
    public string TextCode_Desc;
    public int Skill_Type;
    public float Cooltime;
    public int Max_Hits;
    public double Damage;
    public string Resource;
    public string Icon;

    public override void SetData(JsonData jsonData)
    {
        base.SetData(jsonData);
        this.Data_Id = GameUtils.GameUtils.TryParse<int>("Data_Id", jsonData);
        this.TextCode_Name = GameUtils.GameUtils.TryParse<string>("TextCode_Name", jsonData);
        this.TextCode_Desc = GameUtils.GameUtils.TryParse<string>("TextCode_Desc", jsonData);
        this.Skill_Type = GameUtils.GameUtils.TryParse<int>("Skill_Type", jsonData);
        this.Cooltime = GameUtils.GameUtils.TryParse<float>("Cooltime", jsonData);
        this.Max_Hits = GameUtils.GameUtils.TryParse<int>("Max_Hits", jsonData);
        this.Damage = GameUtils.GameUtils.TryParse<double>("Damage", jsonData);
        this.Resource = GameUtils.GameUtils.TryParse<string>("Resource", jsonData);
        this.Icon = GameUtils.GameUtils.TryParse<string>("Icon", jsonData);
    }
}

[Serializable]
public class SerializableSkillData : SerializableChartData
{
    Dictionary<int, SkillData> datas = new();
    public override void SetChartData()
    {
        JsonData jsonData = ChartManager.Instance.GetChartJsonData("SkillChart");
        if (jsonData != null)
        {
            datas.Clear();
            for (int i = 0; i < jsonData.Count; i++)
            {
                SkillData skillData = new SkillData();
                skillData.SetData(jsonData[i]);
                datas.Add(skillData.Data_Id, skillData);
            }
        }
        IsSetChartData = true;
    }

    public SkillData GetSkillData(int _dataId)
    {
        if (IsSetChartData == false)
            SetChartData();

        return datas[_dataId];
    }
}
