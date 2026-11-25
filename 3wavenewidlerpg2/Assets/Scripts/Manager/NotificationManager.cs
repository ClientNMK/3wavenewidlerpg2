using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class NotificationManager : Singleton<NotificationManager>
{
    public Action OnNewNoticeCreated = null;
    public Action OnNewPostCreated = null;
    public Action OnServerStatusChanged = null;
    public Action OnReceivedGuildApplicant = null;
    public Action OnApprovedGuildJoin = null;
    public Action OnRejectedGuildJoin = null;
    public Action OnGuildMemberExpelled = null;

    BackendNotification backendNotification = null;

    bool bInitialized = false;
    public void Init()
    {
        if (bInitialized)
            return;

        OnNewNoticeCreated = null;
        OnNewPostCreated = null;
        OnServerStatusChanged = null;
        OnReceivedGuildApplicant = null;
        OnApprovedGuildJoin = null;
        OnRejectedGuildJoin = null;
        OnGuildMemberExpelled = null;

        bInitialized = true;
    }
    public void Setup()
    {
        if (!bInitialized)
            Init();

        if (backendNotification == null)
        {
            backendNotification = BackendManager.Instance.BackendNotification;
            backendNotification.Setup();
            EventHandler();
        }
    }

    void EventHandler()
    {
        backendNotification.OnNewNoticeCreated = HandleNewNoticeCreated;
        backendNotification.OnNewPostCreated = HandleNewPostCreated;
        backendNotification.OnServerStatusChanged = HandleServerStatusChanged;
        backendNotification.OnReceivedGuildApplicant = HandleReceivedGuildApplicant;
        backendNotification.OnApprovedGuildJoin = HandleApprovedGuildJoin;
        backendNotification.OnRejectedGuildJoin = HandleRejectedGuildJoin;
        backendNotification.OnGuildMemberExpelled = HandleGuildMemberExpelled;
    }

    void HandleNewNoticeCreated() { }
    void HandleNewPostCreated()
    {
        BackendManager.Instance.MainThreadQueue.Enqueue(OnNewPostCreated);
    }
    void HandleServerStatusChanged()
    {
        // BackendManager.Instance.BackendError.ShowErrPopup(StringManager.Instance.GetStringText("Msg_Maintenance"));
    }
    void HandleReceivedGuildApplicant()
    {
        OnReceivedGuildApplicant?.Invoke();
    }
    void HandleApprovedGuildJoin() { }
    void HandleRejectedGuildJoin() { }
    void HandleGuildMemberExpelled() { }
    public void Release()
    {
        backendNotification = null;

        OnNewNoticeCreated = null;
        OnNewPostCreated = null;
        OnServerStatusChanged = null;
        OnReceivedGuildApplicant = null;
        OnApprovedGuildJoin = null;
        OnRejectedGuildJoin = null;
        OnGuildMemberExpelled = null;

        bInitialized = false;
    }
}
