using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Animations;
using Wolf3D.ReadyPlayerMe.AvatarSDK;

public interface IProvideHandData
{
    AvatarHandData GetLeftHandData();
    AvatarHandData GetRightHandData();
}

public struct AvatarHandData
{
    public bool IsPointing;
    public float GripStrength;
}

public class SkinController : MonoBehaviour
{
    private static AvatarLoader loader = new AvatarLoader();

    [SerializeField] private bool _isDefaultSkin;
    public bool IsDefaultSkin => _isDefaultSkin;

    private Transform headTransform;
    private Transform leftHandTransform;
    private Transform rightHandTransform;
    private IProvideHandData handDataProvider;
    private Transform neckBone;
    private Transform headBone;
    private Vector3 neckHeadOffset;
    private AvatarHandData leftHandData;
    private AvatarHandData rightHandData;
    private Animator rightHandAnimator;
    private Animator leftHandAnimator;
    
    public static async Task<SkinController> CreateSkin(bool IsLocal, string AvatarUrl, Transform HeadTransform, Transform LeftHandTransform, Transform RightHandTransform, IProvideHandData HandDataProvider)
    {
        var avatar = await loadAvatarFromUrl(AvatarUrl);
        var skinController = avatar.AddComponent<SkinController>();
        skinController.SetSourceData(IsLocal, HeadTransform, LeftHandTransform, RightHandTransform, HandDataProvider);
        return skinController;
    }

    private void Update()
    {
        if (handDataProvider != null)
        {
            leftHandData = handDataProvider.GetLeftHandData();
            rightHandData = handDataProvider.GetRightHandData();

            rightHandAnimator.SetFloat("Grip", rightHandData.GripStrength);
            rightHandAnimator.SetBool("Is Pointing", rightHandData.IsPointing);
            leftHandAnimator.SetFloat("Grip", leftHandData.GripStrength);
            leftHandAnimator.SetBool("Is Pointing", leftHandData.IsPointing);
        }
    }

    private void LateUpdate()
    {
        if (neckBone != null && headBone != null)
        {
            neckBone.position = headBone.position + neckHeadOffset;
            neckBone.rotation = Quaternion.Slerp(neckBone.rotation, headBone.rotation, 0.5f * Time.deltaTime); //TODO: Limit non vertical axis rotation so cocking your head to the side doesnt rotate torso
        }
    }

    public void SetSourceData(bool IsLocal, Transform HeadTransform, Transform LeftHandTransform, Transform RightHandTransform, IProvideHandData HandDataProvider)
    {
        this.headTransform = HeadTransform;
        this.leftHandTransform = LeftHandTransform;
        this.rightHandTransform = RightHandTransform;
        this.handDataProvider = HandDataProvider;

        if(IsLocal) patchRendererBounds(gameObject);
        addBoneConstraints(gameObject);
        addAnimationController(gameObject);
    }

    public SkinnedMeshRenderer GetMouthRenderer()
    {
        return gameObject.transform.Find("Wolf3D.Avatar_Renderer_Head").gameObject.GetComponent<SkinnedMeshRenderer>();
    }


    private static async Task<GameObject> loadAvatarFromUrl(string avatarUrl)
    {
        bool isLoaded = false;
        GameObject loadedSkin = null;

        loader.LoadAvatar(avatarUrl, (avatar, metadata) =>
        {
            isLoaded = true;
            loadedSkin = avatar;
        });

        await Task.Run(() =>
        {
            while(!isLoaded) { }
        });

        loadedSkin.name = "Skin";

        return loadedSkin;
    }


    private void patchRendererBounds(GameObject skin)
    {
        var renderers = skin.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (var renderer in renderers)
        {
            renderer.localBounds = new Bounds(Vector3.zero, Vector3.one * 50f); //TODO: This just makes the local bounds huge so that it wont be culled, but needs a better fix
        }
    }

    private void addBoneConstraints(GameObject skin)
    {
        neckBone = skin.transform.Find("Armature/Hips/Spine/Neck");
        headBone = skin.transform.Find("Armature/Hips/Spine/Neck/Head");
        neckHeadOffset = neckBone.position - headBone.position;

        addParentConstraint(skin, "Armature/Hips/Spine/RightHand", rightHandTransform);
        addParentConstraint(skin, "Armature/Hips/Spine/LeftHand", leftHandTransform);
        addParentConstraint(skin, "Armature/Hips/Spine/Neck/Head", headTransform);
    }

    private void addAnimationController(GameObject skin)
    {
        var rightHand = skin.transform.Find("Armature/Hips/Spine/RightHand").gameObject;
        rightHandAnimator = rightHand.AddComponent<Animator>();
        rightHandAnimator.runtimeAnimatorController = Instantiate(Resources.Load<RuntimeAnimatorController>("Animation/Avatar Right Hand"));

        var leftHand = skin.transform.Find("Armature/Hips/Spine/LeftHand").gameObject;
        leftHandAnimator = leftHand.AddComponent<Animator>();
        leftHandAnimator.runtimeAnimatorController = Instantiate(Resources.Load<RuntimeAnimatorController>("Animation/Avatar Left Hand"));
    }

    private void addParentConstraint(GameObject skin, string bonePath, Transform anchor)
    {
        var bone = skin.transform.Find(bonePath);
        var constraint = bone.gameObject.AddComponent<ParentConstraint>();
        var constraintList = new List<ConstraintSource>();
        constraintList.Add(new ConstraintSource() { sourceTransform = anchor, weight = 1 });
        constraint.SetSources(constraintList);
        constraint.constraintActive = true;
    }
}