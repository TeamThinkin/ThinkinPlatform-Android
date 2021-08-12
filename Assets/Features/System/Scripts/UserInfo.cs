using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UserInfo
{
    public string UID;
    public string DisplayName;

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
                    Debug.Log("Current user json: " + json);
                    _currentUser = JsonUtility.FromJson<UserInfo>(json);
                }
            }
            return _currentUser;
        }
        set
        {
            if (value != null && value == _currentUser) return;
            _currentUser = value;
            string json = JsonUtility.ToJson(_currentUser);
            Debug.Log("Saving user json: " + json);
            PlayerPrefs.SetString("UserInfo", json);
            OnCurrentUserChanged?.Invoke(_currentUser);
        }
    }

    public UserInfo(string UID, string DisplayName)
    {
        this.UID = UID;
        this.DisplayName = DisplayName;
    }

    public void Logout()
    {
        CurrentUser = null;
    }
}
