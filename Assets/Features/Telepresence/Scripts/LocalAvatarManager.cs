using Normal.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LocalAvatarManager : MonoBehaviour, IProvideHandData
{
    //private const string defaultAvatarUrl = "https://d1a370nemizbjq.cloudfront.net/4b5de172-a231-4695-a21f-39004feaa54b.glb";
    public static LocalAvatarManager Instance { get; private set; }

    [SerializeField] private Transform RightHandAnchor;
    [SerializeField] private Transform LeftHandAnchor;
    [SerializeField] private Transform HeadAnchor;
    [SerializeField] private GameObject DefaultAvatar;
    [SerializeField] private LocalAvatarHandController LeftAvatarHand;
    [SerializeField] private LocalAvatarHandController RightAvatarHand;

    private XRIDefaultInputActions inputActions;
    private SkinController currentSkin;
    private bool isCurrentSkinDefault;
    private string currentAvatarUrl;

    public SkinController CurrentSkin => currentSkin;

    private void Awake()
    {
        Instance = this;
        inputActions = new XRIDefaultInputActions();
        inputActions.XRIRightHand.GripStrength.Enable();
        inputActions.XRIRightHand.IsFingerOnTrigger.Enable();
        inputActions.XRILeftHand.GripStrength.Enable();
        inputActions.XRILeftHand.IsFingerOnTrigger.Enable();

        LeftAvatarHand.SetInputActions(inputActions.XRILeftHand.IsFingerOnTrigger, inputActions.XRILeftHand.GripStrength);
        RightAvatarHand.SetInputActions(inputActions.XRIRightHand.IsFingerOnTrigger, inputActions.XRIRightHand.GripStrength);

        UserInfo.OnCurrentUserChanged += UserInfo_OnCurrentUserChanged;
    }

    private void OnDestroy()
    {
        UserInfo.OnCurrentUserChanged -= UserInfo_OnCurrentUserChanged;
    }

    private void UserInfo_OnCurrentUserChanged(UserInfo newUser)
    {
        updateSkin();
    }

    private void destroyCurrentSkin()
    {
        if (currentSkin == null) return;
        Destroy(currentSkin.gameObject);
        currentSkin = null;
        currentAvatarUrl = null;
    }

    private async void updateSkin()
    {
        if (UserInfo.CurrentUser != UserInfo.UnknownUser)
        {
            if (currentAvatarUrl == UserInfo.CurrentUser.AvatarUrl) return;
            destroyCurrentSkin();

            isCurrentSkinDefault = false;
            currentAvatarUrl = UserInfo.CurrentUser.AvatarUrl;
            currentSkin = await SkinController.CreateSkin(true, UserInfo.CurrentUser.AvatarUrl, HeadAnchor, LeftHandAnchor, RightHandAnchor, LeftAvatarHand, RightAvatarHand);
        }
        else
        {
            if (isCurrentSkinDefault) return;
            destroyCurrentSkin();

            isCurrentSkinDefault = true;
            currentAvatarUrl = null;
            currentSkin = Instantiate(DefaultAvatar).GetComponent<SkinController>();
            currentSkin.SetSourceData(true, HeadAnchor, LeftHandAnchor, RightHandAnchor, LeftAvatarHand, RightAvatarHand);
            Debug.Log("Instantiated default skin");
        }
    }

    //private void updateHandData()
    //{
    //    bool isFingerOnTrigger;

    //    isFingerOnTrigger = inputActions.XRIRightHand.IsFingerOnTrigger.ReadValue<float>() > 0;
    //    leftHandData.GripStrength = inputActions.XRIRightHand.GripStrength.ReadValue<float>();
    //    leftHandData.IsPointing = !isFingerOnTrigger && leftHandData.GripStrength < 0.1f;

    //    isFingerOnTrigger = inputActions.XRIRightHand.IsFingerOnTrigger.ReadValue<float>() > 0;
    //    rightHandData.GripStrength = inputActions.XRIRightHand.GripStrength.ReadValue<float>();
    //    rightHandData.IsPointing = !isFingerOnTrigger && leftHandData.GripStrength < 0.1f;
    //}

    public AvatarHandData GetHandData()
    {
        return LeftAvatarHand.HandData;
    }

    public AvatarHandData GetRightHandData()
    {
        return RightAvatarHand.HandData;
    }
}