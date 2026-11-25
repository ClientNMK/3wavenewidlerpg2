using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScene : UIBase
{
    virtual protected void Awake()
    {
        PopupManager.Instance.SetCanvas(this, false);
    }
}