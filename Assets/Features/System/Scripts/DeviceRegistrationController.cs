using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public static class DeviceRegistrationController
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

    public static async void CheckDeviceRegistration()
    {
        await RegisterDevice();

        if (UserInfo.CurrentUser != UserInfo.UnknownUser)
            RoomManager.Instance.LoadUrl(UserInfo.CurrentUser.HomeRoomUrl);
        else
            AppSceneManager.Instance.LoadLocalScene("Login");
    }

    public static async Task RegisterDevice()
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
            UserInfo.CurrentUser = user;
        }
        else
        {
            UserInfo.CurrentUser = UserInfo.UnknownUser;
        }
    }

}
