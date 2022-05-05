using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HandSideEnum
{
    Left,
    Right
}

public class LocalAvatarHandController : MonoBehaviour, IProvideHandData
{
    //[SerializeField] private XRRayInteractor RayInteractor;
    //[SerializeField] private XRInteractorLineVisual RayVisualizer;
    [SerializeField] private HandSideEnum HandSide;

    private AvatarHandData handData;

    public AvatarHandData HandData => handData;
    //private InputAction isFingerOnTriggerInputAction;
    //private InputAction gripStrengthInputAction;

    public static LocalAvatarHandController Left { get; private set; }
    public static LocalAvatarHandController Right { get; private set; }

    private void Awake()
    {
        switch(HandSide)
        {
            case HandSideEnum.Left:
                Left = this;
                break;
            case HandSideEnum.Right:
                Right = this;
                break;
        }
    }

    //public void SetInputActions(InputAction isFingerOnTriggerInputAction, InputAction gripStrengthInputAction)
    //{
    //    this.isFingerOnTriggerInputAction = isFingerOnTriggerInputAction;
    //    this.gripStrengthInputAction = gripStrengthInputAction;
    //}

    //private void Update()
    //{
    //    updateHandData();
    //    RayVisualizer.enabled = handData.IsPointing;
    //}

    //private void updateHandData()
    //{
    //    bool isFingerOnTrigger = isFingerOnTriggerInputAction.ReadValue<float>() > 0;
    //    handData.GripStrength = gripStrengthInputAction.ReadValue<float>();
    //    handData.IsPointing = !isFingerOnTrigger && handData.GripStrength < 0.1f;
    //    handData.RayLength = getRayLength();       
    //}

    //private float getRayLength()
    //{
    //    Vector3 rayHitPosition, normal;
    //    int positionInLine;
    //    bool isValidTarget;

    //    if (RayInteractor.TryGetHitInfo(out rayHitPosition, out normal, out positionInLine, out isValidTarget))
    //        return Vector3.Distance(RayInteractor.transform.position, rayHitPosition);
    //    else
    //        return 10;
    //}

    public AvatarHandData GetHandData()
    {
        return handData;
    }
}
