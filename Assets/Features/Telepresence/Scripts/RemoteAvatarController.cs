using Normal.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RemoteAvatarController : RealtimeComponent<UserInfoModel>
{
    [SerializeField] private Transform HeadTransform;
    [SerializeField] private Transform RightHandTransform;
    [SerializeField] private Transform LeftHandTransform;
    [SerializeField] private TMP_Text DisplayNameLabel;
    [SerializeField] private MouthMoveBlendShape MouthMover;
    [SerializeField] private RemoteAvatarHandController LeftHand;
    [SerializeField] private RemoteAvatarHandController RightHand;

    private SkinController currentSkin;

    protected override void OnRealtimeModelReplaced(UserInfoModel previousModel, UserInfoModel currentModel)
    {
        base.OnRealtimeModelReplaced(previousModel, currentModel);
        if (previousModel != null)
        {
            previousModel.avatarUrlDidChange -= CurrentModel_avatarUrlDidChange;
        }

        if (currentModel != null)
        {
            if (currentModel.isOwnedLocallyInHierarchy)
            {
                //Local avatar
                gameObject.name = "Remote Avatar (Local)";
                UserInfo.OnCurrentUserChanged += UserInfo_OnCurrentUserChanged;
                updateModelFromUserInfo();
                linkMouthMover();
            }
            else
            {
                //Remote avatar
                gameObject.name = "Remote Avatar (Remote)";
                currentModel.avatarUrlDidChange += CurrentModel_avatarUrlDidChange;
                updateSkin(currentModel.avatarUrl);
            }          

            currentModel.displayNameDidChange += CurrentModel_displayNameDidChange;
            updateDisplayName();
        }
    }
    
    private void updateModelFromUserInfo()
    {
        model.avatarUrl = UserInfo.CurrentUser.AvatarUrl;
        model.displayName = UserInfo.CurrentUser.DisplayName;
    }

    private void UserInfo_OnCurrentUserChanged(UserInfo obj)
    {
        if (!isOwnedLocallyInHierarchy) return;
        updateModelFromUserInfo();
    }

    private void OnDestroy()
    {
        if(model != null)
        {
            model.displayNameDidChange -= CurrentModel_displayNameDidChange;

            if (model.isOwnedLocallyInHierarchy)
            {
                UserInfo.OnCurrentUserChanged -= UserInfo_OnCurrentUserChanged;
            }
        }
    }

    private void CurrentModel_avatarUrlDidChange(UserInfoModel model, string value)
    {
        if (isOwnedLocallyInHierarchy) return; //Local skins are handled by the LocalAvatarManager
        updateSkin(model.avatarUrl);
    }

    private void CurrentModel_displayNameDidChange(UserInfoModel model, string value)
    {
        updateDisplayName();
    }

    private void updateDisplayName()
    {
        DisplayNameLabel.text = model.displayName;
    }

    private void linkMouthMover() //Note: this assumes the skin model has been loaded, and as that is an async process there may be problems if this called before it completes
    {
        if(isOwnedLocallyInHierarchy)
            MouthMover.Mesh = LocalAvatarManager.Instance.CurrentSkin.GetMouthRenderer();
        else
            MouthMover.Mesh = currentSkin.GetMouthRenderer();
    }

    private async void updateSkin(string avatarUrl)
    {
        if (isOwnedLocallyInHierarchy) return; //Local skins are handled by the LocalAvatarManager

        if(currentSkin != null)
        {
            Destroy(currentSkin.gameObject);
            currentSkin = null;
        }

        currentSkin = await SkinController.CreateSkin(false, avatarUrl, HeadTransform, LeftHandTransform, RightHandTransform, LeftHand, RightHand);
        linkMouthMover();
    }
}
