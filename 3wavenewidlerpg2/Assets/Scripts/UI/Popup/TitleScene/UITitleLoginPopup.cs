using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using BackEnd;

public class UITitleLoginPopup : UIPopup
{
    const string privacyUrl_KR = "https://sites.google.com/view/3wavestudio-personal";
    const string serviceUrl_KR = "https://sites.google.com/view/3wavestudio-service";

    const string privacyUrl_US = "https://sites.google.com/view/3wavestudio-personal-us";
    const string serviceUrl_US = "https://sites.google.com/view/3wavestudio-service-us";

    [SerializeField] Button guestLoginButton = null;

    [SerializeField] UICheckBoxButton privacyButton = null;
    [SerializeField] UICheckBoxButton termsButton = null;
    [SerializeField] Button privacyLinkButton = null;
    [SerializeField] Button termsLinkButton = null;

    bool isAgreePrivacy = true;
    bool isAgreeTerms = true;

    public Action<BackendReturnObject> OnLoginComplete = null;
    public override void Init()
    {
        if (bInitialized)
            return;

        touchPanel.onClick.AddListener(OnClickTouchPanel);

        guestLoginButton.onClick.AddListener(OnClickGuestLoginButton);

        privacyButton.Button.onClick.AddListener(OnClickPrivacyButton);
        termsButton.Button.onClick.AddListener(OnClickTermsButton);

        privacyLinkButton.onClick.AddListener(OnClickPrivacyLinkButton);
        termsLinkButton.onClick.AddListener(OnClickTermsLinkButton);

        bInitialized = true;
    }

    public override void Setup()
    {
        base.Setup();

        isAgreePrivacy = true;
        isAgreeTerms = true;

        privacyButton.SetActiveCheckImage(isAgreePrivacy);
        termsButton.SetActiveCheckImage(isAgreeTerms);
    }

    public override void OnClickTouchPanel()
    {

    }
    void OnClickGuestLoginButton()
    {
        if (!isAgreePrivacy || !isAgreeTerms)
        {
            GameManager.Instance.UISystemScene.ShowToastMessagePanel(StringManager.Instance.GetStringText("Msg_CheckRequiredAgreement"));
            return;
        }

        UIConfirmPopup popup = PopupManager.Instance.ShowPopup<UIConfirmPopup>();
        if (popup != null)
        {
            string context = StringManager.Instance.GetStringText("Msg_GuestLogin_Confirm");

            popup.Setup(null, context)
                .SetConfirm(null, () =>
                {
                    BackendManager.Instance.GuestLogin((callback) =>
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
                }).SetCancel(null, null);
        }
    }
    void OnClickPrivacyButton()
    {
        isAgreePrivacy = !isAgreePrivacy;
        privacyButton.SetActiveCheckImage(isAgreePrivacy);
    }
    void OnClickTermsButton()
    {
        isAgreeTerms = !isAgreeTerms;
        termsButton.SetActiveCheckImage(isAgreeTerms);
    }
    void OnClickPrivacyLinkButton()
    {
        SystemLanguage systemLanguage = StringManager.Instance.GetLanguageType();
        Application.OpenURL(systemLanguage == SystemLanguage.Korean ? privacyUrl_KR : privacyUrl_US);
    }
    void OnClickTermsLinkButton()
    {
        SystemLanguage systemLanguage = StringManager.Instance.GetLanguageType();
        Application.OpenURL(systemLanguage == SystemLanguage.Korean ? serviceUrl_KR : serviceUrl_US);
    }

    public override void OnClickBackButton()
    {

    }
}
