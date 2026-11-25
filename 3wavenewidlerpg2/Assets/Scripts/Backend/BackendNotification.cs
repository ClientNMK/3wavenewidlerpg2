using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using BackEnd;
using BackEnd.Socketio;
using LitJson;

public class BackendNotification
{
    public Action OnNewNoticeCreated = null;
    public Action OnServerStatusChanged = null;
    public Action OnNewPostCreated = null;
    public Action OnReceivedGuildApplicant = null;
    public Action OnApprovedGuildJoin = null;
    public Action OnRejectedGuildJoin = null;
    public Action OnGuildMemberExpelled = null;

    bool isConnect = false;
    public bool IsConnect { get { return isConnect; } }
    public void Init()
    {
        isConnect = false;

        OnNewNoticeCreated = null;
        OnNewPostCreated = null;
        OnServerStatusChanged = null;
        OnReceivedGuildApplicant = null;
        OnApprovedGuildJoin = null;
        OnRejectedGuildJoin = null;
        OnGuildMemberExpelled = null;
    }

    public void Setup()
    {
        Init();
        EventHandler();

        if (isConnect == false)
            Connect();
    }

    public void Connect()
    {
        if (isConnect)
            return;

        // 실시간 알림 서버로 연결
        Backend.Notification.Connect();
    }

    public void DisConnect()
    {
        if (isConnect == false)
            return;

        // 실시간 알림 서버 접속을 해제한다
        Backend.Notification.DisConnect();
    }

    public string GetTempNotice()
    {
        string tempNotice = Backend.Notice.GetTempNotice();
        if (string.IsNullOrEmpty(tempNotice))
            return tempNotice;

        JsonData data = JsonMapper.ToObject(tempNotice);
        bool isUse = bool.Parse(data["isUse"].ToString());
        if (isUse)
            tempNotice = data["contents"].ToString();
        else
            tempNotice = string.Empty;

        return tempNotice;
    }

    void EventHandler()
    {
        // 접속 시 반응하는 핸들러 설정
        Backend.Notification.OnAuthorize = (bool result, string reason) =>
        {
            Debug.Log("실시간 서버 성공 여부 : " + result);
            if (result)
                isConnect = true;
            else
                Debug.Log("실패 시 이유 : " + reason);
        };

        // 접속 해제 시 반응하는 핸들러를 설정.  
        Backend.Notification.OnDisConnect = (string Reason) =>
        {
            Debug.Log("해제 이유 : " + Reason);
            isConnect = false;
        };

        Backend.Notification.OnNewNoticeCreated = (string title, string content) =>
        {
            Debug.Log(
                $"[OnNewNoticeCreated(새로운 공지사항 생성)]\n" +
                $"| title : {title}\n" +
                $"| content : {content}\n"
            );
            OnNewNoticeCreated?.Invoke();
        };

        Backend.Notification.OnNewEventCreated = (string title, string content) =>
        {
            Debug.Log(
                $"[OnNewEventCreated(새로운 이벤트 생성)]\n" +
                $"| title : {title}\n" +
                $"| content : {content}\n"
            );
        };

        Backend.Notification.OnServerStatusChanged = (ServerStatusType serverStatusType) =>
        {
            Debug.Log(
                $"[OnServerStatusChanged(서버 상태 변경)]\n" +
                $"| ServerStatusType : {serverStatusType}\n"
            );
            BackendManager.Instance.MainThreadQueue.Enqueue(OnServerStatusChanged);
        };

        Backend.Notification.OnNewPostCreated = (PostRepeatType postRepeatType, string title, string content, string author) =>
        {
            Debug.Log(
                $"[OnNewPostCreated(새로운 우편 생성)]\n" +
                $"| postRepeatType : {postRepeatType}\n" +
                $"| title : {title}\n" +
                $"| content : {content}\n" +
                $"| author : {author}\n"
            );
        };

        Backend.Notification.OnReceivedGuildApplicant = () =>
        {
            Debug.Log("새 길드 가입 신청이 도착했습니다!");

            BackendManager.Instance.MainThreadQueue.Enqueue(() =>
            {
                OnReceivedGuildApplicant?.Invoke();
            });
        };

        Backend.Notification.OnApprovedGuildJoin = () =>
        {
            Debug.Log("길드 가입 신청이 수락되었습니다!");
            BackendManager.Instance.MainThreadQueue.Enqueue(OnApprovedGuildJoin);
        };

        Backend.Notification.OnRejectedGuildJoin = () =>
        {
            Debug.Log("길드 가입 신청이 거절당했습니다..");
            BackendManager.Instance.MainThreadQueue.Enqueue(OnRejectedGuildJoin);
        };
        Backend.Notification.OnGuildMemberExpelled = () =>
        {
            Debug.Log("길드에서 추방당했습니다..");
            BackendManager.Instance.MainThreadQueue.Enqueue(OnGuildMemberExpelled);
        };
    }

    public void Release()
    {
        DisConnect();
        Init();
    }

    public void OnDestroy()
    {
        Release();
    }

    public void OnApplicationQuit()
    {
        Release();
    }
}
