using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

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
        WebSocketListener.OnSetUser += WebSocketListener_OnSetUser;
        checkDeviceRegistration();
    }

    

    private void OnDestroy()
    {
        UserInfo.OnCurrentUserChanged -= UserInfo_OnCurrentUserChanged;
        WebSocketListener.OnSetUser -= WebSocketListener_OnSetUser;
    }

    private async void checkDeviceRegistration()
    {
        await registerDevice();
        
        if (UserInfo.CurrentUser != null)
            AppSceneManager.Instance.LoadUrl(UserInfo.CurrentUser.HomeRoomUrl);
        else
            AppSceneManager.Instance.LoadLocalScene("Login");
    }

    private async Task registerDevice()
    {
        var userDto = await WebAPI.RegisterDevice(UID);
        if (userDto != null)
        {
            var user = new UserInfo()
            {
                Id = userDto.Id,
                AvatarUrl = userDto.AvatarUrl,
                DisplayName = userDto.DisplayName,
                AuthToken = userDto.Token,
                HomeRoomUrl = userDto.HomeRoomUrl
            };

            //Fetch Domain Info and structure into models
            //if (userDto.Domains != null)
            //{
            //    var manifests = await Task.WhenAll(userDto?.Domains?.Select(i => WebAPI.GetManifest(i.ManifestUrl, user.AuthToken)));

            //    user.Domains = new DomainInfo[userDto.Domains.Length];
            //    for (int i = 0; i < user.Domains.Length; i++)
            //    {
            //        var domain = DomainInfo.FromDomainDto(userDto.Domains[i]);
            //        user.Domains[i] = domain;
            //        user.Domains[i].Rooms = manifests[i].Select(i => RoomInfo.FromRoomDto(i, domain)).ToArray();
            //    }
            //}

            UserInfo.CurrentUser = user;
        }
        else UserInfo.CurrentUser = null;
    }

    private void WebSocketListener_OnSetUser(UserDto obj)
    {
        registerDevice();
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
