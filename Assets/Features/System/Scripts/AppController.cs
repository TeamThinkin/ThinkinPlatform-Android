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
    [SerializeField] MonoBehaviour[] _movementControllers;

    [SerializeField] private Camera _mainCamera;
    public override Camera MainCamera => _mainCamera;

    [SerializeField] private DestinationPresenter _destinationPresenter;

    [SerializeField] private Realtime _realtimeNetwork;
    public Realtime RealtimeNetwork => _realtimeNetwork;

    [SerializeField] private Transform _playerTransform;
    public override Transform PlayerTransform => _playerTransform;

    [SerializeField] private Rigidbody _playerBody;
    public override Rigidbody PlayerBody => _playerBody;

    public override string BundleVersionCode => GeneratedInfo.BundleVersionCode;

    private UIManager _uiManager = new UIManager();
    public override IUIManager UIManager => _uiManager;

    [SerializeField] private Keyboard _keyboard;
    public override IKeyboard Keyboard => _keyboard;

    public override bool IsPancake => true;

    void Start()
    {
        UserInfo.OnCurrentUserChanged += UserInfo_OnCurrentUserChanged;
        WebSocketListener.OnSetUser += WebSocketListener_OnSetUser;
        DestinationPresenter.UrlChanged += DestinationPresenter_UrlChanged;
        FocusManager.OnFocusItemChanged += FocusManager_OnFocusItemChanged;

        CoreModule.Initialize();
        PancakeUIModule.Initialize();
        PresenceModule.Initialize();
    }


    private void OnDestroy()
    {
        UserInfo.OnCurrentUserChanged -= UserInfo_OnCurrentUserChanged;
        WebSocketListener.OnSetUser -= WebSocketListener_OnSetUser;
        DestinationPresenter.UrlChanged -= DestinationPresenter_UrlChanged;
        FocusManager.OnFocusItemChanged -= FocusManager_OnFocusItemChanged;
    }

    private void FocusManager_OnFocusItemChanged(IFocusItem item)
    {
        bool movementEnabled = item == null || !(item is Textbox);
        foreach(var controller in _movementControllers)
        {
            controller.enabled = movementEnabled;
        }
    }

    private void DestinationPresenter_UrlChanged(string Url)
    {
        WebSocketListener.Socket.Emit("userLocationChanged", Url);
    }

    private void WebSocketListener_OnSetUser(UserDto obj)
    {
        Debug.Log("App Controller sees that the user has been set (logged in). Currently ignoring it");
        //DeviceRegistrationController.RegisterDevice();
    }

    private void UserInfo_OnCurrentUserChanged(UserInfo obj)
    {
        if(UserInfo.CurrentUser == null) AppSceneManager.LoadLocalScene("Login");
    }

    public override void SetPlayerPosition(Vector3 WorldPosition)
    {
        _playerTransform.position = WorldPosition;
    }

    public override void SetPlayerPosition(Vector3 WorldPosition, Quaternion WorldRotation)
    {
        _playerTransform.rotation = WorldRotation;
        _playerTransform.position = WorldPosition;
    }

    public override void SetPlayerRotation(Quaternion WorldRotation)
    {
        _playerTransform.rotation = WorldRotation;
    }
}
