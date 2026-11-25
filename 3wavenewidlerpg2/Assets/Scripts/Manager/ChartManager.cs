using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using LitJson;
using BackEnd;
using System.IO;
using System.Linq;
using GameUtils;
using System.Text;
using UnityEngine.SceneManagement;
using BackEnd.Content;

[Serializable]
public class ChartInfoData
{
    public string keyName = string.Empty;
    public string chartName = string.Empty;
    public string chartExplain = string.Empty;
    public string old = string.Empty;

    public bool isChartUpload = false;
    public bool isLiveChart = false;
    public string chartVersion = string.Empty;

    #region new
    public int selectedChartFileId = 0;
    #endregion

    #region old
    public string uuid = string.Empty;
    public string inDate = string.Empty;
    #endregion


    #region CDN
    public BackEnd.Content.ContentTableItem tableItem = null;
    public string createdDate = string.Empty;
    #endregion

    public ChartInfoData(JsonData jsonData)
    {
        this.chartName = jsonData.ContainsKey("chartName") ? jsonData["chartName"].ToString() : string.Empty;
        this.chartExplain = jsonData.ContainsKey("chartExplain") ? jsonData["chartExplain"].ToString() : string.Empty;
        this.old = jsonData.ContainsKey("old") ? jsonData["old"].ToString() : string.Empty;

        string[] chartNameCuts = chartName.Split('_');
        if (chartNameCuts.Length == 2)
        {
            this.keyName = chartNameCuts[0];
            if (chartNameCuts[1].Equals("Live"))
                this.isLiveChart = true;
            else if (chartNameCuts[1].Equals("Dev"))
                this.isLiveChart = false;
        }
        else if (chartNameCuts.Length == 3)
        {
            this.keyName = chartNameCuts[0];
            if (chartNameCuts[1].Equals("Live"))
                this.isLiveChart = true;
            else if (chartNameCuts[1].Equals("Dev"))
                this.isLiveChart = false;
            chartVersion = chartNameCuts[2];
        }
        else
        {
            this.keyName = chartName;
        }

        int outNum = 0;
        if (int.TryParse(jsonData["selectedChartFileId"].ToString(), out outNum))
        {
            this.isChartUpload = true;
            this.selectedChartFileId = outNum;
        }
        else
        {
            this.isChartUpload = false;
            this.selectedChartFileId = 0;
            this.uuid = jsonData.ContainsKey("uuid") ? jsonData["uuid"].ToString() : string.Empty;
            this.inDate = jsonData.ContainsKey("inDate") ? jsonData["inDate"].ToString() : string.Empty;
        }
    }
    public ChartInfoData(BackEnd.Content.ContentTableItem item)
    {
        tableItem = item;
        this.chartName = item != null ? item.chartName : string.Empty;
        this.chartExplain = item != null ? item.chartExplain : string.Empty;
        this.old = string.Empty;

        string[] chartNameCuts = chartName.Split('_');
        if (chartNameCuts.Length == 2)
        {
            this.keyName = chartNameCuts[0];
            if (chartNameCuts[1].Equals("Live"))
                this.isLiveChart = true;
            else if (chartNameCuts[1].Equals("Dev"))
                this.isLiveChart = false;
        }
        else if (chartNameCuts.Length == 3)
        {
            this.keyName = chartNameCuts[0];
            if (chartNameCuts[1].Equals("Live"))
                this.isLiveChart = true;
            else if (chartNameCuts[1].Equals("Dev"))
                this.isLiveChart = false;
            chartVersion = chartNameCuts[2];
        }
        else
        {
            this.keyName = chartName;
        }

        this.selectedChartFileId = item != null ? GameUtils.GameUtils.TryParse<int>(item.selectedChartFileId) : 0;
        this.createdDate = item != null ? item.createdDate : string.Empty;
    }
}
public class ChartManager : Singleton<ChartManager>
{
    // 차트정보 데이터
    List<ChartInfoData> chartInfos = new();
    List<ChartInfoData> loadChartInfos = new();
    // Dictionary<int, ChartInfoData> dicChartInfoDatas = new();

    // 차트 데이터
    Dictionary<string, JsonData> dicChartJsonDatas = new();

    public Action LoadComplete = null;
    public Action LoadAllComplete = null;

    int curLoadChartCount = 0;
    public int CurLoadChartCount { get { return curLoadChartCount; } }
    // public int MaxLoadChartCount { get { return dicChartInfoDatas.Count; } }
    public int MaxLoadChartCount { get { return loadChartInfos.Count; } }
    bool isChartResetRetry = false;
    Coroutine localUpdateCoroutine = null;
    public void LoadAllChartData()
    {
        isChartResetRetry = false;
        chartInfos.Clear();
        loadChartInfos.Clear();
        // dicChartInfoDatas.Clear();
        dicChartJsonDatas.Clear();
        curLoadChartCount = 0;

        GetChartListV3();
        // GetChartList();
    }

    void GetChartListV3()
    {
        chartInfos.Clear();
        loadChartInfos.Clear();
        bool isLiveBuild = !GameManager.Instance.IsDevelopBuild;

        bool isInspectionBuild = false;
        if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
        {
            string clientVersion = Application.version;
            string serverVersion = BackendManager.Instance.BackendUtils.ServerVersion;
            // 앞 버전이 더 큰경우 result = 1
            bool result = GameUtils.GameUtils.IsClientVersionLow(clientVersion, serverVersion);
            if (result)
            {
                isInspectionBuild = true;
            }
        }
        if (isInspectionBuild) isLiveBuild = false;

        Debug.Log(isLiveBuild);

        Backend.CDN.Content.Table.Get(callback =>
        {
            if (callback.IsSuccess())
            {
                foreach (BackEnd.Content.ContentTableItem item in callback.GetContentTableItemList())
                {
                    ChartInfoData chartInfoData = new ChartInfoData(item);
                    if (chartInfoData == null || chartInfoData.selectedChartFileId == 0)
                        continue;

                    if (isLiveBuild == chartInfoData.isLiveChart)
                    {
                        // int selectedChartFileId = chartInfoData.selectedChartFileId;
                        // if (dicChartInfoDatas.ContainsKey(selectedChartFileId) == false)
                        //     dicChartInfoDatas.Add(selectedChartFileId, chartInfoData);
                        loadChartInfos.Add(chartInfoData);
                    }
                    chartInfos.Add(chartInfoData);
                }


                SetChartDataToLocal();
                //ChartInfoData cbtChart = chartInfos.Find(x => x.chartName.Equals("CbtChart"));
                //if (cbtChart != null)
                //{
                //    string cbtChartFileId = cbtChart.selectedChartFileId.ToString();
                //    CheckCbtChart(cbtChartFileId, () =>
                //    {

                //    });
                //}
            }
            else
                ShowErr(callback);
        });
    }

    //void CheckWhiteList(string selectedChartFileId, Action complete)
    //{
    //    SendQueue.Enqueue(Backend.Chart.GetChartContents, selectedChartFileId, (callback) =>
    //    {
    //        if (callback.IsSuccess())
    //        {
    //            JsonData jsonData = callback.FlattenRows();
    //            for (int i = 0; i < jsonData.Count; i++)
    //            {
    //                JsonData jData = jsonData[i];
    //                string Nickname = GameUtils.GameUtils.TryParse<string>("Nickname", jData);
    //                if (UserManager.Instance.UserInfo.nickname.Equals(Nickname))
    //                {
    //                    GameManager.Instance.IsWhiteList = true;
    //                    break;
    //                }
    //            }
    //        }
    //        else
    //            GameManager.Instance.IsWhiteList = false;

    //        if (complete != null)
    //            complete();
    //        complete = null;
    //    });
    //}
    void SetChartDataToLocal()
    {
        if (localUpdateCoroutine != null)
            StopCoroutine(localUpdateCoroutine);

        localUpdateCoroutine = StartCoroutine(LocalUpdateAsyncIEnumerator());
    }
    void HandleLoadComplete()
    {
        LoadComplete?.Invoke();
    }


    void RetryAfterChartReset(string reason)
    {
        Debug.LogError($"[Chart] 로드 실패\n{reason}");

        if (isChartResetRetry)
        {
            Debug.LogError("[Chart] 로컬 초기화 후 재시도도 실패했습니다.");

            localUpdateCoroutine = null;

            ShowErr(
                "게임 데이터를 불러오지 못했습니다.\n" +
                "앱을 완전히 종료한 후 다시 실행해주세요.\n\n" +
                reason
            );

            return;
        }

        isChartResetRetry = true;

        try
        {
            Debug.LogWarning("[Chart] CDN 로컬 차트 전체 초기화 시작");

            Backend.CDN.Content.Local.Reset();

            Debug.LogWarning("[Chart] CDN 로컬 차트 전체 초기화 완료");
        }
        catch (Exception e)
        {
            Debug.LogError($"[Chart] CDN 로컬 초기화 실패\n{e}");

            localUpdateCoroutine = null;
            ShowErr("게임 데이터 초기화에 실패했습니다.\n앱을 다시 실행해주세요.");
            return;
        }

        dicChartJsonDatas.Clear();
        curLoadChartCount = 0;

        localUpdateCoroutine = StartCoroutine(RetryLocalUpdateCoroutine());
    }
    void LoadFinish()
    {
        PlayerPrefs.Save();
        LoadAllComplete?.Invoke();

        LoadComplete = null;
        LoadAllComplete = null;
    }

    void ShowErr(BackendReturnObject _callback)
    {
        if (_callback == null)
            ShowErr();
        else
        {
            string context = string.Format("statusCode : {0}\nerrCode : {1}\nmsg : {2}", _callback.GetStatusCode(), _callback.GetErrorCode(), _callback.GetMessage());
            ShowErr(context);
        }
    }
    void ShowErr(string context = null)
    {
        if (string.IsNullOrEmpty(context))
            context = StringManager.Instance.GetStringText("Msg_NetworkDisconnect");
        BackendManager.Instance.BackendError.ShowErrPopup(context);
    }


    public JsonData GetChartJsonData(string _chartKey)
    {
        if (dicChartJsonDatas.ContainsKey(_chartKey))
            return dicChartJsonDatas[_chartKey];


        ShowErr(string.Format("error : {0}", _chartKey));
        return null;
    }

    private void GetProgress(int totalCount, int remainCount, string fileName)
    {
        Debug.Log("총 파일 갯수 : " + totalCount);
        Debug.Log("남은 파일 갯수 : " + remainCount);
        Debug.Log("현재 다운로드할 파일 이름 : " + fileName);
    }

    void CheckOpen()
    {
#if !UNITY_EDITOR
        JsonData jsonData = GetChartJsonData("OpenChart");
        if (jsonData != null)
        {
            DateTime? StartTime = null;
            if (jsonData[0].ContainsKey("StartTime"))
            {
                string strStartTime = GameUtils.GameUtils.TryParse<string>("StartTime", jsonData[0]);
                if (strStartTime.Equals("0"))
                {

                }
                else
                    StartTime = TimeManager.Instance.ParseToDateTimeKST(strStartTime);
            }

            DateTime curTime = TimeManager.Instance.GetCurrentDateTime();
            if (StartTime != null && StartTime.Value != null && StartTime.Value > curTime)
            {
                StopAllCoroutines();
                ShowErr($"{StartTime.Value.Hour}시 {StartTime.Value.Minute}분에 오픈됩니다.");
            }
        }
#endif
    }

    public void LocalUpdateAsync()
    {
        if (localUpdateCoroutine != null)
            StopCoroutine(localUpdateCoroutine);

        localUpdateCoroutine = StartCoroutine(LocalUpdateAsyncIEnumerator());
    }

    IEnumerator LocalUpdateAsyncIEnumerator()
    {
        BackEnd.Content.BackendContentTableReturnObject tableCallback = null;

        Debug.Log("[Chart] Content.Table.Get 시작");

        Backend.CDN.Content.Table.Get(bro =>
        {
            tableCallback = bro;
        });

        float tableTimeout = 30f;

        while (tableCallback == null && tableTimeout > 0f)
        {
            tableTimeout -= Time.unscaledDeltaTime;
            yield return null;
        }

        if (tableCallback == null)
        {
            localUpdateCoroutine = null;
            RetryAfterChartReset("Content.Table.Get 응답 시간 초과");
            yield break;
        }

        if (!tableCallback.IsSuccess())
        {
            string error = $"Content.Table.Get 실패\nStatusCode: {tableCallback.GetStatusCode()}\nErrorCode: {tableCallback.GetErrorCode()}\nMessage: {tableCallback.GetMessage()}";

            Debug.LogError(error);
            localUpdateCoroutine = null;
            RetryAfterChartReset(error);
            yield break;
        }

        Debug.Log("[Chart] Content.Table.Get 완료");

        BackEnd.Content.BackendContentReturnObject callback = null;

        Debug.Log("[Chart] Content.Local.Update 시작");

        Backend.CDN.Content.Local.Update(tableCallback.GetContentTableItemList(), GetProgress, bro =>
        {
            callback = bro;
        });

        float updateTimeout = 120f;

        while (callback == null && updateTimeout > 0f)
        {
            updateTimeout -= Time.unscaledDeltaTime;
            yield return null;
        }

        if (callback == null)
        {
            localUpdateCoroutine = null;
            RetryAfterChartReset("Content.Local.Update 응답 시간 초과");
            yield break;
        }

        if (!callback.IsSuccess())
        {
            string error = $"Content.Local.Update 실패\nStatusCode: {callback.GetStatusCode()}\nErrorCode: {callback.GetErrorCode()}\nMessage: {callback.GetMessage()}";

            Debug.LogError(error);
            localUpdateCoroutine = null;
            RetryAfterChartReset(error);
            yield break;
        }

        Debug.Log("[Chart] Content.Local.Update 완료");

        Dictionary<string, BackEnd.Content.ContentItem> dic = null;

        try
        {
            dic = callback.GetContentDictionarySortByChartId();
        }
        catch (Exception e)
        {
            localUpdateCoroutine = null;
            RetryAfterChartReset($"차트 Dictionary 생성 실패\n{e}");
            yield break;
        }

        if (dic == null || dic.Count == 0)
        {
            localUpdateCoroutine = null;
            RetryAfterChartReset("다운로드한 차트 Dictionary가 비어 있습니다.");
            yield break;
        }

        dicChartJsonDatas.Clear();
        curLoadChartCount = 0;

        foreach (var item in loadChartInfos)
        {
            if (item == null || item.tableItem == null)
                continue;

            string chartId = item.tableItem.chartId;

            if (string.IsNullOrEmpty(chartId))
            {
                Debug.LogError($"[Chart] ChartId가 비어 있음: {item.chartName}");
                continue;
            }

            if (!dic.TryGetValue(chartId, out BackEnd.Content.ContentItem contentItem))
            {
                Debug.LogError($"[Chart] 다운로드 목록에 차트가 없음: {item.chartName} / ChartId: {chartId}");
                localUpdateCoroutine = null;
                RetryAfterChartReset($"다운로드 목록에 차트가 없습니다.\nChartName: {item.chartName}\nChartId: {chartId}");
                yield break;
            }

            if (contentItem == null || string.IsNullOrEmpty(contentItem.contentString))
            {
                localUpdateCoroutine = null;
                RetryAfterChartReset($"차트 내용이 비어 있습니다.\nChartName: {item.chartName}\nChartId: {chartId}");
                yield break;
            }

            try
            {
                JsonData json = JsonMapper.ToObject(contentItem.contentString);

                if (json == null)
                    throw new Exception("JsonMapper.ToObject 결과가 null입니다.");

                if (dicChartJsonDatas.ContainsKey(item.keyName))
                    dicChartJsonDatas[item.keyName] = json;
                else
                    dicChartJsonDatas.Add(item.keyName, json);

                if (item.keyName == "OpenChart")
                    CheckOpen();

                curLoadChartCount++;
                HandleLoadComplete();

                Debug.Log($"[Chart] 파싱 완료: {item.chartName} ({curLoadChartCount}/{loadChartInfos.Count})");
            }
            catch (Exception e)
            {
                localUpdateCoroutine = null;
                RetryAfterChartReset($"차트 JSON 파싱 실패\nChartName: {item.chartName}\nChartId: {chartId}\n{e}");
                yield break;
            }

            yield return null;
        }

        if (curLoadChartCount != loadChartInfos.Count)
        {
            localUpdateCoroutine = null;
            RetryAfterChartReset($"차트 로드 수량 불일치\n완료: {curLoadChartCount}\n전체: {loadChartInfos.Count}");
            yield break;
        }

        yield return new WaitForSecondsRealtime(0.2f);

        Debug.Log($"[Chart] 전체 로드 완료: {curLoadChartCount}/{loadChartInfos.Count}");

        localUpdateCoroutine = null;
        LoadFinish();
    }
    IEnumerator RetryLocalUpdateCoroutine()
    {
        yield return null;
        yield return new WaitForSecondsRealtime(0.2f);

        localUpdateCoroutine = StartCoroutine(LocalUpdateAsyncIEnumerator());
    }
}
