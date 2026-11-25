using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICheckBoxButton : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] Image checkImage = null;

    public void SetActiveCheckImage(bool _active)
    {
        if (IsActivate != _active)
            checkImage.gameObject.SetActive(_active);
    }

    public Button Button { get { return button; } }
    public bool IsActivate { get { return checkImage.gameObject.activeSelf; } }
}
