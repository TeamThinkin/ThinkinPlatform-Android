using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TabletSpawnArea : XRBaseInteractable
{
    public XRGrabInteractable tabletGrabber;

    [SerializeField] private XRInteractionManager InteractionManager;

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        moveTabletToInteractor(args.interactor);
        InteractionManager.SelectEnter(args.interactor, tabletGrabber);
    }

    private void moveTabletToInteractor(XRBaseInteractor interactor)
    {
        var positionDelta = tabletGrabber.transform.position - tabletGrabber.attachTransform.position;
        var a = tabletGrabber.attachTransform.rotation;
        var b = tabletGrabber.transform.rotation;
        var rotDelta = Quaternion.Inverse(a) * b;

        tabletGrabber.transform.rotation = interactor.attachTransform.rotation * rotDelta;
        tabletGrabber.transform.position = interactor.attachTransform.position + positionDelta;
    }
}
