using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringManager : Singleton<StringManager>
{
    SystemLanguage LanguageType = SystemLanguage.Korean;

    [SerializeField] List<UILocalizeText> localizeTextList = new List<UILocalizeText>();

    public System.Action OnChangeLanguage = null;

    bool bInitialized = false;
    public bool Initialized { get { return bInitialized; } }
    public System.Action OnInitialized = null;

    public void Init()
    {
        if (bInitialized)
            return;

        if (PlayerPrefs.HasKey("LanguageType"))
        {
            LanguageType = (SystemLanguage)PlayerPrefs.GetInt("LanguageType", (int)SystemLanguage.English);
        }
        else
        {
            LanguageType = Application.systemLanguage;
            PlayerPrefs.SetInt("LanguageType", (int)LanguageType);
            PlayerPrefs.Save();
        }

        OnChangeLanguage = null;

        bInitialized = true;

        if (OnInitialized != null)
            OnInitialized();
        OnInitialized = null;
    }

    private void Start()
    {
        if (!bInitialized)
            Init();
    }

    public string GetStringText(string key)
    {
        if (!bInitialized)
            Init();

        return GetStringText(key, LanguageType);
    }
    public string GetStringText(string key, SystemLanguage _languageType)
    {
        if (!bInitialized)
            Init();

        if (string.IsNullOrEmpty(key))
            return key;

        StringData stringData = DataManager.Instance.StringData.GetStringData(key);
        if (stringData == null)
            return key;

        if (_languageType == SystemLanguage.Korean)
            return stringData.Kr;
        else if (_languageType == SystemLanguage.English)
            return stringData.En;
        else if (_languageType == SystemLanguage.Japanese)
            return stringData.Jp;
        else if (_languageType == SystemLanguage.Chinese || _languageType == SystemLanguage.ChineseSimplified || _languageType == SystemLanguage.ChineseTraditional)
            return stringData.Cn;
        else
            return stringData.En;
    }
    public string GetStringText(string key, params string[] values)
    {
        if (!bInitialized)
            Init();

        return GetStringText(key, LanguageType, values);
    }
    public string GetStringText(string key, SystemLanguage _languageType, params string[] values)
    {
        if (!bInitialized)
            Init();

        if (string.IsNullOrEmpty(key))
            return key;

        StringData stringData = DataManager.Instance.StringData.GetStringData(key);
        if (stringData == null)
            return key;

        string localizeText = string.Empty;
        if (_languageType == SystemLanguage.Korean)
            localizeText = stringData.Kr;
        else if (_languageType == SystemLanguage.English)
            localizeText = stringData.En;
        else if (_languageType == SystemLanguage.Japanese)
            localizeText = stringData.Jp;
        else if (_languageType == SystemLanguage.Chinese || _languageType == SystemLanguage.ChineseSimplified || _languageType == SystemLanguage.ChineseTraditional)
            localizeText = stringData.Cn;
        else
            localizeText = stringData.En;

        if (values.Length > 0)
        {
            if (string.IsNullOrEmpty(localizeText) == false)
            {
                if (values.Length == 1) localizeText = string.Format(localizeText, values[0]);
                else if (values.Length == 2) localizeText = string.Format(localizeText, values[0], values[1]);
                else if (values.Length == 3) localizeText = string.Format(localizeText, values[0], values[1], values[2]);
                else if (values.Length == 4) localizeText = string.Format(localizeText, values[0], values[1], values[2], values[3]);
                else if (values.Length == 5) localizeText = string.Format(localizeText, values[0], values[1], values[2], values[3], values[4]);
            }
        }

        if (string.IsNullOrEmpty(localizeText))
            localizeText = key;

        return localizeText;
    }


    public SystemLanguage GetLanguageType()
    {
        if (!bInitialized)
            Init();

        return LanguageType;
    }
    public void ChangeLanguage(SystemLanguage language)
    {
        if (!bInitialized)
            Init();

        LanguageType = language;
        PlayerPrefs.SetInt("LanguageType", (int)LanguageType);
        PlayerPrefs.Save();

        for (int i = 0; i < localizeTextList.Count; i++)
        {
            localizeTextList[i].Refresh();
        }

        if (OnChangeLanguage != null)
            OnChangeLanguage();
    }
    public void AddUILocalizeText(UILocalizeText _uiLocalizeText)
    {
        if (!bInitialized)
            Init();

        localizeTextList.Add(_uiLocalizeText);
    }

    public void ReleaseLocalizeText(UILocalizeText _UILocalizeText)
    {
        if (localizeTextList.Contains(_UILocalizeText))
            localizeTextList.Remove(_UILocalizeText);
    }
    public void ReleaseAllLocalizeText()
    {
        localizeTextList.Clear();
    }
    public void Release()
    {
        localizeTextList.Clear();
        OnChangeLanguage = null;
    }
}
