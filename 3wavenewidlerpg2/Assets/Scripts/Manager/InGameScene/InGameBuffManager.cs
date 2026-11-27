using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameBuffManager : MonoBehaviour
{
    static InGameBuffManager _instance = null;
    static public InGameBuffManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
    }

    public DefaultStatData defaultStatData;
    public MainCharacterStatus BuffStatus = new();
    public Dictionary<int, int> GoldUpgradeLevels = new();    // Gold_Id별 레벨 (저장 전까지 이번 플레이만)
    public Dictionary<int, int> StatPointLevels = new();      // StatPoint_Id별 레벨 (저장 전까지 이번 플레이만)

    public void Setup()
    {
        if (defaultStatData != null)
        {
            defaultStatData.Init();
            BuffStatus.SetDefaultStatData(defaultStatData);
        }
    }

    public void SetGoldUpgradeLevel(int _goldId, int _level)
    {
        GoldUpgradeLevels[_goldId] = _level;
        BuffStatus.SetGoldUpgradeStatData(DataManager.Instance.GoldUpgradeData.GetGoldUpgradeData(_goldId, _level), _level);
    }

    public void SetStatPointLevel(int _statPointId, int _level)
    {
        StatPointLevels[_statPointId] = _level;
        BuffStatus.SetStatPointStatData(DataManager.Instance.StatPointData.GetStatPointData(_statPointId, _level), _level);
    }
}
