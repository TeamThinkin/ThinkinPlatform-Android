using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ConstrainedGrabbleBehavior : InteractableBehavior
{
    public ConstraintVolume Volume;

    private Vector3 interactorGrabPoint;
    private XRBaseInteractor interactor;

    private void Start()
    {
        if (Volume == null) Volume = GetComponentInParent<ConstraintVolume>();
    }

    public override void OnSelectEntering(SelectEnterEventArgs args)
    {
        base.OnSelectEntering(args);
        grab(args.interactor);
    }

    public override void OnSelectExiting(SelectExitEventArgs args)
    {
        base.OnSelectExiting(args);
        drop();
    }

    private void grab(XRBaseInteractor interactor)
    {
        this.interactor = interactor;
        interactorGrabPoint = interactor.transform.InverseTransformPoint(transform.position);
    }

    private void drop()
    {
        interactor = null;
    }

    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        base.ProcessInteractable(updatePhase);

        switch (updatePhase)
        {
            case XRInteractionUpdateOrder.UpdatePhase.Dynamic:
                if (interactor != null)
                {
                    var position = interactor.transform.TransformPoint(interactorGrabPoint); //world space
                    position = Volume.VolumeReference.InverseTransformPoint(position); //volume space
                    position.x = Mathf.Clamp(position.x, -0.5f, 0.5f);
                    position.y = Mathf.Clamp(position.y, -0.5f, 0.5f);
                    position.z = Mathf.Clamp(position.z, -0.5f, 0.5f);
                    position = Volume.VolumeReference.TransformPoint(position); //world space
                    transform.position = position;
                }
                break;
        }
    }
}