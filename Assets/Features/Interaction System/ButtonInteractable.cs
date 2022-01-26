using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ButtonInteractable : XRSimpleInteractable
{
    private ActionBasedController controller;
    private XRBaseInteractor interactor;
    private bool isActive;

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
            hoverController.activateAction.action.canceled += Action_canceled;
        }
    }

    private void Action_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        var e = new ActivateEventArgs() { interactable = this, interactor = interactor };
        this.OnActivated(e);
        //this.activated.Invoke(e);
        isActive = true;
    }

    private void Action_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (!isActive) return;
        isActive = false;
        var e = new DeactivateEventArgs() { interactable = this, interactor = interactor };
        this.OnDeactivated(e);
    }

    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        base.OnHoverExited(args);
        if (controller == null) return;

        var hoverController = args.interactor.gameObject.GetComponentInParent<ActionBasedController>();
        if (hoverController == controller)
        {
            hoverController.activateAction.action.performed -= Action_performed;
            hoverController.activateAction.action.canceled += Action_canceled;

            controller = null;
        }
    }

    public override bool IsSelectableBy(XRBaseInteractor interactor)
    {
        return false;
    }
}
