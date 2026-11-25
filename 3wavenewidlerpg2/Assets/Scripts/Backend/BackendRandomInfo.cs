using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using BackEnd;

public class RandomInfoTable
{
    public string title; // 랜덤 조회 제목
    public string uuid; // 랜덤 조회 uuid
    public RandomType randomtype; //랜덤 조회 유형

    public override string ToString()
    {
        return $"title: {title}\n" +
        $"uuid: {uuid}\n" +
        $"randomtype: {randomtype}";
    }
}
public class RandomInformation
{

    public string inDate; // inDate
    public int score; // 점수
    public string guildName; //(길드만 해당)길드이름

    public override string ToString()
    {
        string str = $"inDate: {inDate}\n" +
        $"score: {score}";
        if (!string.IsNullOrEmpty(guildName))
        {
            str += $"\nguildName : {guildName}";
        }
        return str;
    }
}

public class BackendRandomInfo : MonoBehaviour
{
    public void GetRandomDataTableList(Action<BackendReturnObject, List<RandomInfoTable>> _onComplete = null)
    {
        Backend.RandomInfo.GetRandomDataTableList((callback) =>
        {
            List<RandomInfoTable> list = null; 
            // 이후 처리
            if (callback.IsSuccess())
            {
                list = new List<RandomInfoTable>();
                LitJson.JsonData json = callback.Rows();
                for (int i = 0; i < json.Count; i++)
                {
                    RandomInfoTable table = new RandomInfoTable();
                    table.title = json[i]["title"].ToString();
                    table.uuid = json[i]["uuid"].ToString();
                    if (json[i]["randomType"].ToString() == "user")
                    {
                        table.randomtype = RandomType.User;
                    }
                    else
                    {
                        table.randomtype = RandomType.Guild;
                    }
                    list.Add(table);
                }

                foreach (var table in list)
                {
                    Debug.Log(list.ToString());
                }
            }

            if (_onComplete != null)
                _onComplete(callback, list);
        });
    }

    public void SetRandomData(RandomType randomType, string uuid, int value, Action<BackendReturnObject> _onComplete = null)
    {
        Backend.RandomInfo.SetRandomData(randomType, uuid, value, (callback) =>
        {
            // 이후 처리
            if (callback.IsSuccess())
            {
            }

            if (_onComplete != null)
                _onComplete(callback);
        });
    }

    public void GetRandomData(RandomType randomType, string uuid, int pivot, int gap, int count, Action<BackendReturnObject, List<RandomInformation>> _onComplete = null)
    {
        RandomType type = randomType;
        Backend.RandomInfo.GetRandomData(randomType, uuid, pivot, gap, count, (callback) =>
        {
            List<RandomInformation> list = null;
            // 이후 처리
            if (callback.IsSuccess())
            {
                list = new List<RandomInformation>();
                LitJson.JsonData json = callback.Rows();
                for (int i = 0; i < json.Count; i++)
                {

                    RandomInformation randomInformation = new RandomInformation();
                    if (type == RandomType.User)
                    {
                        randomInformation.inDate = json[i]["gamerInDate"].ToString();
                    }
                    else
                    {
                        randomInformation.guildName = json[i]["guildName"].ToString();
                        randomInformation.inDate = json[i]["guildInDate"].ToString();
                    }

                    randomInformation.score = int.Parse(json[i]["score"].ToString());

                    list.Add(randomInformation);
                }
                foreach (var info in list)
                {
                    Debug.Log(info.ToString());
                }
            }

            if (_onComplete != null)
                _onComplete(callback, list);
        });
    }
}
