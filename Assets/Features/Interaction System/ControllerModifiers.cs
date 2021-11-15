using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerModifiers : MonoBehaviour
{
    [SerializeField] private InputActionProperty _altModifierAction;

    public bool IsAltPressed; //{ get; private set; }

    /// <summary>
    /// Think of this as the <alt> key on your keyboard
    /// </summary>
    public InputActionProperty altModifierAction
    {
        get { return _altModifierAction; }
        set { _altModifierAction = value; }
    }
    void Update()
    {
        IsAltPressed = altModifierAction.action.ReadValue<float>() > 0;
    }
}
