using Normal.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using Wolf3D.ReadyPlayerMe.AvatarSDK;

public class RemoteAvatarLoader : RealtimeComponent<UserInfoModel>
{
    private const string defaultAvatarUrl = "https://d1a370nemizbjq.cloudfront.net/4b5de172-a231-4695-a21f-39004feaa54b.glb";

    [SerializeField] private Transform RightHandAnchor;
    [SerializeField] private Transform LeftHandAnchor;
    [SerializeField] private Transform HeadAnchor;
    [SerializeField] private MouthMoveBlendShape MouthMover;

    private string loadedAvatarUrl;
    private GameObject loadedAvatar;

    private static AvatarLoader loader = new AvatarLoader();

    private void Awake()
    {
        UserInfo.OnCurrentUserChanged += UserInfo_OnCurrentUserChanged;
    }

    private void OnDestroy()
    {
        UserInfo.OnCurrentUserChanged -= UserInfo_OnCurrentUserChanged;
    }

    private void UserInfo_OnCurrentUserChanged(UserInfo newUser)
    {
        if (isOwnedLocallyInHierarchy)
        {
            updateModelFromUserInfo(newUser);
        }
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
        if (string.IsNullOrEmpty(model.avatarUrl)) model.avatarUrl = defaultAvatarUrl;

        if (model.avatarUrl == loadedAvatarUrl) return;
        loadedAvatarUrl = model.avatarUrl;

        Debug.Log("Loading avatar: " + model.avatarUrl);
        loader.LoadAvatar(model.avatarUrl, (avatar, metadata) =>
        {
            Debug.Log("Avatar Loaded");
            if (loadedAvatar != null) Destroy(loadedAvatar);
            loadedAvatar = avatar;
            avatar.transform.SetParent(this.transform);
            avatar.transform.localPosition = new Vector3(0, 0, 0);
            avatar.transform.rotation = Quaternion.Euler(0, 0, 0);
            addConstraints(avatar);
        });
    }

    Transform neckBone;
    Transform headBone;
    Vector3 neckHeadOffset;

    private void LateUpdate()
    {
        if (neckBone != null && headBone != null)
        {
            neckBone.position = headBone.position + neckHeadOffset;
            neckBone.rotation = Quaternion.Slerp(neckBone.rotation, headBone.rotation, 0.5f * Time.deltaTime);
        }
    }

    private void addConstraints(GameObject avatar)
    {
        neckBone = avatar.transform.Find("Armature/Hips/Spine/Neck");
        headBone = avatar.transform.Find("Armature/Hips/Spine/Neck/Head");
        neckHeadOffset = neckBone.position - headBone.position;

        MouthMover.Mesh = avatar.transform.Find("Wolf3D.Avatar_Renderer_Head").gameObject.GetComponent<SkinnedMeshRenderer>();

        var renderers = avatar.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (var renderer in renderers)
        {
            renderer.localBounds = new Bounds(Vector3.zero, Vector3.one * 50f); //TODO: fix culling issue
        }

        addParentConstraint(avatar, "Armature/Hips/Spine/RightHand", RightHandAnchor);
        addParentConstraint(avatar, "Armature/Hips/Spine/LeftHand", LeftHandAnchor);
        addParentConstraint(avatar, "Armature/Hips/Spine/Neck/Head", HeadAnchor);
    }

    private void addParentConstraint(GameObject avatar, string bonePath, Transform anchor)
    {
        var bone = avatar.transform.Find(bonePath);
        var constraint = bone.gameObject.AddComponent<ParentConstraint>();
        var constraintList = new List<ConstraintSource>();
        constraintList.Add(new ConstraintSource() { sourceTransform = anchor, weight = 1 });
        constraint.SetSources(constraintList);
        constraint.constraintActive = true;
    }
}
