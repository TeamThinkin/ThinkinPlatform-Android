using Normal.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using Wolf3D.ReadyPlayerMe.AvatarSDK;

public class AvatarManager : MonoBehaviour
{
    private const string defaultAvatarUrl = "https://d1a370nemizbjq.cloudfront.net/4b5de172-a231-4695-a21f-39004feaa54b.glb";

    [SerializeField] private Transform RightHandAnchor;
    [SerializeField] private Transform LeftHandAnchor;
    [SerializeField] private Transform HeadAnchor;
    [SerializeField] private MouthMoveBlendShape MouthMover;

    private string loadedAvatarUrl;
    private GameObject loadedAvatar;
    private Transform neckBone;
    private Transform headBone;
    private Vector3 neckHeadOffset;

    private static AvatarLoader loader = new AvatarLoader();

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
        LoadAvatarFromUrl(newUser.AvatarUrl);
    }

    private void WebSocketListener_OnAvatarUrlUpdated(string newAvatarUrl)
    {
        LoadAvatarFromUrl(newAvatarUrl);
    }

    public void LoadAvatarFromUrl(string avatarUrl)
    {
        if (string.IsNullOrEmpty(avatarUrl)) avatarUrl = defaultAvatarUrl;

        if (avatarUrl == loadedAvatarUrl) return;
        loadedAvatarUrl = avatarUrl;

        loader.LoadAvatar(avatarUrl, (avatar, metadata) =>
        {
            if (loadedAvatar != null) Destroy(loadedAvatar);
            loadedAvatar = avatar;
            avatar.name = "Local User Avatar";
            avatar.transform.SetParent(this.transform);
            avatar.transform.localPosition = new Vector3(0, 0, 0);
            avatar.transform.rotation = Quaternion.Euler(0, 0, 0);
            addConstraints(avatar);
        });
    }

    private void LateUpdate()
    {
        if (neckBone != null && headBone != null)
        {
            neckBone.position = headBone.position + neckHeadOffset;
            neckBone.rotation = Quaternion.Slerp(neckBone.rotation, headBone.rotation, 0.5f * Time.deltaTime); //TODO: Limit non vertical axis rotation so cocking your head to the side doesnt rotate torso
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
            renderer.localBounds = new Bounds(Vector3.zero, Vector3.one * 50f); //TODO: This just makes the local bounds huge so that it wont be culled, but needs a better fix
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
