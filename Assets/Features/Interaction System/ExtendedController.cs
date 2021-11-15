using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class ExtendedController : ActionBasedController
{
    [SerializeField] private InputActionProperty _altModifierAction;

    /// <summary>
    /// Think of this as the <alt> key on your keyboard
    /// </summary>
    public InputActionProperty altModifierAction
    {
        get { return _altModifierAction; }
        set { _altModifierAction = value; }
    } 

    private void test()
    {
        XRBaseInteractor x;
        altModifierAction.action.ReadValue<bool>();
    }
}
