using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ComposableInteractable : XRBaseInteractable
{
    private InteractableBehavior[] behaviors;

    protected override void Awake()
    {
        base.Awake();
        behaviors = GetComponents<InteractableBehavior>();
        foreach (var behavior in behaviors) behavior.Interactable = this;
    }

    protected override void OnSelectEntering(SelectEnterEventArgs args)
    {
        base.OnSelectEntering(args);
        foreach (var behavior in behaviors) if(behavior.enabled) behavior.OnSelectEntering(args);
    }

    protected override void OnSelectExiting(SelectExitEventArgs args)
    {
        base.OnSelectExiting(args);
        foreach (var behavior in behaviors) if (behavior.enabled) behavior.OnSelectExiting(args);
    }

    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        base.ProcessInteractable(updatePhase);
        foreach (var behavior in behaviors) if (behavior.enabled) behavior.ProcessInteractable(updatePhase);
    }

    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        base.OnHoverEntered(args);
        foreach (var behavior in behaviors) if (behavior.enabled) behavior.OnHoverEntered(args);
    }

    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        base.OnHoverExited(args);
        foreach (var behavior in behaviors) if (behavior.enabled) behavior.OnHoverExited(args);
    }

    public override bool IsSelectableBy(XRBaseInteractor interactor)
    {
        foreach (var behavior in behaviors)
        {
            if (behavior.enabled && !behavior.IsSelectableBy(interactor)) return false;
        }
        return true;
    }
}
