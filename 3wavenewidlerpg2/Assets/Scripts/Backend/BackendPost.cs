using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using BackEnd;
using System.Linq;

[Serializable]
public class UPostReceiveItem
{
    public string chartFileName;
    public int Data_Id;
    public double itemCount;
    public override string ToString()
    {
        return
        "item : \n" +
        $"| chartFileName : {chartFileName}\n" +
        $"| itemID : {Data_Id}\n" +
        $"| itemCount : {itemCount}\n";
    }
}

[Serializable]
public class UPostItem
{
    public PostType postType;
    public string title;
    public string content;
    public DateTime expirationDate;
    public DateTime reservationDate;
    public DateTime sentDate;
    public string nickname;
    public string inDate;
    public string author; // 관리자 우편만 존재합니다.  
    public string rankType; // 랭킹 우편만 존재합니다.  
    public List<UPostReceiveItem> items = new List<UPostReceiveItem>();
    public override string ToString()
    {
        string totalString =
        $"title : {title}\n" +
        $"inDate : {inDate}\n";
        if (postType == PostType.Admin || postType == PostType.Rank)
        {
            totalString +=
            $"content : {content}\n" +
            $"expirationDate : {expirationDate}\n" +
            $"reservationDate : {reservationDate}\n" +
            $"sentDate : {sentDate}\n" +
            $"nickname : {nickname}\n";
            if (postType == PostType.Admin)
            {
                totalString += $"author : {author}\n";
            }
            if (postType == PostType.Rank)
            {
                totalString += $"rankType : {rankType}\n";
            }
        }
        string itemList = string.Empty;
        for (int i = 0; i < items.Count; i++)
        {
            itemList += items[i].ToString();
            itemList += "\n";
        }
        totalString += itemList;
        return totalString;
    }
}

public class BackendPost
{
    public Action<BackendReturnObject> OnGetPostList = null;
    public Action<BackendReturnObject> OnReceivePostItemAll = null;

    public void Setup()
    {
        OnGetPostList = null;
        OnReceivePostItemAll = null;
    }

    public void GetPostList(PostType postType, int limit = 100, Action<BackendReturnObject, List<UPostItem>> complete = null)
    {
        SendQueue.Enqueue(Backend.UPost.GetPostList, postType, limit, callback =>
        {
            List<UPostItem> postItems = null;
            if (callback.IsSuccess())
            {
                postItems = new();
                LitJson.JsonData postListJson = callback.GetReturnValuetoJSON()["postList"];
                for (int i = 0; i < postListJson.Count; i++)
                {
                    UPostItem postItem = new UPostItem();
                    postItem.inDate = postListJson[i]["inDate"].ToString();
                    postItem.title = postListJson[i]["title"].ToString();
                    postItem.postType = postType;

                    if (postType == PostType.Admin || postType == PostType.Rank)
                    {
                        postItem.content = postListJson[i]["content"].ToString();
                        postItem.expirationDate = TimeManager.Instance.ParseToDateTimeUTC(postListJson[i]["expirationDate"].ToString());
                        postItem.reservationDate = TimeManager.Instance.ParseToDateTimeUTC(postListJson[i]["reservationDate"].ToString());
                        postItem.nickname = postListJson[i]["nickname"]?.ToString();
                        postItem.sentDate = TimeManager.Instance.ParseToDateTimeUTC(postListJson[i]["sentDate"].ToString());

                        if (postListJson[i].ContainsKey("author"))
                        {
                            postItem.author = postListJson[i]["author"].ToString();
                        }

                        if (postListJson[i].ContainsKey("rankType"))
                        {
                            postItem.rankType = postListJson[i]["rankType"].ToString();
                        }
                    }
                    if (postListJson[i]["items"].Count > 0)
                    {
                        for (int itemNum = 0; itemNum < postListJson[i]["items"].Count; itemNum++)
                        {
                            UPostReceiveItem item = new UPostReceiveItem();
                            item.itemCount = GameUtils.GameUtils.TryParse<double>(postListJson[i]["items"][itemNum]["itemCount"].ToString());
                            item.chartFileName = postListJson[i]["items"][itemNum]["item"]["chartFileName"].ToString();
                            item.Data_Id = GameUtils.GameUtils.TryParse<int>(postListJson[i]["items"][itemNum]["item"]["Data_Id"].ToString());

                            postItem.items.Add(item);
                        }
                    }
                    postItems.Add(postItem);
                    // Debug.Log(postItem.ToString());
                }
            }

            if (complete != null)
                complete(callback, postItems);
            complete = null;
        });

    }

    public void ReceivePostItem(PostType postType, string postIndate)
    {
        SendQueue.Enqueue(Backend.UPost.ReceivePostItem, postType, postIndate, callback =>
        {
            if (callback.IsSuccess())
            {

            }
        });
    }

    public void ReceivePostItemAll(PostType postType, Action<List<UPostReceiveItem>> complete = null)
    {
        SendQueue.Enqueue(Backend.UPost.ReceivePostItemAll, postType, callback =>
        {
            if (callback.IsSuccess())
            {
                List<UPostReceiveItem> postReceiveItems = new();
                var jsonData = callback.GetReturnValuetoJSON();
                if (jsonData.ContainsKey("postItems"))
                {
                    var postItems = jsonData["postItems"];
                    foreach (LitJson.JsonData postItemJson in postItems)
                    {
                        for (int j = 0; j < postItemJson.Count; j++)
                        {
                            if (!postItemJson[j].ContainsKey("item"))
                                continue;

                            UPostReceiveItem item = new UPostReceiveItem();
                            // 랭킹 보상의 경우 chartFileName이 존재하지 않습니다.  
                            if (postItemJson[j]["item"].ContainsKey("chartFileName"))
                            {
                                item.chartFileName = postItemJson[j]["item"]["chartFileName"].ToString();
                            }
                            if (postItemJson[j]["item"].ContainsKey("Data_Id"))
                            {
                                item.Data_Id = GameUtils.GameUtils.TryParse<int>(postItemJson[j]["item"]["Data_Id"].ToString());
                            }
                            if (postItemJson[j].ContainsKey("itemCount"))
                            {
                                item.itemCount = GameUtils.GameUtils.TryParse<double>(postItemJson[j]["itemCount"].ToString());
                            }
                            postReceiveItems.Add(item);
                            // Debug.Log(item.ToString());
                        }
                    }
                }

                if (complete != null)
                    complete(postReceiveItems);
                complete = null;
            }
        });
    }
}
