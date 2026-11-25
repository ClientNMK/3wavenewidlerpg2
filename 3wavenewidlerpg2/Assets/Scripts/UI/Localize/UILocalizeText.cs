using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum StringType
{
    ServerChart = 1,
    LocalChart,
}
public class UILocalizeText : MonoBehaviour
{
    [SerializeField] TMP_Text localizeText = null;

    // [SerializeField] StringType stringType = StringType.ServerChart;
    [SerializeField] string localizeKey = null;
    [HideInInspector] public string LocalizeKey { get { return localizeKey; } }

    StringManager stringManager = null;
    private void Start()
    {
        if (localizeText == null)
            localizeText = GameUtils.GameUtils.GetOrAddComponent<TMP_Text>(this.gameObject);

        stringManager = StringManager.Instance;
        stringManager.AddUILocalizeText(this);
        SetStringToUseLocalizeKey(localizeKey);
    }

    public void SetLocalizeKey(string key)
    {
        localizeKey = key;
    }

    public void SetStringToUseLocalizeKey(string key)
    {
        localizeKey = key;
        if (stringManager != null)
            localizeText.text = stringManager.GetStringText(localizeKey);
    }

    public void Refresh()
    {
        if (stringManager != null)
            localizeText.text = stringManager.GetStringText(localizeKey);

    }

    private void OnDestroy()
    {
        if (stringManager != null)
            stringManager.ReleaseLocalizeText(this);
    }
}
