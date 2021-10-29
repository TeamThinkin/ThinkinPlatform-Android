using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class AppController : MonoBehaviour
{
    void Start()
    {
        UserInfo.OnCurrentUserChanged += UserInfo_OnCurrentUserChanged;
        WebSocketListener.OnSetUser += WebSocketListener_OnSetUser;
        DeviceRegistrationController.CheckDeviceRegistration();
    }

    private void OnDestroy()
    {
        UserInfo.OnCurrentUserChanged -= UserInfo_OnCurrentUserChanged;
        WebSocketListener.OnSetUser -= WebSocketListener_OnSetUser;
    }

    private void WebSocketListener_OnSetUser(UserDto obj)
    {
        Debug.Log("App Controller sees that the user has been set (logged in)");
        DeviceRegistrationController.RegisterDevice();
    }

    private void UserInfo_OnCurrentUserChanged(UserInfo obj)
    {
        if(UserInfo.CurrentUser == null) AppSceneManager.Instance.LoadLocalScene("Login");
    }
}
