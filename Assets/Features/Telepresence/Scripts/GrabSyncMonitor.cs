using Autohand;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabSyncMonitor : MonoBehaviour
{
    private Grabbable grabbable;
    private Rigidbody body;
    private NetworkItemSync sync;
    private bool isWaitingForRest;

    private void Awake()
    {
        grabbable = GetComponent<Grabbable>();
        if(grabbable != null)
        {
            grabbable.OnBeforeGrabEvent = onBeforeGrab;
            grabbable.onGrab.AddListener(onGrab);
            grabbable.onRelease.AddListener(onRelease);

            body = GetComponent<Rigidbody>();
        }
    }

    private void OnDestroy()
    {
        if(grabbable != null)
        {
            grabbable.onGrab.RemoveListener(onGrab);
            grabbable.onRelease.RemoveListener(onRelease);
        }
    }

    private void onBeforeGrab(Hand Hand, Grabbable Grabbable)
    {
        if (body != null)
        {
            Debug.Log("isKinematic set to false", this);
            body.isKinematic = false;
        }
    }

    private void onGrab(Hand Hand, Grabbable Grabbable)
    {
        sync = NetworkItemSync.Create(this.gameObject);
        isWaitingForRest = false;
    }

    private void onRelease(Hand Hand, Grabbable Grabbable)
    {
        if(body == null)
        {
            sync?.Destroy();
            sync = null;
            body.isKinematic = true;
        }
        else isWaitingForRest = true;
    }

    private void Update()
    {
        if(isWaitingForRest && body != null && body.IsSleeping())
        {
            sync?.Destroy();
            sync = null;
            isWaitingForRest = false;
            if (body != null) body.isKinematic = true;
        }
    }
}
