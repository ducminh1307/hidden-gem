using System;
using UnityEngine;

public class TargetFrameRate : MonoBehaviour
{
    [Obsolete("Obsolete")]
    private void Awake()
    {
        var resolution = Screen.currentResolution;
        
        #if UNITY_EDITOR
            Screen.SetResolution(resolution.width, resolution.height, true, 1000);
            Application.targetFrameRate = 1000;
        #else
            Screen.SetResolution(resolution.width, resolution.height, true, 120);
            Application.targetFrameRate = 120;
        #endif
    }
}