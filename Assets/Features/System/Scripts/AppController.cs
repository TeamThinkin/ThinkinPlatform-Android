using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySocketIO.Events;

public class AppController : MonoBehaviour
{
    private static string _uid;
    public static string UID
    {
        get
        {
            if (string.IsNullOrEmpty(_uid))
            {
                if (PlayerPrefs.HasKey("deviceUID"))
                {
                    _uid = PlayerPrefs.GetString("deviceUID");
                }
                else
                {
                    _uid = Guid.NewGuid().ToString();
                    PlayerPrefs.SetString("deviceUID", _uid);
                }
            }

            return _uid;
        }
    }

    void Start()
    {
        UserInfo.OnCurrentUserChanged += UserInfo_OnCurrentUserChanged;
        registerDevice();        
    }


    private void OnDestroy()
    {
        UserInfo.OnCurrentUserChanged -= UserInfo_OnCurrentUserChanged;
    }

    private async void registerDevice()
    {
        var userDto = await WebAPI.RegisterDevice(UID);
        if(userDto != null)
        {
            UserInfo.CurrentUser = new UserInfo()
            {
                Id = userDto.Id,
                AvatarUrl = userDto.AvatarUrl,
                DisplayName = userDto.DisplayName
            };

            AppSceneManager.Instance.LoadLocalScene("Home Room");
        }
        else
        {
            AppSceneManager.Instance.LoadLocalScene("Login");
        }
    }

    private void UserInfo_OnCurrentUserChanged(UserInfo obj)
    {
        if(UserInfo.CurrentUser == null) AppSceneManager.Instance.LoadLocalScene("Login");
    }

    public void GoToHomeRoom()
    {
        AppSceneManager.Instance.LoadLocalScene("Home Room");
    }
}
