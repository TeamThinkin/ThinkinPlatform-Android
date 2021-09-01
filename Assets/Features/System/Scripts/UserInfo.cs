using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInfo
{
    public string Id;
    public string DisplayName;
    public string AvatarUrl;

    public DomainInfo[] Domains;

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

public class DomainInfo
{
    public string Id;
    public string DisplayName;
    public string ManifestUrl;
    public RoomInfo[] Rooms;

    public static DomainInfo FromDomainDto(DomainDto dto)
    {
        return new DomainInfo()
        {
            Id = dto.Id,
            DisplayName = dto.DisplayName,
            ManifestUrl = dto.ManifestUrl
        };
    }
}

public class RoomInfo
{
    public string Id;
    public string DisplayName;
    public string EnvironmentUrl;
    public DomainInfo ParentDomain;

    public static RoomInfo FromRoomDto(RoomDto dto, DomainInfo ParentDomain)
    {
        return new RoomInfo()
        {
            Id = dto.Id,
            DisplayName = dto.DisplayName,
            EnvironmentUrl = dto.EnvironmentUrl,
            ParentDomain = ParentDomain
        };
    }
}