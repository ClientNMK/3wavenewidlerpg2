using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;

public class BackendError : MonoBehaviour
{
    bool isError = false;

    bool bInitialzed = false;
    public void Init()
    {
        if (bInitialzed)
            return;

        

        bInitialzed = true;
    }

    public void Setup()
    {
        if (!bInitialzed)
            Init();

        if (Backend.IsInitialized)
        {
            Backend.ErrorHandler.OnMaintenanceError = OnMaintenanceError;
            Backend.ErrorHandler.OnTooManyRequestError = OnTooManyRequestError;
            Backend.ErrorHandler.OnTooManyRequestByLocalError = OnTooManyRequestByLocalError;
            Backend.ErrorHandler.OnOtherDeviceLoginDetectedError = OnOtherDeviceLoginDetectedError;
        }
        isError = false;

        bInitialzed = true;
    }

    public void Update()
    {
        //if (Backend.IsInitialized && Backend.ErrorHandler.UseAsyncQueuePoll)
        //{
        //    Backend.ErrorHandler.Poll();
        //}
    }

    public void ShowErrPopup(BackendReturnObject bro)
    {
        string errCode = bro.GetErrorCode();
        string statusCode = bro.GetStatusCode();
        string msg = bro.GetMessage();
        string context = string.Format("errCode : {0}\nstatusCode : {1}\nmsg : {2}", errCode, statusCode, msg);
        ShowErrPopup(context);
    }
    public void ShowErrPopup(string context)
    {
        if (isError)
            return;

        Time.timeScale = 0f;
        UIConfirmPopup popup = PopupManager.Instance.ShowPopup<UIConfirmPopup>("UIConfirmPopup", GameManager.Instance.SystemCanvas?.transform);
        if (popup != null)
        {
            popup.Setup(null, context)
                .SetConfirm(StringManager.Instance.GetStringText("Name_Quit"), () =>
                {
                    GameManager.Instance.QuitGame();
                });
        }
        StopAllCoroutines();
        StartCoroutine(Cor_Quit());
    }

    void OnMaintenanceError()
    {
        Debug.Log("점검 중입니다.");
        ShowErrPopup(StringManager.Instance.GetStringText("Msg_Maintenance"));
    }
    void OnTooManyRequestError()
    {
        Debug.Log("과도한 요청이 감지되었습니다.");
        ShowErrPopup(StringManager.Instance.GetStringText("Msg_TooManyRequest"));
    }
    void OnTooManyRequestByLocalError()
    {
        Debug.Log("과도한 요청중입니다.");
        ShowErrPopup(StringManager.Instance.GetStringText("Msg_TooManyRequest"));
    }
    void OnOtherDeviceLoginDetectedError()
    {
        Debug.Log("다른 기기에서 로그인이 감지되었습니다.");
        ShowErrPopup(StringManager.Instance.GetStringText("Msg_DetectedAnotherDevice"));
    }
    WaitForSecondsRealtime wfsrt = new WaitForSecondsRealtime(300f);
    IEnumerator Cor_Quit()
    {
        yield return wfsrt;
        GameManager.Instance.QuitGame();
    }
}
