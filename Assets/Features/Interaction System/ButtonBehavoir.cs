using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ButtonBehavoir : InteractableBehavior
{
    public event Action<ActivateEventArgs> Clicked;
    private ActionBasedController controller;
    private XRBaseInteractor interactor;

    private void Start() //This needs to be here so that Unity shows the Enabled checkbox
    {
    }

    public override void OnHoverEntered(HoverEnterEventArgs args)
    {
        base.OnHoverEntered(args);
        if (controller != null) return;
        var hoverController = args.interactor.gameObject.GetComponentInParent<ActionBasedController>();
        if (hoverController != null)
        {
            controller = hoverController;
            interactor = args.interactor;
            hoverController.activateAction.action.performed += Action_performed;
        }
    }

    public void Action_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        var e = new ActivateEventArgs() { interactable = Interactable, interactor = interactor };
        this.OnClicked(e);
    }

    protected virtual void OnClicked(ActivateEventArgs e) 
    {
        Clicked?.Invoke(e);
    }

    public override void OnHoverExited(HoverExitEventArgs args)
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
