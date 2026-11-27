using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 처음 접속(닉네임 없음) 때 닉네임 입력 (NightmareIdle UINicknamePopup 기준). 정할 때까지 닫히지 않음
// 차트를 불러오기 전이라 글자는 로컬 문자열(LocalStringChart.csv)에서 읽음
public class UINicknamePopup : UIPopup
{
    static readonly Regex ConsonantOnlyRegex = new Regex("^[ㄱ-ㅎ]+$");
    const int MinLength = 2;
    const int MaxLength = 8;

    [SerializeField] TMP_InputField inputField = null;
    [SerializeField] Button confirmButton = null;

    public Action OnComplete = null;

    public override void Init()
    {
        if (bInitialized)
            return;

        PopupType = PopupType.Modal;
        inputField.text = string.Empty;
        inputField.characterLimit = MaxLength;
        confirmButton.onClick.AddListener(OnClickConfirmButton);
        bInitialized = true;
    }

    // 닉네임을 정해야 하므로 바깥 터치·뒤로 가기로 닫지 않음
    public override void OnClickTouchPanel() { }
    public override void OnClickBackButton() { }

    void OnClickConfirmButton()
    {
        string nickname = Regex.Replace(inputField.text, @"[^0-9a-zA-Z가-힣]", "");
        if (string.IsNullOrEmpty(nickname) || ConsonantOnlyRegex.IsMatch(nickname) || GameUtils.GameUtils.CheckNumberAndSpecialChar(nickname))
        {
            ShowToast("Msg_NotCorrectNickname");
            return;
        }
        if (nickname.Length < MinLength || nickname.Length > MaxLength)
        {
            ShowToast("Msg_NotCorrectNickname_2");
            return;
        }
        string checkNickname = nickname.ToLower();
        if (checkNickname.Contains("gm") || checkNickname.Contains("운영자") || BackendManager.Instance.FilterManager.IsFilteredString(checkNickname))
        {
            ShowToast("Msg_NotCorrectNickname");
            return;
        }

        confirmButton.interactable = false;
        BackendManager.Instance.BackendLogin.UpdateNickname(nickname, HandleUpdateNicknameComplete);
    }

    void HandleUpdateNicknameComplete(BackEnd.BackendReturnObject _callback, string _nickname)
    {
        confirmButton.interactable = true;
        if (!_callback.IsSuccess())
        {
            ShowToast(_callback.GetStatusCode().Equals("409") ? "Msg_DuplicateNickname" : "Msg_CreateNickNameRule");
            return;
        }
        PopupManager.Instance.ClosePopup(this);
        OnComplete?.Invoke();
    }

    void ShowToast(string _key)
    {
        GameManager.Instance.UISystemScene.ShowToastMessagePanel(StringManager.Instance.GetStringText(_key));
    }
}
