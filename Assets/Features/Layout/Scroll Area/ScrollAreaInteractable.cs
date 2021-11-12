using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ScrollAreaInteractable : XRBaseInteractable
{
    [SerializeField] private bool AllowXMovement = true;
    [SerializeField] private bool AllowYMovement;

    private ScrollArea scrollArea;

    private Vector3 referencePoint;
    private Vector3 lastReferencePoint;
    private MomentumVector3 dragDirection = new MomentumVector3(0.05f);
    private Plane referencePlane = new Plane();
    private Ray ray = new Ray();
    private XRBaseInteractor interactor;
    private bool isDragging;

    override protected void Awake()
    {
        scrollArea = GetComponent<ScrollArea>();
    }

    private void Update()
    {
        if (isDragging)
        {
            referencePoint = getReferencePoint();
            var point = transform.InverseTransformDirection(referencePoint - lastReferencePoint);
            if (!AllowXMovement) point.x = 0;
            if (!AllowYMovement) point.y = 0;
            dragDirection.Set(point);
            
            lastReferencePoint = referencePoint;
        }
        else
        {
            dragDirection.Update();
        }

        scrollArea.OffsetScrollPosition(dragDirection.Value);
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        interactor = args.interactor;
        lastReferencePoint = getReferencePoint();
        dragDirection.Value = Vector3.zero;
        isDragging = true;
    }

    private Vector3 getReferencePoint()
    {
        referencePlane.SetNormalAndPosition(transform.forward, transform.position);
        ray.origin = interactor.transform.position;
        ray.direction = interactor.transform.forward;
        return referencePlane.GetRaycastPoint(ray);
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        isDragging = false;
    }
}
