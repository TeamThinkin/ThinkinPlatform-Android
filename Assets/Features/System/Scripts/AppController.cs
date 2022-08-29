using Firesplash.UnityAssets.SocketIO;
using Normal.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class AppController : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;
    public Camera MainCamera => _mainCamera;

    [SerializeField] private DestinationPresenter _destinationPresenter;

    [SerializeField] private Realtime _realtimeNetwork;
    public Realtime RealtimeNetwork => _realtimeNetwork;

    [SerializeField] private GameObject _contentSymbolPrefab;

    public static GameObject ContentSymbolPrefab { get; private set; }

    public static AppController Instance { get; private set; }

    void Start()
    {
        //PlayerPrefs.DeleteAll();

        Instance = this;

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
}
