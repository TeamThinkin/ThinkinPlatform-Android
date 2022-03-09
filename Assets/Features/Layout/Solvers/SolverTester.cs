using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SolverTester : MonoBehaviour
{
    [SerializeField] private Transform LayoutArea;
    [SerializeField] private GameObject ItemPrefab;
    [SerializeField] private Transform ItemContainer;
    [SerializeField] private Material InvalidMaterial;
    [SerializeField] private Material ValidMaterial;

    private XRIDefaultInputActions controls;
    private ExpandingRegionSampler solver = new ExpandingRegionSampler();

    private BlockLayoutItem newItem;

    void Start()
    {
        controls = new XRIDefaultInputActions();
        controls.UIControls.Enable();
        controls.UIControls.TestButton1.performed += TestButton1_performed;
        controls.UIControls.TestButton2.performed += TestButton2_performed;
    }

    private void TestButton1_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        Debug.Log("Test button1 pressed");
        newItem = Instantiate(ItemPrefab).GetComponent<BlockLayoutItem>();
        newItem.transform.SetParent(ItemContainer, false);
        newItem.transform.localScale = new Vector3(
            Random.Range(0.01f, 0.05f),
            Random.Range(0.01f, 0.05f),
            0.02f
        );

        solver.StartSolve(new LayoutSolverParams()
        {
            ExistingItems = ItemContainer.GetComponentsInChildren<BlockLayoutItem>().Select(i => i.GetBounds()).ToArray(),
            LayoutArea = new Bounds(Vector3.zero, LayoutArea.localScale),
            NewItemBounds = newItem.GetBounds()
        });
        iterateSolver();
    }

    private void TestButton2_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        Debug.Log("Test button2 pressed");
        iterateSolver();      
    }

    private void iterateSolver()
    {
        solver.Iterate(10000);
        newItem.transform.localPosition = solver.Params.NewItemBounds.center;
        newItem.GetComponent<Renderer>().sharedMaterial = solver.IsIntersecting ? InvalidMaterial : ValidMaterial;
    }
}
