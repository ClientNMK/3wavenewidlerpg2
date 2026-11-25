using System;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using BackEnd.Leaderboard;
using LitJson;

[Serializable]
public class RankTableItem
{
    public string rankType;
    public string date;
    public string uuid;
    public string order;
    public bool isReset;
    public string title;
    public string table;
    public string column;

    public DateTime rankStartDateAndTime;
    public DateTime rankEndDateAndTime;

    public string extraDataColumn;
    public string extraDataType;

    public override string ToString()
    {
        string str = $"rankType:{rankType}\n" +
                     $"date:{date}\n" +
                     $"uuid:{uuid}\n" +
                     $"order:{order}\n" +
                     $"isReset:{isReset}\n" +
                     $"title:{title}\n" +
                     $"table:{table}\n" +
                     $"column:{column}\n";

        if (rankStartDateAndTime != DateTime.MinValue)
        {
            str += $"rankStartDateAndTime:{rankStartDateAndTime}\n" +
                   $"rankEndDateAndTime:{rankEndDateAndTime}\n";
        }

        if (extraDataColumn != null)
        {
            str += $"extraDataColumn:{extraDataColumn}\n" +
                   $"extraDataType:{extraDataType}\n";
        }

        return str;
    }
}

[Serializable]
public class RankItem
{
    public string gamerInDate;
    public string nickname;
    public string score;
    public string index;
    public string rank;
    public string extraData = string.Empty;
    public string extraName = string.Empty;
    public string totalCount;

    public override string ToString()
    {
        string str = $"유저인데이트:{gamerInDate}\n닉네임:{nickname}\n점수:{score}\n정렬:{index}\n순위:{rank}\n총합:{totalCount}\n";
        if (extraName != string.Empty)
            str += $"{extraName}:{extraData}\n";

        return str;
    }
}

[Serializable]
public class CGuildRankItem
{
    public string guildInDate;
    public string guildName;
    public string score;
    public string index;
    public string rank;
    public string totalCount;
}

[Serializable]
public class RankRewardData
{
    public int StartRank;
    public int EndRank;
    public int ItemId;
    public double ItemValue;

    public RankRewardData(JsonData rewardJsonData)
    {
        StartRank = int.Parse(rewardJsonData["startRank"].ToString());
        EndRank = int.Parse(rewardJsonData["endRank"].ToString());
        ItemId = int.Parse(rewardJsonData["rewardItems"]["Data_Id"].ToString());
        ItemValue = int.Parse(rewardJsonData["rewardItemCount"].ToString());
    }

    public string GetRankString()
    {
        if (StartRank == 0 && EndRank == 0)
            return StringManager.Instance.GetStringText("Msg_Rank_All_Reward");

        if (StartRank == EndRank)
            return $"{StartRank}{StringManager.Instance.GetStringText("Msg_th")}";

        return $"{StartRank}~{EndRank}{StringManager.Instance.GetStringText("Msg_th")}";
    }
}

public class BackendRank
{
    public void GetUserRankTableList(List<RankTableItem> rankTableItems, Action<BackendReturnObject> complete = null)
    {
        Backend.Leaderboard.User.GetLeaderboards(callback =>
        {
            if (callback.IsSuccess())
            {
                if (rankTableItems == null)
                    rankTableItems = new List<RankTableItem>();

                rankTableItems.Clear();

                List<LeaderboardTableItem> tableList = callback.GetLeaderboardTableList();

                for (int i = 0; i < tableList.Count; i++)
                    rankTableItems.Add(ConvertRankTableItem(tableList[i]));
            }
            else
            {
                Debug.LogError("[BackendRank] GetUserRankTableList Fail : " + callback);
            }

            complete?.Invoke(callback);
        });
    }

    public void GetGuildRankTableList(List<RankTableItem> rankTableItems, Action<BackendReturnObject> complete = null)
    {
        Backend.Leaderboard.Guild.GetLeaderboards(callback =>
        {
            if (callback.IsSuccess())
            {
                if (rankTableItems == null)
                    rankTableItems = new List<RankTableItem>();

                rankTableItems.Clear();

                List<LeaderboardTableItem> tableList = callback.GetLeaderboardTableList();

                for (int i = 0; i < tableList.Count; i++)
                    rankTableItems.Add(ConvertRankTableItem(tableList[i]));
            }
            else
            {
                Debug.LogError("[BackendRank] GetGuildRankTableList Fail : " + callback);
            }

            complete?.Invoke(callback);
        });
    }

    public void GetMyRankItem(string uuid, Action<BackendReturnObject, RankItem> complete = null)
    {
        int gap = 0;

        Backend.Leaderboard.User.GetMyLeaderboard(uuid, gap, callback =>
        {
            RankItem rankItem = null;

            if (callback.IsSuccess())
            {
                List<UserLeaderboardItem> list = callback.GetUserLeaderboardList();
                long totalCount = callback.GetTotalCount();

                if (list != null && list.Count > 0)
                    rankItem = ConvertRankItem(list[0], totalCount);
            }
            else
            {
                Debug.LogError("[BackendRank] GetMyRankItem Fail : " + callback);
            }

            complete?.Invoke(callback, rankItem);
        });
    }
    public void GetMyGuildRankItem(string uuid, Action<BackendReturnObject, CGuildRankItem> complete = null)
    {

        Backend.Leaderboard.Guild.GetMyLeaderboard(uuid, callback =>
        {
            CGuildRankItem rankItem = null;

            if (callback.IsSuccess())
            {
                List<GuildLeaderboardItem> list = callback.GetGuildLeaderboardList();
                long totalCount = callback.GetTotalCount();

                if (list != null && list.Count > 0)
                    rankItem = ConvertGuildRankItem(list[0], totalCount);
            }
            else
            {
                Debug.LogError("[BackendRank] GetMyRankItem Fail : " + callback);
            }

            complete?.Invoke(callback, rankItem);
        });
    }
    public void GetRankList(string rankUUID, Action<BackendReturnObject, List<RankItem>> complete = null)
    {
        GetRankList(rankUUID, 50, complete);
    }

    public void GetRankList(string rankUUID, int count, Action<BackendReturnObject, List<RankItem>> complete = null)
    {
        int limit = Mathf.Clamp(count, 1, 50);

        Backend.Leaderboard.User.GetLeaderboard(rankUUID, limit, callback =>
        {
            List<RankItem> rankItems = null;

            if (callback.IsSuccess())
            {
                rankItems = new List<RankItem>();

                List<UserLeaderboardItem> list = callback.GetUserLeaderboardList();
                long totalCount = callback.GetTotalCount();

                for (int i = 0; i < list.Count; i++)
                    rankItems.Add(ConvertRankItem(list[i], totalCount));
            }
            else
            {
                Debug.LogError("[BackendRank] GetRankList Fail : " + callback);
            }

            complete?.Invoke(callback, rankItems);
        });
    }

    public void GetGuildRankList(string uuid, Action<BackendReturnObject, List<CGuildRankItem>> complete = null)
    {
        int limit = 50;

        Backend.Leaderboard.Guild.GetLeaderboard(uuid, limit, callback =>
        {
            List<CGuildRankItem> guildRankItems = new List<CGuildRankItem>();

            if (callback.IsSuccess())
            {
                List<GuildLeaderboardItem> list = callback.GetGuildLeaderboardList();
                long totalCount = callback.GetTotalCount();

                for (int i = 0; i < list.Count; i++)
                {
                    GuildLeaderboardItem src = list[i];

                    CGuildRankItem item = new CGuildRankItem
                    {
                        guildInDate = src.guildInDate,
                        guildName = src.guildName,
                        score = src.score,
                        index = src.index,
                        rank = src.rank,
                        totalCount = totalCount.ToString()
                    };

                    guildRankItems.Add(item);
                }
            }
            else
            {
                Debug.LogError("[BackendRank] GetGuildRankList Fail : " + callback);
            }

            complete?.Invoke(callback, guildRankItems);
        });
    }

    public void UpdateUserScore(string rankUUID, string tableName, string rowInDate, Param param, Action<BackendReturnObject> onComplete = null)
    {
        Backend.Leaderboard.User.UpdateMyDataAndRefreshLeaderboard(rankUUID, tableName, rowInDate, param, callback =>
        {
            if (callback.IsSuccess())
            {
                Debug.Log(rankUUID);
            }
            else
            {
                Debug.LogError("[BackendRank] UpdateUserScore Fail : " + callback);
            }

            onComplete?.Invoke(callback);
        });
    }
    public void UpdateMyGuildGoodsAndRefreshLeaderboard(string _uuid, goodsType _goodsType, GoodsUse _goodsUse, double _value, Action<BackendReturnObject> onComplete = null)
    {
        Backend.Leaderboard.Guild.UpdateMyGuildGoodsAndRefreshLeaderboard(_uuid, _goodsType, _goodsUse, _value, callback =>
        {
            if (callback.IsSuccess())
            {
                Debug.Log(_uuid);
            }
            else
            {
                Debug.LogError("[BackendRank] UpdateGuildScore Fail : " + callback);
            }

            onComplete?.Invoke(callback);
        });
    }

    public bool CheckRankingCalulateTime()
    {
        DateTime dt = TimeManager.Instance.GetCurrentDateTime();

        int checkHour_KR = 4;
        int checkHour_UTC = checkHour_KR - 9;
        if (checkHour_UTC < 0) checkHour_UTC += 24;

        return dt.Hour >= checkHour_UTC && dt.Hour < checkHour_UTC + 1;
    }

    private RankTableItem ConvertRankTableItem(LeaderboardTableItem src)
    {
        RankTableItem item = new RankTableItem
        {
            rankType = src.rankType,
            date = src.date,
            uuid = src.uuid,
            order = src.order,
            isReset = src.isReset,
            title = src.title,
            table = src.table,
            column = src.column,
            extraDataColumn = src.extraDataColumn,
            extraDataType = src.extraDataType
        };

        if (string.IsNullOrEmpty(src.rankStartDateAndTime) == false)
            DateTime.TryParse(src.rankStartDateAndTime, out item.rankStartDateAndTime);

        if (string.IsNullOrEmpty(src.rankEndDateAndTime) == false)
            DateTime.TryParse(src.rankEndDateAndTime, out item.rankEndDateAndTime);

        return item;
    }

    private RankItem ConvertRankItem(UserLeaderboardItem src, long totalCount)
    {
        RankItem item = new RankItem
        {
            gamerInDate = src.gamerInDate,
            nickname = src.nickname,
            score = src.score,
            index = src.index,
            rank = src.rank,
            extraData = src.extraData,
            extraName = src.extraName,
            totalCount = totalCount.ToString()
        };

        return item;
    }
    private CGuildRankItem ConvertGuildRankItem(GuildLeaderboardItem src, long totalCount)
    {
        CGuildRankItem item = new CGuildRankItem
        {
            guildInDate = src.guildInDate,
            guildName = src.guildName,
            score = src.score,
            index = src.index,
            rank = src.rank,
            totalCount = totalCount.ToString()
        };
        return item;
    }
}