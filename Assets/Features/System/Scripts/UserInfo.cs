using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UserInfo
{
    public string UID;
    public string DisplayName;
    public string AvatarUrl;

    public static event Action<UserInfo> OnCurrentUserChanged;

    private static UserInfo _currentUser;
    public static UserInfo CurrentUser
    {
        get
        {
            if(_currentUser == null)
            {
                if(PlayerPrefs.HasKey("UserInfo"))
                {
                    var json = PlayerPrefs.GetString("UserInfo");
                    _currentUser = JsonUtility.FromJson<UserInfo>(json);
                }
            }
            return _currentUser;
        }
        set
        {
            if (value != null && value == _currentUser) return;
            _currentUser = value;
            SaveCurrentUser();
        }
    }

    public static void SaveCurrentUser()
    {
        string json = JsonUtility.ToJson(_currentUser);
        PlayerPrefs.SetString("UserInfo", json);
        OnCurrentUserChanged?.Invoke(_currentUser);
    }

    public void Logout()
    {
        CurrentUser = null;
    }
}
