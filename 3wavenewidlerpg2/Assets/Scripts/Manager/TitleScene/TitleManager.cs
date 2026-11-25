using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using BackEnd;

public class TitleManager : MonoBehaviour
{
    string marketUrl_google = string.Empty;

    static TitleManager _instance = null;
    public static TitleManager Instance { get { return _instance; } }
    public Camera MainCamera = null;
    public Camera UICamera = null;

    [SerializeField] UITitleScene uiTitleScene = null;
    [SerializeField] TMP_Text googleHash = null;
    bool bStart = false;
    System.DateTime appOpenTime = new System.DateTime();

    bool isComplete = false;
    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        bStart = false;
        SoundManager.Instance.PlayBgm(BGMType.Title);
        GameManager.Instance.gameType = GameType.Title;
        BackendManager.Instance.OnInitialized = this.HandleBackendInitComplete;
        BackendManager.Instance.Init();

        if (DataManager.Instance.StringData.IsSetLocalString == false)
            DataManager.Instance.StringData.SetLocalStringData();

        appOpenTime = System.DateTime.UtcNow;

        marketUrl_google = "market://details?id=" + Application.identifier;
        isComplete = false;
    }

    void TokenLogin()
    {
        uiTitleScene.SetLoadingText(StringManager.Instance.GetStringText("Msg_LoggingIn"));

        float delayTime = 1.0f - (System.DateTime.UtcNow - appOpenTime).Seconds;
        if (delayTime > 0)
        {
            Invoke("TokenLogin", delayTime);
            return;
        }

        int login = PlayerPrefs.GetInt("LoginType", -1);
        if (login.Equals((int)LoginType.Guest))
            BackendManager.Instance.GuestLogin(HandleTokenLoginComplete);
        else
            HandleTokenLoginComplete(null);
    }
    void GetServerStatus()
    {
        uiTitleScene.SetLoadingText(StringManager.Instance.GetStringText("Msg_LoadServerStatus"));
        uiTitleScene.SetGuageBar(0.25f);
        BackendManager.Instance.BackendUtils.GetServerStatus(HandleGetServerStatusComplete);
    }
    void GetLatestVersion()
    {
        uiTitleScene.SetLoadingText(StringManager.Instance.GetStringText("Msg_LoadServerVersion"));
        uiTitleScene.SetGuageBar(0.5f);
        BackendManager.Instance.BackendUtils.GetLatestVersion(HandleGetLatestVersionComplete);
    }
    void GetServerTime()
    {
        uiTitleScene.SetLoadingText(StringManager.Instance.GetStringText("Msg_LoadServerTime"));
        uiTitleScene.SetGuageBar(0.75f);
        BackendManager.Instance.BackendUtils.GetServerTime(HandleGetServerTimeComplete);
    }
    void GetUserInfo()
    {
        uiTitleScene.SetLoadingText(StringManager.Instance.GetStringText("Msg_LoadUserInfo"));
        uiTitleScene.SetGuageBar(1f);
        BackendManager.Instance.BackendLogin.GetUserInfo(HandleGetUserInfoComplete);
    }
    public void SetGoogleHash(string _text)
    {
        if (googleHash != null)
            googleHash.text = $"{_text}\nabcdefghijklmnopqrstuvwxyz\nABCDEFGHIJKLMNOPQRSTUVWXYZ\n01234567890";
    }
    void HandleBackendInitComplete()
    {
        TokenLogin();
    }
    void HandleGetServerStatusComplete()
    {
        ServerStatus serverStatus = BackendManager.Instance.BackendUtils.ServerStatus;
        if (serverStatus == ServerStatus.Online)
        {
            GetLatestVersion();
            return;
        }

        if (serverStatus == ServerStatus.Offline || serverStatus == ServerStatus.Maintenance)
        {
            string title = StringManager.Instance.GetStringText("Name_Notification");
            string context = StringManager.Instance.GetStringText("Msg_Maintenance");
            UIConfirmPopup popup = PopupManager.Instance.ShowPopup<UIConfirmPopup>();
            if (popup != null)
                popup.Setup(title, context).SetCancel(StringManager.Instance.GetStringText("Name_Quit"), Application.Quit);

            return;
        }

        HandleTokenLoginComplete(null);
    }
    void HandleGetLatestVersionComplete()
    {
        string clientVer = Application.version;
        string serverVer = BackendManager.Instance.BackendUtils.ServerVersion;
        if (clientVer.Equals(serverVer))
        {
            GetServerTime();
            return;
        }

        bool isLowVersion = true;
        string[] serverVerCuts = serverVer.Split('.');
        string[] clientVerCuts = clientVer.Split('.');
        for (int i = 0; i < clientVerCuts.Length; i++)
        {
            int serverCut = GameUtils.GameUtils.TryParse<int>(serverVerCuts[i]);
            int clientCut = GameUtils.GameUtils.TryParse<int>(clientVerCuts[i]);
            if (serverCut == clientCut)
                continue;

            isLowVersion = serverCut > clientCut;
            break;
        }

        if (isLowVersion && BackendManager.Instance.BackendUtils.UpdateType == UpdateType.Force)
        {
            UIConfirmPopup popup = PopupManager.Instance.ShowPopup<UIConfirmPopup>();
            if (popup != null)
            {
                string title = StringManager.Instance.GetStringText("Name_Update");
                string context = StringManager.Instance.GetStringText("Msg_Update");
                string confirm = StringManager.Instance.GetStringText("Name_UpdateConfirm");
                popup.Setup(title, context).SetConfirm(confirm, () =>
                {
#if UNITY_ANDROID
                    Application.OpenURL(marketUrl_google);
#endif
                });
            }
        }
        else
        {
            GetServerTime();
        }
    }
    void HandleGetServerTimeComplete(BackendReturnObject _bro)
    {
        if (_bro.IsSuccess())
        {
            TimeManager.Instance.SetTimestamp(BackendManager.Instance.BackendUtils.ServerTime);

            GetUserInfo();
        }
    }
    void HandleTokenLoginComplete(BackendReturnObject _bro)
    {
        Debug.Log("HandleTokenLoginComplete" + _bro);
        if (_bro == null)
        {
            ShowLoginPopup();
            return;
        }

        if (_bro.IsSuccess())
        {
            HandleLoginComplete(_bro);
        }
        else
        {
            string statusCode = _bro.GetStatusCode();

            // 400: 로컬 토큰 없음, 410: refresh_token 만료, 401: 다른 기기 로그인
            if (statusCode.Equals("400") || statusCode.Equals("410") || statusCode.Equals("401"))
            {
                ShowLoginPopup();
            }
            // 403: 차단 유저 또는 테스트 출시 AU 초과
            else if (statusCode.Equals("403"))
            {
                var popup = PopupManager.Instance.ShowPopup<UIConfirmPopup>();
                if (popup != null)
                {
                    popup.Setup(null, _bro.GetMessage()).SetCancel(StringManager.Instance.GetStringText("Name_Quit"), null);
                }
            }
        }
    }
    void ShowLoginPopup()
    {
        var popup = PopupManager.Instance.ShowPopup<UITitleLoginPopup>();
        if (popup != null)
        {
            popup.OnLoginComplete = HandleLoginComplete;
        }
    }

    void HandleLoginComplete(BackendReturnObject _bro)
    {
        if (_bro == null)
        {
            HandleTokenLoginComplete(null);
            return;
        }

        if (_bro.IsSuccess())
            GetLatestVersion();
        else
            GetServerStatus();
    }
    void HandleGetUserInfoComplete(BackendReturnObject _bro)
    {
        if (!_bro.IsSuccess())
            return;

        LitJson.JsonData userInfoJson = _bro.GetReturnValuetoJSON()["row"];
        string nickname = GameUtils.GameUtils.TryParse<string>("nickname", userInfoJson);

        if (string.IsNullOrEmpty(nickname))
        {
            BackendManager.Instance.BackendLogin.UpdateNickname(
                GameUtils.GameUtils.GenerateRandomNickname(),
                HandleUpdateNicknameComplete
            );
            return;
        }

        LoadAllChartData();
    }
    void LoadAllChartData()
    {
        uiTitleScene.SetLoadingText(StringManager.Instance.GetStringText("Msg_LoadChartDatas"));
        uiTitleScene.SetGuageBar(0f);
        ChartManager.Instance.LoadComplete = HandleLoadChartDataComplete;
        ChartManager.Instance.LoadAllComplete = CompleteTitleLoading;
        ChartManager.Instance.LoadAllChartData();
    }
    void HandleLoadChartDataComplete()
    {
        uiTitleScene.SetGuageBar((float)ChartManager.Instance.CurLoadChartCount / ChartManager.Instance.MaxLoadChartCount);
    }
    void HandleUpdateNicknameComplete(BackendReturnObject _callback, string _nickname)
    {
        if (!_callback.IsSuccess())
        {
            string statusCode = _callback.GetStatusCode();
            if (statusCode.Equals("400"))
                GameManager.Instance.UISystemScene.ShowToastMessagePanel(StringManager.Instance.GetStringText("Msg_CreateNickNameRule"));
            else if (statusCode.Equals("409"))
            {
                GameManager.Instance.UISystemScene.ShowToastMessagePanel(StringManager.Instance.GetStringText("Msg_DuplicateNickname"));
                BackendManager.Instance.BackendLogin.UpdateNickname(GameUtils.GameUtils.GenerateRandomNickname(), HandleUpdateNicknameComplete);
            }
            return;
        }
        LoadAllChartData();
    }
    void CompleteTitleLoading()
    {
        if (isComplete)
            return;

        uiTitleScene.SetGuageBar(1f);
        uiTitleScene.SetActiveGaugeBar(false);
        uiTitleScene.SetActiveColorTweener(true);
        uiTitleScene.SetLoadingText(StringManager.Instance.GetStringText("Msg_TouchToStart"));

        isComplete = true;
    }

    private void Update()
    {
        if (bStart)
            return;

        if (isComplete && Input.GetMouseButtonDown(0))
        {
            bStart = true;
            LoadingManager.LoadScene("InGameScene");
        }
    }
}
