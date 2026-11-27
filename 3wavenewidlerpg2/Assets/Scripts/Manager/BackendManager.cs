using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using BackEnd;
using GameUtils;
using System.Threading.Tasks;

public class BackendManager : Singleton<BackendManager>
{
    #region Backend Class
    public BackendUtils BackendUtils { get; } = new();
    public BackendLogin BackendLogin { get; } = new();
    public BackendRank BackendRank { get; } = new();
    public BackendGameData BackendGameData { get; } = new();
    public BackendError BackendError { get; } = new();
    public BackendChat BackendChat { get; } = new();
    public BackendProbability BackendProbability { get; } = new();
    public BackendPost BackendPost { get; } = new();
    public BackendGuild BackendGuild { get; } = new();
    public BackendRandomInfo BackendRandomInfo { get; } = new();
    public BackendCoupon BackendCoupon { get; } = new();
    public BackendNotification BackendNotification { get; } = new();
    public BackendDataBase BackendDataBase { get; } = new();

    public TheBackend.ToolKit.InvalidFilter.FilterManager FilterManager = null;
    public Queue<Action> MainThreadQueue = new Queue<Action>();
    #endregion

    public bool IsInitialized { get { return Backend.IsInitialized; } }
    public Action OnInitialized = null;

    public void Init()
    {
        Debug.Log("BackendManager Init Start");

        if (Backend.IsInitialized == false)
        {
            //var bro = Backend.Initialize(true, true);
            //if (bro.IsSuccess())
            //{
            //    HandleInitializedComplete();
            //}
            //else
            //{
            //    // 초기화 실패 시 로직
            //}

            BackendCustomSetting settings = new BackendCustomSetting();
            settings.autoRefreshToken = true;
            settings.useAsyncPoll = true;

            Backend.InitializeAsync(callback =>
            {
                if (callback.IsSuccess())
                {
                    HandleInitializedComplete();
                }
                else
                {
                    Debug.Log(callback);
                }
            });
        }
        else
        {
            HandleInitializedComplete();
        }
    }

    public void GuestLogin(string id, Action<BackendReturnObject> onComplete = null)
    {
        Debug.Log("GuestLogin");

        BackendLogin.GuestLogin(id, (callback) =>
        {
            Debug.Log($"GuestLogin callback : {callback}");

            //if (callback != null && callback.IsSuccess())
            //{
            //    OnLoginSuccess();
            //}

            onComplete?.Invoke(callback);
            onComplete = null;
        });
    }

    private void HandleInitializedComplete()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (!string.IsNullOrEmpty(Backend.Utils.GetGoogleHash()))
            {
                string hashCode = Backend.Utils.GetGoogleHash();
                Debug.Log("GetGoogleHash = " + hashCode);
                TitleManager.Instance.SetGoogleHash(hashCode);
            }
        }
        FilterManager = new();
        if (FilterManager.LoadInvalidString())
        {
            Debug.Log("성공했습니다.");
        }
        else
        {
            Debug.LogError("실패했습니다.");
        }

        BackendError.Setup();

        if (OnInitialized != null)
            OnInitialized();
        OnInitialized = null;
    }
    public void RefreshToken()
    {
        bool istoken = Backend.BMember.IsAccessTokenAlive().GetMessage() == "Success" ? true : false;
        if (!istoken)
        {
            BackendReturnObject obj = Backend.BMember.RefreshTheBackendToken();
            if (obj.IsSuccess())
            {

            }
        }
    }
    //async Task OnLoginSuccess()
    //{
    //    try
    //    {
    //        Debug.Log("OnLoginSuccess DB Setup");
    //        await BackendDataBase.Setup();
    //    }
    //    catch (Exception e)
    //    {
    //        Debug.LogError($"OnLoginSuccess DB Init Error: {e}");
    //    }
    //}

    private void Update()
    {
        if (Backend.IsInitialized)
        {
            SendQueue.Poll();
            if (BackendError != null)
                BackendError.Update();

            if (BackendChat != null)
                BackendChat.Update();

            if (MainThreadQueue != null && MainThreadQueue.Count > 0)
            {
                // Dequeue를 통해 행동을 추출 후 호출한다.(메인쓰레드이기 떄문)
                MainThreadQueue.Dequeue().Invoke();
            }
        }
    }

    public void OnDestroy()
    {
        if (BackendChat != null)
            BackendChat.OnDestroy();
        if (BackendNotification != null)
            BackendNotification.OnDestroy();
    }

    override protected void OnApplicationQuit()
    {
        if (BackendChat != null)
            BackendChat.OnApplicationQuit();
        if (BackendNotification != null)
            BackendNotification.OnApplicationQuit();

        base.OnApplicationQuit();
    }

    public void DeleteAllChart()
    {
        // Backend.Chart.DeleteLocalChartData();
    }
}
