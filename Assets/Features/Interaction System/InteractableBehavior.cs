using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(ComposableInteractable))]
public class InteractableBehavior : MonoBehaviour
{
    public ComposableInteractable Interactable { get; set; }

    public virtual void OnSelectEntering(SelectEnterEventArgs args) { }
    public virtual void OnSelectExiting(SelectExitEventArgs args) { }
    public virtual void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase) { }
    public virtual void OnHoverEntered(HoverEnterEventArgs args) { }
    public virtual void OnHoverExited(HoverExitEventArgs args) { }
    public virtual bool IsSelectableBy(XRBaseInteractor interactor) { return true; }
}
