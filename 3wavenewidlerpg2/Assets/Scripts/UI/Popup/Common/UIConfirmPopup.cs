using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIConfirmPopup : UIPopup
{
    [SerializeField] TMP_Text titleText = null;
    [SerializeField] ScrollRect contextScrollRect = null;
    [SerializeField] RectTransform contextPanel = null;
    [SerializeField] ContentSizeFitter descCSF = null;
    [SerializeField] TMP_Text descText = null;
    [SerializeField] Button cancelButton = null;
    [SerializeField] Button confirmButton = null;
    [SerializeField] TMP_Text confirmButtonText = null;
    [SerializeField] TMP_Text cancelButtonText = null;
    //[SerializeField] Button closeButton = null;
    Action OnCancel = null;
    Action OnConfirm = null;
    public override void Init()
    {
        if (bInitialized)
            return;

        PopupType = PopupType.Modal;
        touchPanel.onClick.AddListener(OnClickTouchPanel);

        cancelButton.onClick.AddListener(OnClickCancelButton);
        confirmButton.onClick.AddListener(OnClickConfirmButton);

        bInitialized = true;

        if (OnInitialized != null)
            OnInitialized();
        OnInitialized = null;
    }

    public UIConfirmPopup Setup(string _title = null, string _desc = null)
    {
        base.Setup();
        if (string.IsNullOrEmpty(_title))
            _title = StringManager.Instance.GetStringText("Name_Notification");

        titleText.text = _title;
        descText.text = _desc;

        descCSF.enabled = true;
        LayoutRebuilder.ForceRebuildLayoutImmediate(descText.rectTransform);
        descText.rectTransform.anchoredPosition = Vector2.zero;
        descCSF.enabled = false;
        if (descText.rectTransform.rect.height < contextPanel.rect.height)
        {
            descText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, contextPanel.rect.height);
            descText.rectTransform.anchoredPosition = Vector2.zero;
        }

        cancelButton.gameObject.SetActive(false);
        confirmButton.gameObject.SetActive(false);

        return this;
    }
    public UIConfirmPopup SetCancel(string _strText = null, Action _onCancel = null)
    {
        if (string.IsNullOrEmpty(_strText))
            _strText = StringManager.Instance.GetStringText("Name_Cancel");
        cancelButtonText.text = _strText;
        OnCancel = _onCancel;
        cancelButton.gameObject.SetActive(true);

        return this;
    }
    public UIConfirmPopup SetConfirm(string _strText = null, Action _onConfirm = null)
    {
        if (string.IsNullOrEmpty(_strText))
            _strText = StringManager.Instance.GetStringText("Name_Confirm");

        confirmButtonText.text = _strText;
        OnConfirm = _onConfirm;
        confirmButton.gameObject.SetActive(true);

        return this;
    }

    public override void OnClickBackButton()
    {
        OnClickCancelButton();
    }

    void OnClickCancelButton()
    {
        if (OnCancel != null)
            OnCancel();
        OnCancel = null;
        PopupManager.Instance.ClosePopup(this);
    }
    void OnClickConfirmButton()
    {
        if (OnConfirm != null)
            OnConfirm();
        OnConfirm = null;
        PopupManager.Instance.ClosePopup(this);
    }
}
