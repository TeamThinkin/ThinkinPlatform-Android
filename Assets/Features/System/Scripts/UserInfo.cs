using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UserInfo
{
    public string UID { get; private set; }
    public string DisplayName { get; private set; }

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
            if (value == _currentUser) return;
            _currentUser = value;
            PlayerPrefs.SetString("UserInfo", JsonUtility.ToJson(_currentUser));
            OnCurrentUserChanged?.Invoke(_currentUser);
        }
    }
}
