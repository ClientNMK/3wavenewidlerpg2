using System;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using BackEnd.ProbabilityContent;
using LitJson;

public class ProbabilityCardV2
{
    public string probabilityName;
    public string probabilityExplain;
    public string selectedProbabilityFileId;

    public override string ToString()
    {
        return $"probabilityName: {probabilityName}\n" +
               $"probabilityExplain: {probabilityExplain}\n" +
               $"selectedProbabilityFileId: {selectedProbabilityFileId}\n";
    }
}

[Serializable]
public class ProbabilityItem
{
    public int Data_Id;
    public float percent;
    public int grade;

    public override string ToString()
    {
        return $"Data_Id : {Data_Id}\n" +
               $"percent : {percent}\n" +
               $"grade : {grade}\n";
    }
}

public class BackendProbability
{
    public List<ProbabilityCardV2> ProbabilityCardList = new();
    public Dictionary<string, Queue<ProbabilityItem>> ProbabilityItemList = new();

    private readonly Dictionary<string, List<ProbabilityItem>> probabilityContentCache = new();
    private readonly Dictionary<string, ProbabilityContentItem> probabilityContentByFileId = new();

    bool bInitialzed = false;

    public void Init()
    {
        if (bInitialzed)
            return;

        bInitialzed = true;
    }

    public void GetProbabilityCardListV2(Action<BackendReturnObject> complete = null)
    {
        Backend.CDN.Probability.Table.Get(callback =>
        {
            if (callback.IsSuccess())
            {
                ProbabilityCardList.Clear();

                List<ProbabilityTableItem> tableList = callback.GetProbabilityTableItemList();

                for (int i = 0; i < tableList.Count; i++)
                {
                    ProbabilityTableItem tableItem = tableList[i];

                    ProbabilityCardV2 probabilityCard = new ProbabilityCardV2
                    {
                        probabilityName = tableItem.probabilityName,
                        probabilityExplain = string.Empty,
                        selectedProbabilityFileId = tableItem.selectedProbabilityFileId
                    };

                    ProbabilityCardList.Add(probabilityCard);
                }
            }
            else
            {
                Debug.LogError("[BackendProbability] GetProbabilityCardListV2 Fail : " + callback);
            }

            complete?.Invoke(callback);
        });
    }

    public void GetProbabilityContents(string fileId, Action<BackendReturnObject, List<ProbabilityItem>, bool> complete = null)
    {
        if (string.IsNullOrEmpty(fileId))
        {
            Debug.LogError("[BackendProbability] fileId is null or empty");
            complete?.Invoke(null, null, false);
            return;
        }

        Backend.CDN.Probability.Table.Get(tableBro =>
        {
            if (tableBro.IsSuccess() == false)
            {
                Debug.LogError("[BackendProbability] Probability Table Get Fail : " + tableBro);
                complete?.Invoke(tableBro, null, false);
                return;
            }

            List<ProbabilityTableItem> tableList = tableBro.GetProbabilityTableItemList();

            Backend.CDN.Probability.Get(tableList, GetProgress, contentBro =>
            {
                if (contentBro.IsSuccess() == false)
                {
                    Debug.LogError("[BackendProbability] Probability Content Get Fail : " + contentBro);
                    complete?.Invoke(contentBro, null, false);
                    return;
                }

                BuildProbabilityContentCache(contentBro);

                List<ProbabilityItem> itemList = GetCachedProbabilityItems(fileId);
                bool isSuccess = itemList != null;

                if (isSuccess == false)
                    Debug.LogError("[BackendProbability] Probability content not found. fileId : " + fileId);

                complete?.Invoke(contentBro, itemList, isSuccess);
            });
        });
    }

    public void GetProbabilityContents(string[] fileIds, int[] levels, Action<BackendReturnObject, int, List<ProbabilityItem>, bool> complete = null)
    {
        if (fileIds == null || levels == null || fileIds.Length != levels.Length)
            return;

        Backend.CDN.Probability.Table.Get(tableBro =>
        {
            if (tableBro.IsSuccess() == false)
            {
                Debug.LogError("[BackendProbability] Probability Table Get Fail : " + tableBro);
                return;
            }

            List<ProbabilityTableItem> tableList = tableBro.GetProbabilityTableItemList();

            Backend.CDN.Probability.Get(tableList, GetProgress, contentBro =>
            {
                if (contentBro.IsSuccess() == false)
                {
                    Debug.LogError("[BackendProbability] Probability Content Get Fail : " + contentBro);
                    return;
                }

                BuildProbabilityContentCache(contentBro);

                for (int i = 0; i < fileIds.Length; i++)
                {
                    string fileId = fileIds[i];
                    int level = levels[i];
                    bool isFinal = i == fileIds.Length - 1;

                    List<ProbabilityItem> itemList = GetCachedProbabilityItems(fileId);
                    bool isSuccess = itemList != null;

                    if (isSuccess == false)
                        Debug.LogError("[BackendProbability] Probability content not found. fileId : " + fileId);

                    complete?.Invoke(contentBro, level, itemList, isFinal);
                }
            });
        });
    }

    public void GetProbabilitys(string cardFileID, int count, List<ProbabilityItem> resultProbabilitys, Action complete = null)
    {
        if (string.IsNullOrEmpty(cardFileID))
        {
            Debug.LogError("[BackendProbability] cardFileID is null or empty");
            complete?.Invoke();
            return;
        }

        if (resultProbabilitys == null)
        {
            Debug.LogError("[BackendProbability] resultProbabilitys is null");
            complete?.Invoke();
            return;
        }

        if (count < 1) count = 1;
        if (count > 100) count = 100;

        if (ProbabilityItemList.ContainsKey(cardFileID) == false)
            ProbabilityItemList.Add(cardFileID, new Queue<ProbabilityItem>());

        Queue<ProbabilityItem> probabilityItems = ProbabilityItemList[cardFileID];

        if (count <= probabilityItems.Count)
        {
            for (int i = 0; i < count; i++)
                resultProbabilitys.Add(probabilityItems.Dequeue());

            complete?.Invoke();
            return;
        }

        Backend.Probability.GetProbabilitys(cardFileID, 100, callback =>
        {
            if (callback.IsSuccess())
            {
                JsonData json = callback.GetFlattenJSON()["elements"];

                for (int i = 0; i < json.Count; i++)
                {
                    ProbabilityItem item = ParseProbabilityItem(json[i]);
                    probabilityItems.Enqueue(item);
                }

                for (int i = 0; i < count && probabilityItems.Count > 0; i++)
                    resultProbabilitys.Add(probabilityItems.Dequeue());
            }
            else
            {
                Debug.LogError("[BackendProbability] GetProbabilitys Fail : " + callback);
            }

            complete?.Invoke();
        });
    }

    private void BuildProbabilityContentCache(BackendProbabilityContentReturnObject bro)
    {
        probabilityContentCache.Clear();
        probabilityContentByFileId.Clear();

        List<ProbabilityContentItem> contentList = bro.GetProbabilityContentList();

        for (int i = 0; i < contentList.Count; i++)
        {
            ProbabilityContentItem content = contentList[i];

            if (content == null)
                continue;

            if (string.IsNullOrEmpty(content.selectedProbabilityFileId) == false)
                probabilityContentByFileId[content.selectedProbabilityFileId] = content;

            List<ProbabilityItem> itemList = ParseProbabilityContent(content.contentJson);

            if (string.IsNullOrEmpty(content.selectedProbabilityFileId) == false)
                probabilityContentCache[content.selectedProbabilityFileId] = itemList;

            if (string.IsNullOrEmpty(content.probabilityId) == false)
                probabilityContentCache[content.probabilityId] = itemList;

            if (string.IsNullOrEmpty(content.probabilityName) == false)
                probabilityContentCache[content.probabilityName] = itemList;
        }
    }

    private List<ProbabilityItem> GetCachedProbabilityItems(string key)
    {
        if (string.IsNullOrEmpty(key))
            return null;

        if (probabilityContentCache.TryGetValue(key, out List<ProbabilityItem> itemList))
            return itemList;

        return null;
    }

    private List<ProbabilityItem> ParseProbabilityContent(JsonData json)
    {
        List<ProbabilityItem> itemList = new();

        if (json == null)
            return itemList;

        JsonData rows = null;

        if (json.IsArray)
        {
            rows = json;
        }
        else if (json.ContainsKey("rows"))
        {
            rows = json["rows"];
        }
        else if (json.ContainsKey("elements"))
        {
            rows = json["elements"];
        }

        if (rows == null)
            return itemList;

        for (int i = 0; i < rows.Count; i++)
            itemList.Add(ParseProbabilityItem(rows[i]));

        return itemList;
    }

    private ProbabilityItem ParseProbabilityItem(JsonData json)
    {
        ProbabilityItem item = new ProbabilityItem
        {
            Data_Id = GameUtils.GameUtils.TryParse<int>("Data_Id", json),
            percent = GameUtils.GameUtils.TryParse<float>("percent", json),
            grade = GameUtils.GameUtils.TryParse<int>("grade", json)
        };

        return item;
    }

    private void GetProgress(int totalCount, int remainCount, string fileName)
    {
        Debug.Log($"[BackendProbability] totalCount : {totalCount} / remainCount : {remainCount} / fileName : {fileName}");
    }
}