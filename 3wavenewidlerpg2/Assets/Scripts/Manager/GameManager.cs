using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
using System;
public enum GameType
{
    Title = 1,
    InGame,
}

public class GameManager : Singleton<GameManager>
{
    public GameType gameType = GameType.Title;

    public bool IsDevelopBuild = true;
    public bool IsWhiteList = false;

    public Canvas SystemCanvas = null;
    public UISystemScene UISystemScene = null;
    public InputManager InputManager = null;

    public DateTime? CbtStartTime = null;
    public DateTime? CbtEndTime = null;
    public bool SetCbtData = false;
    float checkTimeLength = 60;
    float curCheckTime = 0;

    public bool IsPaused { get { return isPause; } }
    void Start()
    {
        Init();
    }

    void Init()
    {
        CultureInfo info = CultureInfo.CurrentCulture;
        Debug.Log(info.Name);

        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        Application.runInBackground = true;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Application.targetFrameRate = 60; // targetFrameRates[PlayerPrefs.GetInt(frameRateKey, 2)];
        QualitySettings.vSyncCount = 0;

        // 허용할 방향 모두 ON
        Screen.autorotateToPortrait = true;
        Screen.autorotateToPortraitUpsideDown = true;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;

        // 1) 앱 시작 직후 세로로 고정
        Screen.orientation = ScreenOrientation.Portrait;
        Screen.orientation = ScreenOrientation.AutoRotation;
    }
    private void Update()
    {
        if (SetCbtData == false)
            return;

        curCheckTime += Time.deltaTime;
        if (curCheckTime > checkTimeLength)
        {
            curCheckTime = 0;
            if (CheckCbtTime())
            {
                BackendManager.Instance.BackendError.ShowErrPopup("CBT가 종료되었습니다. \n배고파용 키우기 CBT에 참여해주셔서 감사합니다.");
                return;
            }
        }
    }
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_ANDROID
        // System.Diagnostics.Process.GetCurrentProcess().Kill();
        Application.Quit();
#elif UNITY_IOS
        Application.Quit();
#endif
    }

    bool isPause = false;
    private void OnApplicationPause(bool pause)
    {
        //if (Application.isEditor)
        //    return;

        if (isPause == pause)
            return;

        if (BackendManager.Instance.IsInitialized == false)
            return;

        isPause = pause;
        if (isPause)
        {
            if (gameType != GameType.InGame)
                return;
        }
        else
        {
            BackendManager.Instance.BackendUtils.GetServerTime((callback) =>
            {
                if (callback.IsSuccess())
                {
                    LitJson.JsonData jsonData = callback.GetReturnValuetoJSON();
                    string datetime = jsonData["utcTime"].ToString();
                    TimeManager.Instance.SetTimestamp(datetime);
                }
            });
        }
    }
    private void OnApplicationFocus(bool focus)
    {
        //if (Application.isEditor)
        //    return;

        if (isPause == !focus)
            return;

        if (BackendManager.Instance.IsInitialized == false)
            return;

        isPause = !focus;
        if (isPause)
        {
            if (gameType != GameType.InGame)
                return;
        }
        else
        {
            BackendManager.Instance.BackendUtils.GetServerTime((callback) =>
            {
                if (callback.IsSuccess())
                {
                    LitJson.JsonData jsonData = callback.GetReturnValuetoJSON();
                    string datetime = jsonData["utcTime"].ToString();
                    TimeManager.Instance.SetTimestamp(datetime);
                }
            });
        }
    }

    override protected void OnApplicationQuit()
    {
        base.OnApplicationQuit();
    }
    bool CheckCbtTime()
    {
        if (CbtEndTime.Value < TimeManager.Instance.GetCurrentDateTime())
            return true;

        return false;
    }
}
