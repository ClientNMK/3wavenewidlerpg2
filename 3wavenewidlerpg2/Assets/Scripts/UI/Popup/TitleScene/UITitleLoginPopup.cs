using System;
using UnityEngine;
using UnityEngine.UI;
using BackEnd;

// 테스트: 계정 ID만 입력해 로그인 (없는 ID면 가입)
public class UITitleLoginPopup : UIPopup
{
    [SerializeField] TMPro.TMP_InputField idInputField = null;
    [SerializeField] Button guestLoginButton = null;

    public Action<BackendReturnObject> OnLoginComplete = null;
    public override void Init()
    {
        if (bInitialized)
            return;

        touchPanel.onClick.AddListener(OnClickTouchPanel);
        guestLoginButton.onClick.AddListener(OnClickGuestLoginButton);

        bInitialized = true;
    }

    public override void OnClickTouchPanel()
    {

    }
    void OnClickGuestLoginButton()
    {
        if (string.IsNullOrEmpty(idInputField.text))
        {
            GameManager.Instance.UISystemScene.ShowToastMessagePanel(StringManager.Instance.GetStringText("Msg_IdPlaceholder"));
            return;
        }

        BackendManager.Instance.GuestLogin(idInputField.text, (callback) =>
        {
            PopupManager.Instance.ClosePopup(this);
            if (callback != null && callback.IsSuccess())
            {
                OnLoginComplete?.Invoke(callback);
                OnLoginComplete = null;
            }
            else
                OnLoginComplete?.Invoke(callback);
        });
    }

    public override void OnClickBackButton()
    {

    }
}
