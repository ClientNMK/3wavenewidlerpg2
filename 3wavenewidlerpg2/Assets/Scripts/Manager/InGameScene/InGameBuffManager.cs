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

    public void Setup()
    {
        if (defaultStatData != null)
        {
            defaultStatData.Init();
            BuffStatus.SetDefaultStatData(defaultStatData);
        }
    }
}
