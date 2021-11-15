using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ControllerTester : MonoBehaviour
{
    public bool isPressed;

    private ExtendedController controller;

    void Start()
    {
        controller = GetComponent<ExtendedController>();
        
    }

    void Update()
    {
        var value = controller.altModifierAction.action.ReadValue<float>();
        isPressed = value > 0;
    }
}

public class RayInteractor : XRRayInteractor
{
    private void test()
    {
        var controller = this.xrController as ExtendedController;
    }
}