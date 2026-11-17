using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLauncher : IStartLaunch
{
    public void StartLaunch()
    {
        PlatformUtility.PlatformAnalysis.Init();
    }
}
