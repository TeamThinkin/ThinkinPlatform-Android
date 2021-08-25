using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySocketIO.Events;

public class WebSocketListener : MonoBehaviour
{
    public static event System.Action OnSocketConnected;
    public static event System.Action<UserDto> OnSetUser;
    public static event System.Action<string> OnAvatarUrlUpdated;

    private SocketIOController socket;

    void Start()
    {
        socket = GetComponent<SocketIOController>();
        socket.On("connect", onSocketConnected);
        socket.On("avatarUrlUpdated", onAvatarUrlUpdatedReceived);
        socket.On("setUser", onSetUserReceived);
    }

    private void OnDestroy()
    {
        socket.Off("connect", onSocketConnected);
        socket.Off("avatarUrlUpdated", onAvatarUrlUpdatedReceived);
        socket.Off("setUser", onSetUserReceived);
    }

    private void onSocketConnected(SocketIOEvent e)
    {
        OnSocketConnected?.Invoke();
    }

    private void onAvatarUrlUpdatedReceived(SocketIOEvent e)
    {
        if (OnAvatarUrlUpdated == null) return;
        var url = e.data.StripQuotes();
        OnAvatarUrlUpdated(url);
    }

    private void onSetUserReceived(SocketIOEvent e)
    {
        if (OnSetUser == null) return;
        var dto = JsonConvert.DeserializeObject<UserDto>(e.data);
        OnSetUser(dto);
    }
}
