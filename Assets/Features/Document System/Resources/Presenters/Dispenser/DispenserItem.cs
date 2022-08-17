using Autohand;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DispenserItem : MonoBehaviour, IHandlePointerEvent
{
    private GameObject itemPrefab; //NOTE: might need to move to sharing ItemInfo instead of prefabs if we start dynamically loading and unloading prefabs from memory

    private void Awake()
    {
        this.gameObject.layer = LayerMask.NameToLayer("UI");
    }

    public void SetItemPrefab(GameObject ItemPrefab)
    {
        itemPrefab = ItemPrefab;
    }

    public void OnGripStart(UIPointer Sender, RaycastHit RayInfo)
    {
        var clone = Instantiate(itemPrefab);
        
        clone.transform.localScale = 0.1f * Vector3.one;
        clone.transform.position = Sender.PrimaryHand.transform.position + Sender.PrimaryHand.transform.right * -0.1f;
        makeGrabbable(clone);

        StartCoroutine(attachToHand(Sender.PrimaryHand, clone));
    }


    private void makeGrabbable(GameObject item)
    {
        var body = item.AddComponent<Rigidbody>();
        body.useGravity = false;
        //body.isKinematic = true;
        //checkPhysicsMaterials(item);

        item.AddComponent<Grabbable>();
        item.AddComponent<DistanceGrabbable>();
        //item.AddComponent<GrabSyncMonitor>();
    }



    //public override void Grab(Hand hand)
    //{
    //    base.Grab(hand);
    //    if (!this.enabled) return;

    //    var tablet = Instantiate(TabletPrefab);
    //    tablet.transform.position = hand.transform.position;
    //    tablet.transform.rotation = hand.transform.rotation;

    //    StartCoroutine(attachToHand(hand, tablet));
    //}

    private IEnumerator attachToHand(Hand hand, GameObject clone)
    {
        yield return new WaitForEndOfFrame(); //The grabbable seems to need some things to be setup in the first frame before the TryGrab can succeed

        var tabletGrabbable = clone.GetComponent<Grabbable>();
        hand.AllowGrabbing = true;
        hand.TryGrab(tabletGrabbable);
    }

    #region -- Unused IHandlePointerEvent's --
    public void OnGripEnd(UIPointer Sender)
    {
    }

    public void OnHoverStart(UIPointer Sender, RaycastHit RayInfo)
    {
    }

    public void OnHoverEnd(UIPointer Sender)
    {
    }

    public void OnTriggerStart(UIPointer Sender, RaycastHit RayInfo)
    {
    }

    public void OnTriggerEnd(UIPointer Sender)
    {
    }
    #endregion


}
