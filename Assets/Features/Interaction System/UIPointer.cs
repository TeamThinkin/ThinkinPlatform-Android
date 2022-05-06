using Autohand;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPointer : MonoBehaviour
{
    [SerializeField] private Hand PrimaryHand;
    [SerializeField] private LayerMask Layers;
    [SerializeField] private float MaxRayDistance = 10;
    [SerializeField] private LineRenderer Line;

    private void OnEnable()
    {
        PrimaryHand.OnTriggerGrab += PrimaryHand_OnTriggerGrab;
        PrimaryHand.OnTriggerRelease += PrimaryHand_OnTriggerRelease;
        PrimaryHand.OnSqueezed += PrimaryHand_OnSqueezed;
        PrimaryHand.OnUnsqueezed += PrimaryHand_OnUnsqueezed;
    }


    private void OnDisable()
    {
        PrimaryHand.OnTriggerGrab -= PrimaryHand_OnTriggerGrab;
        PrimaryHand.OnTriggerRelease -= PrimaryHand_OnTriggerRelease;
        PrimaryHand.OnSqueezed -= PrimaryHand_OnSqueezed;
        PrimaryHand.OnUnsqueezed -= PrimaryHand_OnUnsqueezed;
    }

    private void Update()
    {
        if(Physics.Raycast(new Ray(transform.position, transform.forward), out RaycastHit hitInfo, MaxRayDistance, Layers))
        {
            Line.positionCount = 2;
            Line.SetPositions(new Vector3[] { transform.position, hitInfo.point });
            Line.enabled = true;

            PrimaryHand.AllowGrabbing = false;
        }
        else
        {
            Line.enabled = false;

            PrimaryHand.AllowGrabbing = true;
            //Line.positionCount = 2;
            //Line.SetPositions(new Vector3[] { transform.position, transform.position + transform.forward * MaxRayDistance });
        }
    }

    private void PrimaryHand_OnSqueezed(Hand hand, Grabbable grabbable) //Trigger button
    {
    }

    private void PrimaryHand_OnUnsqueezed(Hand hand, Grabbable grabbable)
    {
    }

    private void PrimaryHand_OnTriggerGrab(Hand hand, Grabbable grabbable) //Grip button
    {
    }

    private void PrimaryHand_OnTriggerRelease(Hand hand, Grabbable grabbable)
    {
    }

}
