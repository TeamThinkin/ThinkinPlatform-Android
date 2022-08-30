using Firesplash.UnityAssets.SocketIO;
using Normal.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class AppController : AppControllerBase
{
    [SerializeField] private Autohand.AutoHandPlayer _autoHandPlayer;

    [SerializeField] private Camera _mainCamera;
    public override Camera MainCamera => _mainCamera;

    [SerializeField] private DestinationPresenter _destinationPresenter;

    [SerializeField] private Realtime _realtimeNetwork;
    public Realtime RealtimeNetwork => _realtimeNetwork;

    [SerializeField] private GameObject _contentSymbolPrefab;

    public override Transform PlayerTransform => _autoHandPlayer.transform;

    public override Rigidbody PlayerBody => _autoHandPlayer.body;

    public static GameObject ContentSymbolPrefab { get; private set; }

    void Start()
    {
        //PlayerPrefs.DeleteAll();
        ContentSymbolPrefab = _contentSymbolPrefab;
        
        UserInfo.OnCurrentUserChanged += UserInfo_OnCurrentUserChanged;
        WebSocketListener.OnSetUser += WebSocketListener_OnSetUser;
        DestinationPresenter.UrlChanged += DestinationPresenter_UrlChanged;

        DeviceRegistrationController.CheckDeviceRegistration();
    }

    private void OnDestroy()
    {
        UserInfo.OnCurrentUserChanged -= UserInfo_OnCurrentUserChanged;
        WebSocketListener.OnSetUser -= WebSocketListener_OnSetUser;
        DestinationPresenter.UrlChanged -= DestinationPresenter_UrlChanged;
    }

    private void DestinationPresenter_UrlChanged(string Url)
    {
        WebSocketListener.Socket.Emit("userLocationChanged", Url);
    }

    private void WebSocketListener_OnSetUser(UserDto obj)
    {
        Debug.Log("App Controller sees that the user has been set (logged in)");
        DeviceRegistrationController.RegisterDevice();
    }

    private void UserInfo_OnCurrentUserChanged(UserInfo obj)
    {
        if(UserInfo.CurrentUser == null) AppSceneManager.LoadLocalScene("Login");
    }

    public override void SetPlayerPosition(Vector3 WorldPosition)
    {
        _autoHandPlayer.SetPosition(WorldPosition);
    }

    public override void SetPlayerPosition(Vector3 WorldPosition, Quaternion WorldRotation)
    {
        _autoHandPlayer.SetPosition(WorldPosition, WorldRotation);
    }

    public override void SetPlayerRotation(Quaternion WorldRotation)
    {
        _autoHandPlayer.SetRotation(WorldRotation);
    }
}
