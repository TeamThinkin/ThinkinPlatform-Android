using Autohand;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoumeConstraintedPointerGrabbable : MonoBehaviour, IHandlePointerEvent
{
    public ConstraintVolume Volume;

    private Vector3 interactorGrabPoint;
    private UIPointer interactor;

    private void Start()
    {
        if (Volume == null) Volume = GetComponentInParent<ConstraintVolume>();
    }

    public void OnGripStart(UIPointer Sender, RaycastHit RayInfo)
    {
        Debug.Log("Volume grabbable grip start");

        this.interactor = Sender;
        interactorGrabPoint = interactor.transform.InverseTransformPoint(transform.position);
    }

    public void OnGripEnd(UIPointer Sender)
    {
        interactor = null;
    }

    private void Update()
    {
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
        
    }

    #region -- Unused Pointer Events --
    public void OnHoverEnd(UIPointer Sender)
    {
    }

    public void OnHoverStart(UIPointer Sender, RaycastHit RayInfo)
    {
    }

    public void OnTriggerEnd(UIPointer Sender)
    {
    }

    public void OnTriggerStart(UIPointer Sender, RaycastHit RayInfo)
    {
    }
    #endregion
}
