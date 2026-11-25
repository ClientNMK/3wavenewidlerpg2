using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using BackEnd;
using BackEnd.GlobalSupport;
using System.Linq;

public class GuildItem
{
    public int memberCount;
    public Dictionary<string, string> viceMasterList = new Dictionary<string, string>();
    public string masterNickname;
    public string inDate;
    public string guildName;
    public int goodsCount;
    public bool _immediateRegistration;
    public string _countryCode;
    public string masterInDate;

    public int level;
    public int mark;
    public int markBg;
    public string intro;
    public string notice;
    public long createAt;
    public long writeAt;
    public bool isLive;
    public int server;
    public long expelcooltime;
    public int condition;

    public override string ToString()
    {
        string viceMasterString = string.Empty;
        foreach (var li in viceMasterList)
        {
            viceMasterString += $"부 길드 마스터 : {li.Value}({li.Key})\n";
        }
        return $"memberCount : {memberCount}\n" +
        $"masterNickname : {masterNickname}\n" +
        $"inDate : {inDate}\n" +
        $"guildName : {guildName}\n" +
        $"goodsCount : {goodsCount}\n" +
        $"_immediateRegistration : {_immediateRegistration}\n" +
        $"_countryCode : {_countryCode}\n" +
        $"masterInDate : {masterInDate}\n" +
        $"memberCount : {memberCount}\n" +
        viceMasterString;
    }
};

public class GuildGoodsItem
{
    public string key = string.Empty;
    public goodsType goodsType = BackEnd.goodsType.goods1;
    public int totalGoodsAmount;
    public List<GoodsUserItem> userList = new List<GoodsUserItem>();
    public override string ToString()
    {
        string userString = string.Empty;
        for (int i = 0; i < userList.Count; i++)
        {
            userString += userList[i].ToString() + "\n";
        }
        return $"[totalGoodsAmount : {totalGoodsAmount}]\n" +
        $"{userString}\n";
    }
}
public class GoodsUserItem
{
    public int usingTotalAmount;
    public int totalAmount;
    public string inDate;
    public string nickname;
    public string updatedAt;
    public override string ToString()
    {
        return $"\tnickname : {nickname}\n" +
        $"\tinDate : {inDate}\n" +
        $"\ttotalAmount : {totalAmount}\n" +
        $"\tusingTotalAmount : {usingTotalAmount}\n" +
        $"\tupdatedAt : {updatedAt}\n";
    }
}

public enum GuildPosition
{
    Master = 1,
    ViceMaster,
    Member
}

public class GuildMemberItem
{
    public string nickname;
    public string inDate;
    public string gamerInDate;
    public string position;
    public GuildPosition guildPosition;
    public string lastLogin;
    public Dictionary<string, int> totalGoodsAmount = new Dictionary<string, int>();
    public override string ToString()
    {
        string goodsString = string.Empty;
        foreach (var dic in totalGoodsAmount)
        {
            goodsString += $"{dic.Key} : {dic.Value}\n";
        }
        return $"nickname : {nickname}\n" +
        $"inDate : {inDate}\n" +
        $"gamerInDate : {gamerInDate}\n" +
        $"position : {position}\n" +
        $"lastLogin : {lastLogin}\n" +
        goodsString;
    }
}

public class ApplicantsItem
{
    public string inDate;
    public string nickname;
    public override string ToString()
    {
        return $"nickname : {nickname}\ninDate : {inDate}\n";
    }
}

public class BackendGuild : MonoBehaviour
{
    public void GetMyGuildInfoV3(Action<BackendReturnObject, GuildItem> _onComplete = null)
    {
        SendQueue.Enqueue(Backend.Guild.GetMyGuildInfoV3, (callback) =>
        {
            GuildItem guildItem = null;
            // 이후 처리
            if (callback.IsSuccess())
            {
                LitJson.JsonData json = callback.GetFlattenJSON();
                LitJson.JsonData jsonGuild = json["guild"];

                guildItem = new GuildItem();
                guildItem.memberCount = GameUtils.GameUtils.TryParse<int>("memberCount", jsonGuild);
                guildItem.masterNickname = jsonGuild["masterNickname"].ToString();
                guildItem.inDate = jsonGuild["inDate"].ToString();
                guildItem.guildName = jsonGuild["guildName"].ToString();
                guildItem.goodsCount = GameUtils.GameUtils.TryParse<int>("goodsCount", jsonGuild);
                if (jsonGuild.ContainsKey("_immediateRegistration"))
                {
                    guildItem._immediateRegistration = jsonGuild["_immediateRegistration"].ToString() == "True" ? true : false;
                }
                if (jsonGuild.ContainsKey("_countryCode"))
                {
                    guildItem._countryCode = jsonGuild["_countryCode"].ToString();
                }
                guildItem.masterInDate = json["guild"]["masterInDate"].ToString();
                LitJson.JsonData viceListJson = json["guild"]["viceMasterList"];
                for (int j = 0; j < viceListJson.Count; j++)
                {
                    guildItem.viceMasterList.Add(viceListJson[j]["inDate"].ToString(), viceListJson[j]["nickname"].ToString());
                }
                if (jsonGuild.ContainsKey("level"))
                {
                    guildItem.level = GameUtils.GameUtils.TryParse<int>("level", jsonGuild);
                }
                if (jsonGuild.ContainsKey("mark"))
                {
                    guildItem.mark = GameUtils.GameUtils.TryParse<int>("mark", jsonGuild);
                }
                if (jsonGuild.ContainsKey("markBg"))
                {
                    guildItem.markBg = GameUtils.GameUtils.TryParse<int>("markBg", jsonGuild);
                }
                if (jsonGuild.ContainsKey("intro"))
                {
                    guildItem.intro = GameUtils.GameUtils.TryParse<string>("intro", jsonGuild);
                }
                if (jsonGuild.ContainsKey("notice"))
                {
                    guildItem.notice = GameUtils.GameUtils.TryParse<string>("notice", jsonGuild);
                }
                if (jsonGuild.ContainsKey("createAt"))
                {
                    guildItem.createAt = GameUtils.GameUtils.TryParse<long>("createAt", jsonGuild);
                }
                if (jsonGuild.ContainsKey("writeAt"))
                {
                    guildItem.writeAt = GameUtils.GameUtils.TryParse<long>("writeAt", jsonGuild);
                }
                if (jsonGuild.ContainsKey("isLive"))
                {
                    guildItem.isLive = GameUtils.GameUtils.TryParse<bool>("isLive", jsonGuild);
                }
                if (jsonGuild.ContainsKey("server"))
                {
                    guildItem.server = GameUtils.GameUtils.TryParse<int>("server", jsonGuild);
                }
                if (jsonGuild.ContainsKey("condition"))
                {
                    guildItem.condition = GameUtils.GameUtils.TryParse<int>("condition", jsonGuild);
                }
                Debug.Log(guildItem.ToString());
            }

            if (_onComplete != null)
                _onComplete(callback, guildItem);
        });
    }
    public void GetGuildIndateByGuildNameV3(string _guildName, Action<BackendReturnObject, string> _onComplete = null)
    {
        Backend.Guild.GetGuildIndateByGuildNameV3(_guildName, (callback) =>
        {
            string guildIndate = string.Empty;
            // 이후 처리
            if (callback.IsSuccess())
            {
                guildIndate = callback.GetReturnValuetoJSON()["guildInDate"]["S"].ToString();
            }

            if (_onComplete != null)
                _onComplete(callback, guildIndate);
        });
    }
    public void GetGuildInfoV3(string _guildIndate, Action<BackendReturnObject, GuildItem> _onComplete = null)
    {
        SendQueue.Enqueue(Backend.Guild.GetGuildInfoV3, _guildIndate, (callback) =>
        {
            GuildItem guildItem = null;
            // 이후 처리
            if (callback.IsSuccess())
            {
                LitJson.JsonData json = callback.GetFlattenJSON();
                LitJson.JsonData jsonGuild = json["guild"];

                guildItem = new GuildItem();
                guildItem.memberCount = GameUtils.GameUtils.TryParse<int>("memberCount", jsonGuild);
                guildItem.masterNickname = jsonGuild["masterNickname"].ToString();
                guildItem.inDate = jsonGuild["inDate"].ToString();
                guildItem.guildName = jsonGuild["guildName"].ToString();
                guildItem.goodsCount = GameUtils.GameUtils.TryParse<int>("goodsCount", jsonGuild);
                if (jsonGuild.ContainsKey("_immediateRegistration"))
                {
                    guildItem._immediateRegistration = jsonGuild["_immediateRegistration"].ToString() == "True" ? true : false;
                }
                if (jsonGuild.ContainsKey("_countryCode"))
                {
                    guildItem._countryCode = jsonGuild["_countryCode"].ToString();
                }
                guildItem.masterInDate = json["guild"]["masterInDate"].ToString();
                LitJson.JsonData viceListJson = json["guild"]["viceMasterList"];
                for (int j = 0; j < viceListJson.Count; j++)
                {
                    guildItem.viceMasterList.Add(viceListJson[j]["inDate"].ToString(), viceListJson[j]["nickname"].ToString());
                }

                if (jsonGuild.ContainsKey("level"))
                {
                    guildItem.level = GameUtils.GameUtils.TryParse<int>("level", jsonGuild);
                }
                if (jsonGuild.ContainsKey("mark"))
                {
                    guildItem.mark = GameUtils.GameUtils.TryParse<int>("mark", jsonGuild);
                }
                if (jsonGuild.ContainsKey("markBg"))
                {
                    guildItem.markBg = GameUtils.GameUtils.TryParse<int>("markBg", jsonGuild);
                }
                if (jsonGuild.ContainsKey("intro"))
                {
                    guildItem.intro = GameUtils.GameUtils.TryParse<string>("intro", jsonGuild);
                }
                if (jsonGuild.ContainsKey("notice"))
                {
                    guildItem.notice = GameUtils.GameUtils.TryParse<string>("notice", jsonGuild);
                }
                if (jsonGuild.ContainsKey("createAt"))
                {
                    guildItem.createAt = GameUtils.GameUtils.TryParse<long>("createAt", jsonGuild);
                }
                if (jsonGuild.ContainsKey("writeAt"))
                {
                    guildItem.writeAt = GameUtils.GameUtils.TryParse<long>("writeAt", jsonGuild);
                }
                if (jsonGuild.ContainsKey("isLive"))
                {
                    guildItem.isLive = GameUtils.GameUtils.TryParse<bool>("isLive", jsonGuild);
                }
                if (jsonGuild.ContainsKey("server"))
                {
                    guildItem.server = GameUtils.GameUtils.TryParse<int>("server", jsonGuild);
                }
                if (jsonGuild.ContainsKey("condition"))
                {
                    guildItem.condition = GameUtils.GameUtils.TryParse<int>("condition", jsonGuild);
                }
                Debug.Log(guildItem.ToString());
            }

            if (_onComplete != null)
                _onComplete(callback, guildItem);
        });
    }
    public void GetGuildMemberListV3(string _guildIndate, Action<BackendReturnObject, List<GuildMemberItem>> _onComplete = null)
    {
        SendQueue.Enqueue(Backend.Guild.GetGuildMemberListV3, _guildIndate, (callback) =>
        {
            List<GuildMemberItem> guildMemberList = null;

            // 이후 처리
            if (callback.IsSuccess())
            {
                LitJson.JsonData guildMemberJson = callback.FlattenRows();
                guildMemberList = new List<GuildMemberItem>();
                for (int i = 0; i < guildMemberJson.Count; i++)
                {
                    GuildMemberItem member = new GuildMemberItem();

                    member.nickname = guildMemberJson[i]["nickname"].ToString();
                    member.inDate = guildMemberJson[i]["inDate"].ToString();
                    member.gamerInDate = guildMemberJson[i]["gamerInDate"].ToString();
                    member.lastLogin = guildMemberJson[i]["lastLogin"].ToString();
                    member.position = guildMemberJson[i]["position"].ToString();
                    if (member.position.Equals("master")) member.guildPosition = GuildPosition.Master;
                    else if (member.position.Equals("viceMaster")) member.guildPosition = GuildPosition.ViceMaster;
                    else if (member.position.Equals("member")) member.guildPosition = GuildPosition.Member;

                    foreach (var goods in guildMemberJson[i].Keys)
                    {
                        if (goods.Contains("totalGoods"))
                        {
                            member.totalGoodsAmount.Add(goods, int.Parse(guildMemberJson[i][goods].ToString()));
                        }
                    }

                    guildMemberList.Add(member);
                    Debug.Log(member.ToString());
                }
                guildMemberList = guildMemberList
                .OrderBy(x => x.guildPosition)
                .OrderByDescending(x => x.lastLogin)
                .ToList();
            }

            if (_onComplete != null)
                _onComplete(callback, guildMemberList);
        });
    }

    public void CreateGuildV3(string _guildName, Param param, Action<BackendReturnObject> _onComplete = null)
    {
        int goodsCount = 10;
        SendQueue.Enqueue(Backend.Guild.CreateGuildV4, _guildName, goodsCount, param, (callback) =>
        {
            // 이후 처리
            if (callback.IsSuccess())
            {
            }

            if (_onComplete != null)
                _onComplete(callback);
        });
    }

    public void CreateGuildV3(string _guildName, int _mark, int _markBg, string _intro, int _condition, Action<BackendReturnObject> _onComplete = null)
    {
        // 메타 정보가 있는 경우
        Param param = new Param();
        param.Add("level", 1);
        param.Add("mark", _mark);
        param.Add("markBg", _markBg);
        param.Add("intro", _intro);
        param.Add("notice", string.Empty);
        param.Add("createAt", TimeManager.Instance.GetCurrentTimeStamp());
        param.Add("isLive", GameManager.Instance.IsDevelopBuild ? false : true);
        param.Add("condition", _condition);

        int goodsCount = 10;
        SendQueue.Enqueue(Backend.Guild.CreateGuildV4, _guildName, goodsCount, param, (callback) =>
        {
            // 이후 처리
            if (callback.IsSuccess())
            {
            }

            if (_onComplete != null)
                _onComplete(callback);
        });
    }

    public void ApplyGuildV3(string _guildIndate,  Action<BackendReturnObject> _onComplete = null)
    {
        SendQueue.Enqueue(Backend.Guild.ApplyGuildV3, _guildIndate, (callback) =>
        {
            // 이후 처리
            if (callback.IsSuccess())
            {
            }

            if (_onComplete != null)
                _onComplete(callback);
        });
    }

    public void WithdrawGuildV3(Action<BackendReturnObject> _onComplete = null)
    {
        SendQueue.Enqueue(Backend.Guild.WithdrawGuildV3, (callback) =>
        {
            // 이후 처리
            if (callback.IsSuccess())
            {
            }

            if (_onComplete != null)
                _onComplete(callback);
        });
    }

    public void SetRegistrationValueV3(bool _immediateFlag, Action<BackendReturnObject> _onComplete = null)
    {
        SendQueue.Enqueue(Backend.Guild.SetRegistrationValueV3, _immediateFlag, (callback) =>
        {
            // 이후 처리
            if (callback.IsSuccess())
            {
            }
            if (_onComplete != null)
                _onComplete(callback);
        });
    }

    public void ContributeGoodsV5(goodsType _goodsType, int _amount, Action<BackendReturnObject> _onComplete = null)
    {
        SendQueue.Enqueue(Backend.Guild.ContributeGoodsV5, _goodsType, _amount, (callback) =>
        {
            // 이후 처리
            if (callback.IsSuccess())
            {
            }

            if (_onComplete != null)
                _onComplete(callback);
        });
    }

    public void GetMyGuildGoodsV3(Action<BackendReturnObject, List<GuildGoodsItem>> _onComplete = null)
    {
        SendQueue.Enqueue(Backend.Guild.GetMyGuildGoodsV3, (callback) =>
        {
            List<GuildGoodsItem> guildGoodsItems = null;
            // 이후 처리
            if (callback.IsSuccess())
            {
                guildGoodsItems = new();
                var goodsJson = callback.GetFlattenJSON()["goods"];
                foreach (var column in goodsJson.Keys)
                {
                    if (column.Contains("totalGoods"))
                    {
                        GuildGoodsItem goodsItem = new();
                        goodsItem.totalGoodsAmount = GameUtils.GameUtils.TryParse<int>(column, goodsJson);

                        string goodsNum = column.Replace("totalGoods", "");
                        goodsNum = goodsNum.Replace("Amount", "");

                        goodsItem.key = "goods" + goodsNum;

                        int iGoodsNum = GameUtils.GameUtils.TryParse<int>(goodsNum);
                        BackEnd.goodsType goodsType = (BackEnd.goodsType)(iGoodsNum - 1);
                        goodsItem.goodsType = goodsType;

                        string goodsName = "goods" + goodsNum + "UserList";
                        LitJson.JsonData userListJson = goodsJson[goodsName];
                        for (int i = 0; i < userListJson.Count; i++)
                        {
                            GoodsUserItem user = new GoodsUserItem();
                            user.inDate = userListJson[i]["inDate"].ToString();
                            user.nickname = userListJson[i]["nickname"].ToString();
                            if (userListJson[i].ContainsKey("usingTotalAmount"))
                            {
                                user.usingTotalAmount = int.Parse(userListJson[i]["usingTotalAmount"].ToString());
                            }
                            user.totalAmount = int.Parse(userListJson[i]["totalAmount"].ToString());
                            user.updatedAt = userListJson[i]["updatedAt"].ToString();

                            goodsItem.userList.Add(user);
                        }
                        guildGoodsItems.Add(goodsItem);
                    }
                }
            }

            if (_onComplete != null)
                _onComplete(callback, guildGoodsItems);
        });
    }
    public void GetGuildGoodsByIndateV3(string _guildIndate, Action<BackendReturnObject> _onComplete = null)
    {
        SendQueue.Enqueue(Backend.Guild.GetGuildGoodsByIndateV3, _guildIndate, (callback) =>
        {
            // 이후 처리
            if (callback.IsSuccess())
            {
                Dictionary<string, GuildGoodsItem> goodsDictionary = new();

                var goodsJson = callback.GetFlattenJSON()["goods"];
                foreach (var column in goodsJson.Keys)
                {
                    if (column.Contains("totalGoods"))
                    {
                        GuildGoodsItem goodsItem = new();

                        goodsItem.totalGoodsAmount = GameUtils.GameUtils.TryParse<int>(column, goodsJson);

                        string goodsNum = column.Replace("totalGoods", "");
                        goodsNum = goodsNum.Replace("Amount", "");

                        string goodsName = "goods" + goodsNum + "UserList";

                        LitJson.JsonData userListJson = goodsJson[goodsName];
                        for (int i = 0; i < userListJson.Count; i++)
                        {
                            GoodsUserItem user = new GoodsUserItem();

                            user.inDate = userListJson[i]["inDate"].ToString();
                            user.nickname = userListJson[i]["nickname"].ToString();
                            if (userListJson[i].ContainsKey("usingTotalAmount"))
                            {
                                user.usingTotalAmount = int.Parse(userListJson[i]["usingTotalAmount"].ToString());
                            }
                            user.totalAmount = int.Parse(userListJson[i]["totalAmount"].ToString());
                            user.updatedAt = userListJson[i]["updatedAt"].ToString();

                            goodsItem.userList.Add(user);
                        }
                        goodsDictionary.Add("goods" + goodsNum, goodsItem);
                    }
                }

                foreach (var dic in goodsDictionary)
                {
                    Debug.Log($"-----{dic.Key}------\n" +
                    $"{dic.Value.ToString()}\n");
                }
            }

            if (_onComplete != null)
                _onComplete(callback);
        });
    }

    public void GetApplicantsV3(Action<BackendReturnObject, List<ApplicantsItem>> _onComplete = null)
    {
        SendQueue.Enqueue(Backend.Guild.GetApplicantsV3, callback =>
        {
            List<ApplicantsItem> applicantsList = null;
            if (callback.IsSuccess())
            {
                applicantsList = new List<ApplicantsItem>();
                LitJson.JsonData applicantsListJson = callback.FlattenRows();
                for (int i = 0; i < applicantsListJson.Count; i++)
                {
                    ApplicantsItem applicant = new ApplicantsItem();

                    if (applicantsListJson[i].ContainsKey("nickname"))
                    {
                        applicant.nickname = applicantsListJson[i]["nickname"].ToString();
                    }
                    applicant.inDate = applicantsListJson[i]["inDate"].ToString();

                    applicantsList.Add(applicant);
                    Debug.Log(applicant.ToString());
                }
            }
            if (_onComplete != null)
                _onComplete(callback, applicantsList);
        });
    }
    public void ApproveApplicantV3(string gamerIndate, Action<BackendReturnObject> _onComplete = null)
    {
        SendQueue.Enqueue(Backend.Guild.ApproveApplicantV3, gamerIndate, callback =>
        {
            if (callback.IsSuccess())
            {

            }
            if (_onComplete != null)
                _onComplete(callback);
        });
    }
    public void RejectApplicantV3(string gamerIndate, Action<BackendReturnObject> _onComplete = null)
    {
        SendQueue.Enqueue(Backend.Guild.RejectApplicantV3, gamerIndate, callback =>
        {
            if (callback.IsSuccess())
            {

            }
            if (_onComplete != null)
                _onComplete(callback);
        });
    }
    public void ExpelMemberV3(string gamerIndate, Action<BackendReturnObject> _onComplete = null)
    {
        SendQueue.Enqueue(Backend.Guild.ExpelMemberV3, gamerIndate, callback =>
        {
            if (callback.IsSuccess())
            {

            }
            if (_onComplete != null)
                _onComplete(callback);
        });
    }
    public void NominateMasterV3(string gamerIndate, Action<BackendReturnObject> _onComplete = null)
    {
        SendQueue.Enqueue(Backend.Guild.NominateMasterV3, gamerIndate, callback =>
        {
            if (callback.IsSuccess())
            {

            }
            if (_onComplete != null)
                _onComplete(callback);
        });
    }
    public void NominateViceMasterV3(string gamerIndate, Action<BackendReturnObject> _onComplete = null)
    {
        SendQueue.Enqueue(Backend.Guild.NominateViceMasterV3, gamerIndate, callback =>
        {
            if (callback.IsSuccess())
            {

            }
            if (_onComplete != null)
                _onComplete(callback);
        });
    }
    public void ReleaseViceMasterV3(string gamerIndate, Action<BackendReturnObject> _onComplete = null)
    {
        SendQueue.Enqueue(Backend.Guild.ReleaseViceMasterV3, gamerIndate, callback =>
        {
            if (callback.IsSuccess())
            {

            }
            if (_onComplete != null)
                _onComplete(callback);
        });
    }
    public void ModifyGuildV5(Param param, Action<BackendReturnObject> _onComplete = null)
    { 
        SendQueue.Enqueue(Backend.Guild.ModifyGuildV5, param, (callback) =>
        {
            if (callback.IsSuccess())
            {

            }
            if (_onComplete != null)
                _onComplete(callback);
        });
    }
}
