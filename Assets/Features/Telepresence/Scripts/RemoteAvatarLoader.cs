using Normal.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using Wolf3D.ReadyPlayerMe.AvatarSDK;

public class RemoteAvatarLoader : RealtimeComponent<UserInfoModel>
{
    [SerializeField] AvatarManager AvatarManager;

    private void Awake()
    {
        UserInfo.OnCurrentUserChanged += UserInfo_OnCurrentUserChanged;
        WebSocketListener.OnAvatarUrlUpdated += WebSocketListener_OnAvatarUrlUpdated;
    }    

    private void OnDestroy()
    {
        UserInfo.OnCurrentUserChanged -= UserInfo_OnCurrentUserChanged;
        WebSocketListener.OnAvatarUrlUpdated -= WebSocketListener_OnAvatarUrlUpdated;
    }

    private void UserInfo_OnCurrentUserChanged(UserInfo newUser)
    {
        if (isOwnedLocallyInHierarchy) updateModelFromUserInfo(newUser);
    }

    private void WebSocketListener_OnAvatarUrlUpdated(string newAvatarUrl)
    {
        if (isOwnedLocallyInHierarchy) model.avatarUrl = newAvatarUrl;
    }

    private void updateModelFromUserInfo(UserInfo userInfo)
    {
        if (userInfo != null)
        {
            model.displayName = userInfo.DisplayName;
            model.avatarUrl = userInfo.AvatarUrl;
        }
    }

    protected override void OnRealtimeModelReplaced(UserInfoModel previousModel, UserInfoModel currentModel)
    {
        base.OnRealtimeModelReplaced(previousModel, currentModel);
        if (previousModel != null)
        {
            previousModel.avatarUrlDidChange -= CurrentModel_avatarUrlDidChange;
        }

        if (currentModel != null)
        {
            currentModel.avatarUrlDidChange += CurrentModel_avatarUrlDidChange;

            if (currentModel.isOwnedLocallyInHierarchy)
                updateModelFromUserInfo(UserInfo.CurrentUser);

            updateAvatarUrl();
        }
    }

    private void CurrentModel_avatarUrlDidChange(UserInfoModel model, string value)
    {
        updateAvatarUrl();
    }

    private void updateAvatarUrl()
    {
        if (isOwnedLocallyInHierarchy) return; //The local avatar is handled elsewhere

        AvatarManager.LoadAvatarFromUrl(model.avatarUrl);
    }
}
