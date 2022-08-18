using Autohand;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DispenserItem : MonoBehaviour, IHandlePointerEvent
{
    public DispenserElementPresenter ParentDispenser;

    private DispenserElementPresenter.ItemInfo itemInfo;

    private void Awake()
    {
        this.gameObject.layer = LayerMask.NameToLayer("UI");
    }

    public void SetItemInfo(DispenserElementPresenter.ItemInfo ItemInfo)
    {
        this.itemInfo = ItemInfo;
    }

    public void OnGripStart(UIPointer Sender, RaycastHit RayInfo)
    {
        var clone = Instantiate(itemInfo.Prefab, ParentDispenser.SceneChildrenContainer.transform);
        clone.name = gameObject.name + " Clone " + ParentDispenser.GetNextItemId();
        clone.transform.localScale = 0.1f * Vector3.one;
        clone.transform.position = Sender.PrimaryHand.transform.position + Sender.PrimaryHand.transform.right * -0.1f;
        NetworkItemSync.MakeGrabbable(clone);

        var networkSync = NetworkItemSync.FindOrCreate(clone, itemInfo.AssetSourceUrl);

        StartCoroutine(attachToHand(Sender.PrimaryHand, clone));
    }


    //private void makeGrabbable(GameObject item)
    //{
    //    var body = item.AddComponent<Rigidbody>();
    //    body.useGravity = false;
    //    body.drag = 0.2f;
    //    body.angularDrag = 0.2f;
    //    //body.isKinematic = true;
    //    //checkPhysicsMaterials(item);

    //    item.AddComponent<Grabbable>();
    //    item.AddComponent<DistanceGrabbable>();
    //    item.AddComponent<GrabSyncMonitor>();
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
