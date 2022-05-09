using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHandlePointerEvent
{
    GameObject gameObject { get; }

    void OnHoverStart(UIPointer Sender, RaycastHit RayInfo);
    void OnHoverEnd(UIPointer Sender);

    void OnGripStart(UIPointer Sender, RaycastHit RayInfo);
    void OnGripEnd(UIPointer Sender);

    void OnTriggerStart(UIPointer Sender, RaycastHit RayInfo);
    void OnTriggerEnd(UIPointer Sender);
}