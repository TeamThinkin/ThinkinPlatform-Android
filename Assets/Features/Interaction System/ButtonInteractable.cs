using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ButtonInteractable : XRSimpleInteractable
{
    private ActionBasedController controller;
    private XRBaseInteractor interactor;

    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        base.OnHoverEntered(args);
        if (controller != null) return;
        var hoverController = args.interactor.gameObject.GetComponentInParent<ActionBasedController>();
        if(hoverController != null)
        {
            controller = hoverController;
            interactor = args.interactor;
            hoverController.activateAction.action.performed += Action_performed;
        }
    }

    private void Action_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        var e = new ActivateEventArgs() { interactable = this, interactor = interactor };
        this.OnActivated(e);
        //this.activated.Invoke(e);
    }

    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        base.OnHoverExited(args);
        if (controller == null) return;

        var hoverController = args.interactor.gameObject.GetComponentInParent<ActionBasedController>();
        if (hoverController == controller)
        {
            hoverController.activateAction.action.performed -= Action_performed;
            controller = null;
        }
    }

    public override bool IsSelectableBy(XRBaseInteractor interactor)
    {
        return false;
    }
}
