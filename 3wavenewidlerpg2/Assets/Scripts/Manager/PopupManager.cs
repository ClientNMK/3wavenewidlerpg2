using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PopupManager : Singleton<PopupManager>
{
    [SerializeField] int orderIndex = 0;
    Camera uiCamera = null;
    public Camera UICamera
    {
        get
        {
            // 레이드(Additive 씬) 중에는 InGame 카메라가 비활성이므로 활성 씬의 카메라를 다시 찾는다.
            if (uiCamera == null || !uiCamera.gameObject.activeInHierarchy)
                uiCamera = GameObject.FindWithTag("UICamera").GetComponent<Camera>();
            //if (uiCamera == null)
            //    uiCamera = Camera.main;
            return uiCamera;
        }
    }

    public List<UIPopup> OpenedPopupList = new List<UIPopup>();
    [SerializeField] List<UIPopup> closedPopupList = new List<UIPopup>();

    public Action OnClosePopupAction = null;
    public void SetCanvas(UIBase _uiBase, bool _isSort = true)
    {
        Canvas canvas = GameUtils.GameUtils.GetOrAddComponent<Canvas>(_uiBase.gameObject);
        if (canvas == null)
            return;

        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.overrideSorting = _isSort ? true : false;
        canvas.worldCamera = UICamera;
        canvas.sortingLayerName = "UI";
        canvas.sortingOrder = _isSort ? 100 + orderIndex++ : 0;
    }

    public T ShowPopup<T>(string _popupName = null, Transform _parent = null) where T : UIPopup
    {
        if (string.IsNullOrEmpty(_popupName))
            _popupName = typeof(T).Name;

        OpenedPopupList.RemoveAll(x => x == null);
        var popup2 = OpenedPopupList.Find(x => x.PopupName == _popupName);
        if (popup2 != null)
            return null;

        _parent = _parent != null ? _parent : Root.transform;

        T popup = GetPopup<T>(_popupName);
        if (popup != null)
        {
            if (popup.transform.parent != _parent)
            {
                popup.transform.SetParent(_parent);
                popup.transform.localScale = Vector3.one;
                var rect = popup.GetComponent<RectTransform>();
                if (rect != null)
                {
                    rect.offsetMin = Vector2.zero;
                    rect.offsetMax = Vector2.zero;
                }
            }

            closedPopupList.Remove(popup);
            OpenedPopupList.Add(popup);
            popup.ShowPopup();
            if (OpenedPopupList.Count > 0)
                SoundManager.Instance.SetSFX(SFXPlayType.InGame, false);
            return popup as T;
        }

        if (popup == null)
        {
            if (OpenedPopupList.Contains(popup))
                return null;

            GameObject obj = Resources.Load<GameObject>("Prefabs/Popup/" + _popupName);
            if (obj != null)
            {
                popup = Instantiate(obj, _parent).GetComponent<T>();
                popup.PopupName = _popupName;
                popup.name = _popupName;
            }
        }
        else
        {
            if (OpenedPopupList.Contains(popup))
                return null;

            popup.transform.SetParent(_parent);
            // popup.transform.SetAsFirstSibling();
            closedPopupList.Remove(popup);
        }
        OpenedPopupList.Add(popup);
        popup.ShowPopup();
        if (OpenedPopupList.Count > 0)
            SoundManager.Instance.SetSFX(SFXPlayType.InGame, false);
        return popup;
    }

    public void ClosePopup()
    {
        int lastIdx = OpenedPopupList.Count - 1;
        if (lastIdx < 0)
            return;

        UIPopup popup = OpenedPopupList[lastIdx];
        if (popup != null)
            ClosePopup(popup);
    }
    public void ClosePopup(string _popupName)
    {
        UIPopup popup = GetPopup<UIPopup>(_popupName);
        if (popup != null)
            ClosePopup(popup);
    }
    public void ClosePopup(UIPopup _popup)
    {
        if (!GameUtils.GameUtils.IsNull(_popup))
        {
            if (_popup.IsNotClosing)
                return;

            _popup.ClosePopup();
            if (OpenedPopupList.Contains(_popup))
                OpenedPopupList.Remove(_popup);

            if (closedPopupList.Contains(_popup) == false)
                closedPopupList.Add(_popup);

            if (_popup.IsSort)
                orderIndex--;
        }

        OnClosePopupAction?.Invoke();
        if (OpenedPopupList.Count < 1)
            SoundManager.Instance.SetSFX(SFXPlayType.InGame, true);

    }
    public void CloseAllPopup()
    {
        var arrPopups = OpenedPopupList.ToArray();
        foreach (var popup in arrPopups)
        {
            if (popup.PopupName == "UIInGameOfflineRewardPopup" || popup.PopupName == "UIInGameEventGamePopup")
                continue;
            // zi f (popup.PopupName)
            ClosePopup(popup);
        }
        orderIndex = OpenedPopupList.Count <= 0 ? 0 : OpenedPopupList.Count;
        //SoundManager.Instance.SetSFX(SFXPlayType.InGame, true);
        // OnClosePopupAction?.Invoke();
    }

    public void OnClickBackButton()
    {
        int lastIdx = OpenedPopupList.Count - 1;
        if (lastIdx < 0)
            return;

        UIPopup popup = OpenedPopupList[lastIdx];
        if (popup != null)
            popup.OnClickBackButton();
    }

    public T GetPopup<T>(string _popupName = null) where T : UIPopup
    {
        if (string.IsNullOrEmpty(_popupName))
            _popupName = typeof(T).Name;

        // 레이드 씬 언로드로 파괴된 팝업 정리.
        closedPopupList.RemoveAll(x => x == null);
        UIPopup popup = closedPopupList.Find(x => x.PopupName.Equals(_popupName));
        if (popup == null)
            return null;

        return popup as T;
    }
    public T GetOpenedPopup<T>(string _popupName = null) where T : UIPopup
    {
        if (string.IsNullOrEmpty(_popupName))
            _popupName = typeof(T).Name;

        UIPopup popup = OpenedPopupList.Find(x => x.PopupName.Equals(_popupName));
        if (popup == null)
            return null;

        return popup as T;
    }

    public void ReleaseAllPopup()
    {
        CloseAllPopup();

        var arrPopups = closedPopupList.ToArray();
        foreach (var popup in arrPopups)
        {
            if (GameUtils.GameUtils.IsNull(popup))
                continue;

            if (GameUtils.GameUtils.IsFakeNull(popup))
                continue;

            Destroy(popup);
        }

        OpenedPopupList.Clear();
        closedPopupList.Clear();
        orderIndex = 0;

        OnClosePopupAction = null;
    }


    private GameObject _root;
    private GameObject Root
    {
        get
        {
            if (_root != null && _root.activeInHierarchy)
                return _root;

            _root = GameObject.Find("SubCanvas");
            if (_root == null)
                _root = new GameObject { name = "SubCanvas" };

            return _root;
        }
    }

}
