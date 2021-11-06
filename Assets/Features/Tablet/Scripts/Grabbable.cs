using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Grabbable : XRGrabInteractable
{
    [SerializeField] private Transform LeftAttachTransform;
    [SerializeField] private Transform RightAttachTransform;

    protected override void OnSelectEntering(SelectEnterEventArgs args)
    {
        if (args.interactor.gameObject == LocalAvatarHandController.Left.gameObject)
            attachTransform = LeftAttachTransform;
        else if (args.interactor.gameObject == LocalAvatarHandController.Right.gameObject)
            attachTransform = RightAttachTransform;

        base.OnSelectEntering(args);
    }
}
