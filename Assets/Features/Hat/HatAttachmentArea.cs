using Autohand;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HatAttachmentArea : MonoBehaviour
{
    [SerializeField] private Transform AttachPoint; 

    private Grabbable hoverItem;
    private Grabbable attachedHat;

    private void OnTriggerEnter(Collider other)
    {
        var grabbable = other.GetComponentInParent<Grabbable>();
        if (grabbable != null && grabbable.IsHeld())
        {
            grabbable.onRelease.AddListener(hoverItemReleased);
            hoverItem = grabbable;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var grabbable = other.GetComponentInParent<Grabbable>();
        if (grabbable != null && grabbable == hoverItem)
        {
            hoverItem.onRelease.RemoveListener(hoverItemReleased);
        }
    }

    private void hoverItemReleased(Hand hand, Grabbable grabbable)
    {
        attachHat(hoverItem);
    }

    private void attachHat(Grabbable grabbable)
    {
        Debug.Log("Attaching hat!");
        attachedHat = hoverItem;
        attachedHat.body.isKinematic = true;
        attachedHat.onGrab.AddListener(attachedHatGrabbed);
        //attachedHat.onRelease.AddListener(attachedHatReleased);
    }

    private void detachHat()
    {
        if (attachedHat == null) return;
        attachedHat.onGrab.RemoveListener(attachedHatGrabbed);
        //attachedHat.onRelease.RemoveListener(attachedHatReleased);
        //attachedHat.body.isKinematic = attachedHat.wasKinematic;
        attachedHat = null;
    }

    private void attachedHatGrabbed(Hand hand, Grabbable grabbable)
    {
        Debug.Log("Attached hat grabbed");
        detachHat();
    }

    //private void attachedHatReleased(Hand hand, Grabbable grabbable)
    //{
    //    if (hoverItem == null) detachHat();
    //}

    private void Update()
    {
        moveHat();
    }

    private void LateUpdate()
    {
        moveHat();
    }

    private void moveHat()
    {
        if (attachedHat == null) return;

        attachedHat.transform.position = AttachPoint.position;
        attachedHat.transform.rotation = AttachPoint.rotation;
    }
}
