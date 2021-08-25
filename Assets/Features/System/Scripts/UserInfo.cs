using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UserInfo
{
    public string Id;
    public string DisplayName;
    public string AvatarUrl;

    public static event Action<UserInfo> OnCurrentUserChanged;

    private static UserInfo _currentUser;
    public static UserInfo CurrentUser
    {
        get { return _currentUser; }
        set
        {
            if (_currentUser == value) return;
            _currentUser = value;
            OnCurrentUserChanged?.Invoke(_currentUser);
        }
    }
}
