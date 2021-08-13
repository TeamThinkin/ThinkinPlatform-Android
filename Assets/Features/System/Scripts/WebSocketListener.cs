using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySocketIO.Events;

public class WebSocketListener : MonoBehaviour
{
    private SocketIOController socket;

    void Start()
    {
        socket = GetComponent<SocketIOController>();
        socket.On("avatarUrlUpdated", onAvatarUrlUpdated);
    }

    private void OnDestroy()
    {
        socket.Off("avatarUrlUpdated", onAvatarUrlUpdated);
    }

    private void onAvatarUrlUpdated(SocketIOEvent e)
    {
        var url = e.data.StripQuotes();
        Debug.Log("Avatar Url updated: " + url);
        if(UserInfo.CurrentUser != null)
        {
            UserInfo.CurrentUser.AvatarUrl = url;
            UserInfo.SaveCurrentUser();
        }
    }
}
