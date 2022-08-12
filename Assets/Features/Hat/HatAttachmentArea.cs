using Autohand;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HatAttachmentArea : MonoBehaviour
{
    [SerializeField] private MonoBehaviour[] AreasToDisableWhenHovering;
    [SerializeField] private Transform AttachPoint;
    [SerializeField] private LayerMask HandLayer;

    private Grabbable hoverItem;
    private Grabbable attachedHat;

    private List<GameObject> hoveringHandItems = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        //Check for hands in the trigger area, so we can disable things like the Tablet Spawn Area to prevent accidental activations when the user is trying to remove their hat
        if (HandLayer.Contains(other.gameObject.layer))
        {
            hoveringHandItems.Add(other.gameObject);
            if (attachedHat != null)
            {
                foreach (var area in AreasToDisableWhenHovering)
                {
                    area.enabled = false;
                }

                attachedHat.enabled = true;
            }
        }

        //Check for hovering hat
        var grabbable = other.GetComponentInParent<Grabbable>();
        if (grabbable != null && grabbable.gameObject.tag == "Hat" && grabbable.IsHeld())
        {
            grabbable.onRelease.AddListener(hoverItemReleased);
            hoverItem = grabbable;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Check for hands leaving trigger area
        if (HandLayer.Contains(other.gameObject.layer))
        {
            hoveringHandItems.Remove(other.gameObject);
            if(hoveringHandItems.Count == 0)
            {
                foreach(var area in AreasToDisableWhenHovering)
                {
                    area.enabled = true;
                }

                if(attachedHat != null)
                {
                    attachedHat.enabled = false;
                }
            }
        }

        //Check for hats leaving the area
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
        attachedHat = hoverItem;
        attachedHat.enabled = false;
        attachedHat.body.isKinematic = true;
        attachedHat.onGrab.AddListener(attachedHatGrabbed);
    }

    private void detachHat()
    {
        if (attachedHat == null) return;
        attachedHat.onGrab.RemoveListener(attachedHatGrabbed);
        attachedHat = null;
    }

    private void attachedHatGrabbed(Hand hand, Grabbable grabbable)
    {
        detachHat();
    }

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
