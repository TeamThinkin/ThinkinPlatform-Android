using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class GraphExplorer : MonoBehaviour
{
    [SerializeField] private GameObject NodeVisual;
    [SerializeField] private GameObject LinkVisual;
    [SerializeField] private Transform Offset;
    [SerializeField] private Transform TargetPosition;
    [SerializeField] private AnimationCurve TransitionCurve;

    private TypedObjectPool<NodeVisual> nodeVisualPool;
    private Coroutine animateOffsetCoroutine;
    private Dictionary<IGraphNode, NodeViewModel> viewModels = new Dictionary<IGraphNode, NodeViewModel>();
    private List<NodeViewModel> itemsOfInterest = new List<NodeViewModel>();
    private NodeViewModel currentViewModel;

    public event Action<NodeViewModel> NodeSelected;

    public NodeViewModel CurrentNodeViewModel => currentViewModel;

    public IGraphNode GraphRoot { get; private set; }

    private void Start()
    {
        nodeVisualPool = new TypedObjectPool<NodeVisual>(NodeVisual);
    }

    public void SetGraphRoot(IGraphNode GraphRoot)
    {
        itemsOfInterest.Clear();
        viewModels.Clear(); //TODO: should probably clear their visuals as well

        this.GraphRoot = GraphRoot;
        SelectNode(GraphRoot);
    }

    private NodeViewModel getViewModel(IGraphNode node)
    {
        NodeViewModel viewModel;
        if (!viewModels.ContainsKey(node))
        {
            viewModel = new NodeViewModel();
            viewModel.Node = node;
            viewModel.ParentController = this;
            viewModels.Add(node, viewModel);
        }
        else viewModel = viewModels[node];

        return viewModel;
    }

    public void SelectNode(IGraphNode node)
    {
        NodeViewModel viewModel = getViewModel(node);
        
        //Reset all the existing items
        foreach(var item in itemsOfInterest)
        {
            item.IsVisible = false;
            item.AreLinksVisible = false;
        }

        

        //Turn on the  new ones of interest;
        NodeViewModel targetNodeViewModel = viewModel;

        var newItemsOfInterest = new List<NodeViewModel>();
        if (node.ParentNode != null)
        {
            targetNodeViewModel = viewModels[node.ParentNode];
            targetNodeViewModel.IsVisible = true;
            newItemsOfInterest.Add(targetNodeViewModel);
            
            foreach(var childNode in node.ParentNode.ChildNodes)
            {
                var childViewModel = getViewModel(childNode);
                childViewModel.IsVisible = true;
                newItemsOfInterest.Add(childViewModel);
            }
        }
        else
        {
            viewModel.IsVisible = true;
            newItemsOfInterest.Add(viewModel);
        }
        
        foreach(var childNode in node.ChildNodes)
        {
            var childViewModel = getViewModel(childNode);
            childViewModel.IsVisible = true;
            childViewModel.AreLinksVisible = true;
            newItemsOfInterest.Add(childViewModel);
        }

        //Run through old items and kill the unused ones
        foreach(var item in itemsOfInterest)
        {
            if(!item.IsVisible && item.Visual != null)
            {
                item.Visual.AnimateClosed(() =>
                {
                    nodeVisualPool.Release(item.Visual);
                    item.Visual = null;
                });
            }

        }

        itemsOfInterest = newItemsOfInterest;
        
        generateNodeLayout(targetNodeViewModel);
        currentViewModel = viewModel;
        centerViewOnNode(viewModel);

        NodeSelected?.Invoke(currentViewModel);
    }

    private void generateNodeLayout(NodeViewModel viewModel)
    {
        if(viewModel.Visual == null)
        {
            var visual = viewModel.Visual = nodeVisualPool.Get();
            visual.SetNodeViewModel(viewModel);
            visual.transform.SetParent(Offset, false);
            visual.transform.localPosition = viewModel.Position;
            visual.transform.localRotation = viewModel.Rotation;
        }

        Quaternion rotation = viewModel.Visual.transform.localRotation;
        Vector3 position = Vector3.right * .4f; //TODO: this value needs to match the Node's Link size. 

        int childCount = viewModel.Node.ChildNodes.Count();
        float spreadAngle = childCount * 20;
        float startAngle = spreadAngle / 2;
        float stepSize = childCount > 1 ? spreadAngle / (childCount - 1) : 0;

        int i = 0;
        foreach(var childNode in viewModel.Node.ChildNodes)
        {
            var childViewModel = getViewModel(childNode);
            if (!itemsOfInterest.Contains(childViewModel)) continue;

            if (childViewModel.Visual == null)
            {
                var visual = childViewModel.Visual = nodeVisualPool.Get();
                visual.SetNodeViewModel(childViewModel);
                visual.transform.SetParent(Offset, false);
                if (childViewModel.IsLayoutDetermined)
                {
                    visual.transform.localPosition = childViewModel.Position;
                    visual.transform.localRotation = childViewModel.Rotation;
                }
                else
                {
                    childViewModel.Rotation = visual.transform.localRotation = rotation * Quaternion.AngleAxis(stepSize * -i + startAngle, Vector3.forward);
                    childViewModel.Position = visual.transform.localPosition = viewModel.Visual.transform.localPosition + (visual.transform.localRotation * position);
                    childViewModel.IsLayoutDetermined = true;
                }
            }

            generateNodeLayout(childViewModel); //Recurse
            i++;
        }
    }

    //This one uses world coordinates
    //private void centerViewOnNode(NodeViewModel viewModel)
    //{
    //    //Center view on selected node
    //    var rotOffset = TargetPosition.rotation * Quaternion.Inverse(viewModel.Visual.transform.rotation);
    //    var targetRot = rotOffset * Offset.rotation;
    //    var originalRot = Offset.rotation;
    //    Offset.rotation = targetRot;
    //    var targetPosition = Offset.position + (TargetPosition.position - viewModel.Visual.transform.position);
    //    Offset.rotation = originalRot;

    //    startAnimateOffsetTo(targetPosition, targetRot);
    //}

    private void centerViewOnNode(NodeViewModel viewModel) //Using local space
    {
        //Center view on selected node
        var visualLocalRot = Quaternion.Inverse(transform.rotation) * viewModel.Visual.transform.rotation;

        var rotOffset = TargetPosition.localRotation * Quaternion.Inverse(visualLocalRot);
        var targetRot = rotOffset * Offset.localRotation;
        var originalRot = Offset.localRotation;
        Offset.localRotation = targetRot;
        var visualLocalPosition = transform.InverseTransformPoint(viewModel.Visual.transform.position);
        var targetPosition = Offset.localPosition + (TargetPosition.localPosition - visualLocalPosition);
        Offset.localRotation = originalRot;

        startAnimateOffsetTo(targetPosition, targetRot);
    }

    private string vectorString(Vector3 v)
    {
        string format = "0.000000";
        return "(" + v.x.ToString(format) + ", " + v.y.ToString(format) + ", " + v.z.ToString(format) + ")";
    }

    private void startAnimateOffsetTo(Vector3 targetPosition, Quaternion targetRotation)
    {
        if (animateOffsetCoroutine != null) StopCoroutine(animateOffsetCoroutine);
        animateOffsetCoroutine = StartCoroutine(animateOffsetTo(targetPosition, targetRotation));
    }

    private IEnumerator animateOffsetTo(Vector3 targetPosition, Quaternion targetRotation) //Using local space
    {
        float duration = 0.25f;
        float elapsed = 0;
        float t;

        var startingPosition = Offset.localPosition;
        var startingRotation = Offset.localRotation;

        while (elapsed <= duration)
        {
            t = TransitionCurve.Evaluate(elapsed / duration);

            Offset.localPosition = Vector3.Lerp(startingPosition, targetPosition, t);
            Offset.localRotation = Quaternion.Slerp(startingRotation, targetRotation, t);

            yield return null;
            elapsed += Time.deltaTime;
        }

        Offset.localPosition = targetPosition;
        Offset.localRotation = targetRotation;
    }

    //This one uses world space
    //private IEnumerator animateOffsetTo(Vector3 targetPosition, Quaternion targetRotation)
    //{
    //    float duration = 0.25f;
    //    float elapsed = 0;
    //    float t;

    //    var startingPosition = Offset.position;
    //    var startingRotation = Offset.rotation;

    //    while (elapsed <= duration)
    //    {
    //        t = TransitionCurve.Evaluate(elapsed / duration);

    //        Offset.position = Vector3.Lerp(startingPosition, targetPosition, t);
    //        Offset.rotation = Quaternion.Slerp(startingRotation, targetRotation, t);

    //        yield return null;
    //        elapsed += Time.deltaTime;
    //    }

    //    Offset.position = targetPosition;
    //    Offset.rotation = targetRotation;
    //}



    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame) onMouseClick();
    }

    private void onMouseClick()
    {
        var mouseRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hitInfo;
        if (Physics.Raycast(mouseRay, out hitInfo))
        {
            var nodeVisual = hitInfo.collider.gameObject.GetComponentInParent<NodeVisual>();
            if (nodeVisual != null)
            {
                SelectNode(nodeVisual.NodeViewModel.Node);
            }
        }
    }

    //TODO: temp implementation for testing with a mouse
    private void Action_performed(InputAction.CallbackContext obj)
    {
        Debug.Log("Mouse down");
        onMouseClick();
    }
}

public class NodeViewModel
{
    public GraphExplorer ParentController;
    public IGraphNode Node;
    public bool IsVisible;
    public bool AreLinksVisible;
    public bool IsLayoutDetermined;
    public Vector3 Position;
    public Quaternion Rotation;
    public NodeVisual Visual;

    public override string ToString()
    {
        return Node?.ToString() ?? base.ToString();
    }
}