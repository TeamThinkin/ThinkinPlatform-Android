using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncTelepresenceRoom : MonoBehaviour
{
    [SerializeField] private Normal.Realtime.Realtime Normcore;

    private void Start()
    {
        AppSceneManager.OnEnvironmentLoaded += AppSceneManager_OnEnvironmentLoaded;
        AppSceneManager.OnEnvironmentUnloaded += AppSceneManager_OnEnvironmentUnloaded;
    }

    private void OnDestroy()
    {
        AppSceneManager.OnEnvironmentLoaded -= AppSceneManager_OnEnvironmentLoaded;
        AppSceneManager.OnEnvironmentUnloaded -= AppSceneManager_OnEnvironmentUnloaded;
    }

    private void AppSceneManager_OnEnvironmentLoaded()
    {
        if (AppSceneManager.CurrentRoom == null) return;
        
        Normcore.Connect(AppSceneManager.CurrentRoom.Id);
        Debug.Log("Normcore connect to: " + AppSceneManager.CurrentRoom.Id);
    }

    private void AppSceneManager_OnEnvironmentUnloaded()
    {
        Debug.Log("Disconnect normcore");
        Normcore.Disconnect();
    }
}
