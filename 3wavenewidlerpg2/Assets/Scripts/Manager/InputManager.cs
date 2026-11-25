using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    public bool bCantClose = false;
    [SerializeField] UIEffect_Touch touchEffect = null;

    private void Start()
    {
        bCantClose = false;
    }

    void Update()
    {
        //if (Input.GetMouseButton(0))
        //{
        //    if (touchEffect != null)
        //        touchEffect.ShowEffect(Input.mousePosition);
        //}
        if (Input.GetMouseButtonDown(0))
        {
            if (touchEffect != null)
                touchEffect.ShowEffect(Input.mousePosition);

            SoundManager.Instance.PlaySFX(SFXPlayType.UI, SFXType.Click);
        }

//#if UNITY_ANDROID
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (bCantClose)
                return;

            if (GameManager.Instance.gameType == GameType.Title)
            {
                if (PopupManager.Instance.OpenedPopupList.Count > 0)
                {
                    PopupManager.Instance.OnClickBackButton();
                }
                else
                {
                    UIConfirmPopup popup = PopupManager.Instance.ShowPopup<UIConfirmPopup>(null, GameManager.Instance.SystemCanvas.transform);
                    if (popup != null)
                    {
                        string title = StringManager.Instance.GetStringText("Name_Quit");
                        string context = StringManager.Instance.GetStringText("Msg_Quit");
                        popup.Setup(title, context)
                            .SetConfirm(title, () =>
                            {
                                GameManager.Instance.QuitGame();
                            })
                            .SetCancel(null, null);
                    }
                }
            }
            else if (GameManager.Instance.gameType == GameType.InGame)
            {
                if (PopupManager.Instance.OpenedPopupList.Count > 0)
                {
                    PopupManager.Instance.OnClickBackButton();
                }
                else
                {
                    UIConfirmPopup popup = PopupManager.Instance.ShowPopup<UIConfirmPopup>(null, GameManager.Instance.SystemCanvas.transform);
                    if (popup != null)
                    {
                        popup.Setup(null, StringManager.Instance.GetStringText("Msg_GameOff"))
                            .SetConfirm(null, GameManager.Instance.QuitGame)
                            .SetCancel(null, null);
                    }
                }
            }

        }
//#endif
    }
}
