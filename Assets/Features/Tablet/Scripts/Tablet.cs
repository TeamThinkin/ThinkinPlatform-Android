using Normal.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tablet : MonoBehaviour
{
    public bool IsInDespawnZone;

    [SerializeField] private LayoutContainer MenuContentContainer;
    [SerializeField] private LayoutContainer MenuScrollArea;

    //private XRGrabInteractable grabInteractable;

    private void Start()
    {
        //grabInteractable = GetComponent<XRGrabInteractable>();
        //grabInteractable.selectExited.AddListener(grabInteractable_SelectExited);

        MenuScrollArea.UpdateLayout();
    }

    private void OnDestroy()
    {
        //grabInteractable?.selectExited.RemoveListener(grabInteractable_SelectExited);
    }

    public void Test()
    {
        Debug.Log("Testing!!!!");
    }

    //private void grabInteractable_SelectExited(SelectExitEventArgs e)
    //{
    //    if (e.isCanceled) return;
    //    if(IsInDespawnZone) Realtime.Destroy(this.gameObject);
    //}
}
