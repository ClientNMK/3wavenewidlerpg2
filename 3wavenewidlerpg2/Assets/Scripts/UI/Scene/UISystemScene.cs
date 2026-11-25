using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISystemScene : UIScene
{
    [SerializeField] UISystemLoadingPanel loadingPanel = null;
    [SerializeField] UISystemSmallLoadingPanel smallLoadingPanel = null;
    [SerializeField] UIToastMessagePanel toastPanel = null;
    [SerializeField] UITopNoticeMessagePanel topNoticePanel = null;
    protected override void Awake()
    {
        loadingPanel.gameObject.SetActive(false);
        toastPanel.gameObject.SetActive(false);
        topNoticePanel.gameObject.SetActive(false);
    }

    public bool IsShowLoadingPanel => loadingPanel.gameObject.activeSelf;

    public void ShowLoadingPanel()
    {
        GameManager.Instance.InputManager.bCantClose = true;
        loadingPanel.ShowPanel();
        // 이전 CloseLoadingPanel에서 알파가 0으로 남아 있으므로 복구(레이드처럼 씬 로드 대기가 있는 경우 가려지지 않음).
        loadingPanel.SetResourcesAlpha(1f);
    }
    public void CloseLoadingPanel(float _delayTime, System.Action _start = null ,System.Action _complete = null)
    {
        loadingPanel.CloseLoadingPanel(()=>
        {
            _start?.Invoke();
        },
        _delayTime, ()=>
        {
            _complete?.Invoke();
            GameManager.Instance.InputManager.bCantClose = false;
        });
    }

    public void ShowSmallLoadingPanel()
    {
        GameManager.Instance.InputManager.bCantClose = true;
        smallLoadingPanel.ShowPanel();
    }
    public void CloseSmallLoadingPanel()
    {
        GameManager.Instance.InputManager.bCantClose = false;
        smallLoadingPanel.ClosePanel();
    }

    public void ShowToastMessagePanel(string message)
    {
        toastPanel.ShowPanel();
        toastPanel.ShowMessage(message);
    }
    public void ShowTopNoticeMessagePanel()
    {
        topNoticePanel.ShowPanel();
        topNoticePanel.ShowMessage();
        SoundManager.Instance.PlaySFX(SFXPlayType.UI, SFXType.UI_Guide);
    }
    public void CloseToastMessagePanel()
    {
        toastPanel.ClosePanel();
    }
}
