using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using BackEnd;
using LitJson;

public class BackendGameData : MonoBehaviour
{
    public void GetMyData(string _tableName, Action<BackendReturnObject> _onComplete = null)
    {
        GameUtils.GameUtils.Debug_Log("GetMyData");
        SendQueue.Enqueue(Backend.GameData.GetMyData, _tableName, new Where(), 1, callback =>
        {
            if (callback.IsSuccess())
            {
            }

            if (_onComplete != null)
                _onComplete(callback);
        });
    }
    public void GetData(string _tableName, string _ownerInDate, Action<BackendReturnObject> _onComplete = null)
    {
        Where where = new Where();
        where.Equal("owner_inDate", _ownerInDate);

        GameUtils.GameUtils.Debug_Log("GetData");
        SendQueue.Enqueue(Backend.GameData.Get, _tableName, where, 1, callback =>
        {
            if (_onComplete != null)
                _onComplete(callback);
        });
    }
    public void GetData(string _tableName, string _ownerInDate, string[] _select, Action<BackendReturnObject> _onComplete = null)
    {
        Where where = new Where();
        where.Equal("owner_inDate", _ownerInDate);

        GameUtils.GameUtils.Debug_Log("GetData");
        SendQueue.Enqueue(Backend.GameData.Get, _tableName, where, _select, 1, callback =>
        {
            if (_onComplete != null)
                _onComplete(callback);
        });
    }
    public void GetOtherData(string _tableName, string _ownerInDate, string[] _select, Action<BackendReturnObject> _onComplete = null)
    {
        SendQueue.Enqueue(Backend.PlayerData.GetOtherData, _tableName, _ownerInDate, _select, callback =>
        {
            if (_onComplete != null)
                _onComplete(callback);
        });
    }
    public void InsertGameData(string _tableName, Param _param, Action<BackendReturnObject> _onComplete = null)
    {
        GameUtils.GameUtils.Debug_Log("Insert");
        SendQueue.Enqueue(Backend.GameData.Insert, _tableName, _param, callback =>
        {
            if (callback.IsSuccess())
            {

            }

            if (_onComplete != null)
                _onComplete(callback);
        });
    }
    public void UpdateGameData(string _tableName, string _inDate, string _ownerInDate, Param _param, Action<BackendReturnObject> _onComplete = null)
    {
        GameUtils.GameUtils.Debug_Log("Update");
        SendQueue.Enqueue(Backend.GameData.UpdateV2, _tableName, _inDate, _ownerInDate, _param, callback =>
        {
            if (callback.IsSuccess())
            {

            }

            if (_onComplete != null)
                _onComplete(callback);
        });
    }
    public void UpdateGameData(string _tableName, Where _where, Param _param, Action<BackendReturnObject> _onComplete = null)
    {
        GameUtils.GameUtils.Debug_Log("Update");
        if (_where == null) _where = new Where();
        SendQueue.Enqueue(Backend.GameData.Update, _tableName, _where, _param, callback =>
        {
            if (callback.IsSuccess())
            {

            }

            if (_onComplete != null)
                _onComplete(callback);
        });
    }

    public void TransactionRead(List<TransactionValue> _transactionList, Action<BackendReturnObject> _onComplete = null)
    {
        GameUtils.GameUtils.Debug_Log("TransactionRead");
        Backend.GameData.TransactionReadV2(_transactionList, (callback) =>
        {
            // 이후 처리
            if (callback.IsSuccess())
            {

            }

            if (_onComplete != null)
                _onComplete(callback);
        });
    }
    public void TransactionWrite(List<TransactionValue> _transactionList, Action<BackendReturnObject> _onComplete = null)
    {
        GameUtils.GameUtils.Debug_Log("TransactionWrite");
        SendQueue.Enqueue(Backend.GameData.TransactionWriteV2, _transactionList.ToArray(), (callback) =>
        {
            // 이후 처리
            if (callback.IsSuccess())
            {

            }
            else
                Debug.Log(callback.ToString());
            if (_onComplete != null)
                _onComplete(callback);
        });
    }
   
}
