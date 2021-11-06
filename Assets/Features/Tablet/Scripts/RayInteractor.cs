using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class RayInteractor : XRRayInteractor
{
    private InputActionProperty originalSelectAction, originalActivateAction;
    private ActionBasedController controller;
    private XRBaseInteractable hoverObject;
    private bool isSwapped;

    protected override void Awake()
    {
        base.Awake();

        controller = this.xrController as ActionBasedController;
        originalSelectAction = controller.selectAction;
        originalActivateAction = controller.activateAction;
    }

    //protected override void OnHoverEntered(HoverEnterEventArgs args)
    //{
    //    base.OnHoverEntered(args);


    //    if (hoverObject != null) return;

    //    Debug.Log("Hover enter: " + args.interactable.gameObject.name);

    //    if (args.interactable.gameObject.CompareTag("SelectOnActivate"))
    //    {
    //        //Debug.Log("Using swap");
    //        controller.activateAction = originalSelectAction;
    //        controller.selectAction = originalActivateAction;
    //        hoverObject = args.interactable;
    //        Debug.Log("Hover object: " + hoverObject.gameObject.name);
    //        isSwapped = true;
    //    }
    //    else
    //    {
    //        //Debug.Log("Using standard");
    //        controller.activateAction = originalActivateAction;
    //        controller.selectAction = originalSelectAction;
    //        isSwapped = false;
    //    }
    //}    

    //protected override void OnHoverExited(HoverExitEventArgs args)
    //{
    //    base.OnHoverExited(args);

    //    if (args.interactable != hoverObject) return;

    //    Debug.Log("Hover exit: " + args.interactable.gameObject.name);
    //    controller.activateAction = originalActivateAction;
    //    controller.selectAction = originalSelectAction;
    //    isSwapped = false;
    //    hoverObject = null;
    //}
}

public class MyInteractionManager : XRInteractionManager
{
    
}
