using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using BackEnd;
using LitJson;

public enum UpdateType
{
    Optional = 1,
    Force = 2,
}

public enum ServerStatus
{
    Online = 0,
    Offline = 1,
    Maintenance = 2,
}

public class BackendUtils
{
    public ServerStatus ServerStatus = ServerStatus.Online;
    public string ServerVersion = "0.0.1";
    public UpdateType UpdateType = UpdateType.Force;
    public string ServerTime = string.Empty;

    public void GetServerStatus(Action complete = null)
    {
        Backend.Utils.GetServerStatus(callback =>
        {
            if (callback.IsSuccess())
            {
                string serverStatus = callback.GetReturnValuetoJSON()["serverStatus"].ToString();
                if (serverStatus.Equals("0")) ServerStatus = ServerStatus.Online;
                else if (serverStatus.Equals("1")) ServerStatus = ServerStatus.Offline;
                else if (serverStatus.Equals("2")) ServerStatus = ServerStatus.Maintenance;
            }
            else
            {
                ServerStatus = ServerStatus.Offline;
            }

            if (complete != null)
                complete();
            complete = null;
        });
    }

    public void GetLatestVersion(Action complete = null)
    {
        if (Application.platform == RuntimePlatform.WindowsEditor ||
            Application.platform == RuntimePlatform.LinuxEditor ||
            Application.platform == RuntimePlatform.OSXEditor)
        {
            ServerVersion = Application.version;
            UpdateType = UpdateType.Optional;

            if (complete != null)
                complete();
            complete = null;
            return;
        }

        Backend.Utils.GetLatestVersion(callback =>
        {
            if (callback.IsSuccess())
            {
                ServerVersion = callback.GetReturnValuetoJSON()["version"].ToString();
                string forceUpdate = callback.GetReturnValuetoJSON()["type"].ToString();
                if (forceUpdate.Equals("1")) UpdateType = UpdateType.Optional;
                else if (forceUpdate.Equals("2")) UpdateType = UpdateType.Force;
            }

            if (complete != null)
                complete();
            complete = null;
        });
    }

    public void GetServerTime(Action<BackendReturnObject> complete = null)
    {
        Backend.Utils.GetServerTime(callback =>
        {
            if (callback.IsSuccess())
            {
                ServerTime = callback.GetReturnValuetoJSON()["utcTime"].ToString();
            }

            if (complete != null)
                complete(callback);
            complete = null;
        });
    }

    public void InsertLog(string _logName, Param param, Action<BackendReturnObject> complate = null)
    {
        SendQueue.Enqueue(Backend.GameLog.InsertLogV2, _logName, param, (callback) =>
        {
            if (callback.IsSuccess() == false)
                Debug.LogError(callback);

            if (complate != null)
                complate(callback);
        });
    }
}
