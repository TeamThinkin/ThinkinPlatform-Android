using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncTelepresenceRoom : MonoBehaviour
{
    [SerializeField] private Normal.Realtime.Realtime Normcore;

    private void Start()
    {
        RoomManager.OnRoomLoaded += RoomManager_OnRoomLoaded;
        RoomManager.OnRoomUnloaded += RoomManager_OnRoomUnloaded;
    }

    private void OnDestroy()
    {
        RoomManager.OnRoomLoaded -= RoomManager_OnRoomLoaded;
        RoomManager.OnRoomUnloaded -= RoomManager_OnRoomUnloaded;
    }

    private void RoomManager_OnRoomLoaded()
    {
        if (!enabled) return;
        if (RoomManager.CurrentRoomId == null) return;

        Normcore.Connect(RoomManager.CurrentRoomId.ToString());
    }

    private void RoomManager_OnRoomUnloaded()
    {
        if (!enabled) return;
        Normcore.Disconnect();
    }
}
