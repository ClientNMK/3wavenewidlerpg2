using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotation : MonoBehaviour
{
    void LateUpdate()
    {
        if (Input.acceleration.y > 0.5f)
        {
            if (Screen.orientation == ScreenOrientation.LandscapeRight)
            {
                Screen.orientation = ScreenOrientation.LandscapeLeft;
            }
            else
            {
                Screen.orientation = ScreenOrientation.LandscapeRight;
            }
        }
    }
}
