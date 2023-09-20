using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boot : MonoBehaviour
{
    public int fps = 60;
    private void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = fps;
    }

    private void Update()
    {
        if (Application.targetFrameRate != fps)
            Application.targetFrameRate = fps;
    }
}
