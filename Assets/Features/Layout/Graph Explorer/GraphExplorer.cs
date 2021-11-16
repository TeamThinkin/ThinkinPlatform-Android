using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class GraphExplorer : MonoBehaviour
{
    [SerializeField] private GameObject NodeVisual;
    [SerializeField] private GameObject LinkVisual;
    [SerializeField] private Transform Offset;
    [SerializeField] private Transform TargetPosition;
    [SerializeField] private AnimationCurve TransitionCurve;

    private GraphNode<VisualNode> mapRoot;
    private GraphNode<VisualNode> currentNode;

    private TypedObjectPool<NodeVisual> nodeVisualPool;
    private TypedObjectPool<LinkVisual> linkVisualPool;
    private Coroutine animateOffsetCoroutine;

    private List<VisualNode> itemsOfInterest = new List<VisualNode>();
    private GraphNode<VisualNode> graphOfInterest;

    private void Start()
    {
        nodeVisualPool = new TypedObjectPool<NodeVisual>(NodeVisual);
        linkVisualPool = new TypedObjectPool<LinkVisual>(LinkVisual);
        //mapRoot = getMockGraph().Project(i => new VisualNode() { Dto = i, VisualPool = nodeVisualPool });
        mapRoot = getGenericMockGraph().Project(i => new VisualNode() { Dto = i, VisualPool = nodeVisualPool });
        generateMap();
        selectNode(mapRoot);
    }

    private void generateMap()
    {
        var nodeVisual = mapRoot.Item.Visual = nodeVisualPool.Get();
        nodeVisual.transform.SetParent(Offset, false);
        mapRoot.Item.Position = Vector3.zero;
        mapRoot.Item.Rotation = Quaternion.identity;
        mapRoot.Item.IsLayoutDetermined = true;
        mapRoot.Item.IsVisible = true;
        nodeVisual.SetNode(mapRoot);
    }

    private void selectNode(GraphNode<VisualNode> node)
    {
        //Reset all the existing items
        foreach(var item in itemsOfInterest)
        {
            item.IsVisible = false;
            item.AreLinksVisible = false;
        }

        //Turn on the  new ones of interest;
        GraphNode<VisualNode> targetNode = node;

        var newItemsOfInterest = new List<VisualNode>();
        if (node.ParentNode != null)
        {
            targetNode = node.ParentNode;
            node.ParentNode.Item.IsVisible = true;
            newItemsOfInterest.Add(node.ParentNode.Item);
            foreach(var childNode in node.ParentNode.ChildNodes)
            {
                childNode.Item.IsVisible = true;
                newItemsOfInterest.Add(childNode.Item);
            }
        }
        else
        {
            node.Item.IsVisible = true;
            newItemsOfInterest.Add(node.Item);
        }
        
        foreach(var child in node.ChildNodes)
        {
            child.Item.IsVisible = true;
            child.Item.AreLinksVisible = true;
            newItemsOfInterest.Add(child.Item);
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
                
                //foreach(var linkVisual in item.ChildLinkVisuals)
                //{
                //    linkVisualPool.Release(linkVisual);
                //}
                //item.ChildLinkVisuals.Clear();
            }

        }

        itemsOfInterest = newItemsOfInterest;

        
        generateNodeLayout(targetNode);
        currentNode = node;
        centerViewOnNode(currentNode);
    }

    private void generateNodeLayout(GraphNode<VisualNode> node)
    {
        if(node.Item.Visual == null)
        {
            var visual = node.Item.Visual = nodeVisualPool.Get();
            visual.SetNode(node);
            visual.transform.SetParent(Offset);
            visual.transform.localPosition = node.Item.Position;
            visual.transform.localRotation = node.Item.Rotation;
        }

        Quaternion rotation = node.Item.Visual.transform.localRotation;
        Vector3 position = Vector3.right * .4f; //TODO: this value needs to match the Node's Link size. 

        float spreadAngle = node.ChildNodes.Count * 20;
        float startAngle = spreadAngle / 2;
        float stepSize = node.ChildNodes.Count > 1 ? spreadAngle / (node.ChildNodes.Count - 1) : 0;

        for(int i=0;i<node.ChildNodes.Count;i++)
        {
            var childNode = node.ChildNodes[i];
            if (!itemsOfInterest.Contains(childNode.Item)) continue;

            if (childNode.Item.Visual == null)
            {
                var visual = childNode.Item.Visual = nodeVisualPool.Get();
                visual.SetNode(childNode);
                visual.transform.SetParent(Offset);
                if (childNode.Item.IsLayoutDetermined)
                {
                    visual.transform.localPosition = childNode.Item.Position;
                    visual.transform.localRotation = childNode.Item.Rotation;
                }
                else
                {
                    childNode.Item.Rotation = visual.transform.localRotation = rotation * Quaternion.AngleAxis(stepSize * -i + startAngle, Vector3.forward);
                    childNode.Item.Position = visual.transform.localPosition = node.Item.Visual.transform.localPosition + (visual.transform.localRotation * position);
                    childNode.Item.IsLayoutDetermined = true;
                }

                //add link
                //var linkVisual = linkVisualPool.Get();
                //linkVisual.transform.SetParent(Offset);
                //linkVisual.transform.localPosition = node.Item.Visual.transform.localPosition;
                //linkVisual.transform.localRotation = visual.transform.localRotation;
                //node.Item.ChildLinkVisuals.Add(linkVisual);
            }

            generateNodeLayout(childNode); //Recurse
        }
    }


    #region -- Prev Implementation --
    //private void Start()
    //{
    //    nodeVisualPool = new TypedObject<NodeVisual>(NodeVisual);
    //    linkVisualPool = new TypedObject<LinkVisual>(LinkVisual);

    //    initializeMockMap();
    //    generateMap();

    //    Debug.Log("Subscribing to mouse action");
    //    ClickAction.action.performed += Action_performed;
    //}

    //private void generateMap()
    //{
    //    currentNodeVisual = generateNodeVisual(mapRoot, null);
    //    populateChildLinks(currentNodeVisual);
    //}

    //private void selectNode(NodeVisual nodeVisual)
    //{
    //    currentNodeVisual = nodeVisual;
    //    generateChildLinks(currentNodeVisual);
    //    populateChildLinks(nodeVisual);
    //    cullLinks();

    //    //Center view on selected node
    //    var rotOffset = TargetPosition.rotation * Quaternion.Inverse(nodeVisual.transform.rotation);
    //    var targetRot = rotOffset * Offset.rotation;
    //    var originalRot = Offset.rotation;
    //    Offset.rotation = targetRot;
    //    var targetPosition = Offset.position + (TargetPosition.position - nodeVisual.transform.position);
    //    Offset.rotation = originalRot;

    //    startAnimateOffsetTo(targetPosition, targetRot);
    //}

    //private void cullLinks()
    //{
    //    if (currentNodeVisual.ParentLink != null)
    //    {
    //        var parentNodeVisual = currentNodeVisual.ParentLink.ParentNodeVisual;
    //        foreach (var link in parentNodeVisual.ChildLinks)
    //        {
    //            if (link.ChildNodeVisual == currentNodeVisual) continue;
    //            removeChildLinks(link.ChildNodeVisual);
    //        }

    //        removeParentLinks(parentNodeVisual);
    //    }
    //}

    //private void removeNodeVisual(NodeVisual nodeVisual, bool removeChildLinks)
    //{
    //    if(removeChildLinks) this.removeChildLinks(nodeVisual);
    //    nodeVisual.ParentLink = null;
    //    nodeVisual.ChildLinks.Clear();
    //    nodeVisual.SetDto(null);
    //    nodeVisualPool.Release(nodeVisual);
    //}

    //private void removeChildLinks(NodeVisual nodeVisual)
    //{
    //    foreach (var link in nodeVisual.ChildLinks)
    //    {
    //        if(link.ChildNodeVisual != null) removeNodeVisual(link.ChildNodeVisual, true);
    //        link.ParentNodeVisual = null;
    //        link.ChildNodeVisual = null;
    //        link.ChildDto = null;

    //        linkVisualPool.Release(link);
    //    }
    //    nodeVisual.ChildLinks.Clear();
    //}

    //private void removeParentLinks(NodeVisual nodeVisual)
    //{
    //    if (nodeVisual.ParentLink == null) return;

    //    nodeVisual.transform.SetParent(Offset, true);
    //    nodeVisual.ParentLink.ChildDto = null;
    //    nodeVisual.ParentLink.ChildNodeVisual = null;

    //    var parent = nodeVisual.ParentLink.ParentNodeVisual;
    //    while(parent.ParentLink != null)
    //    {
    //        parent = parent.ParentLink.ParentNodeVisual;
    //    }

    //    removeNodeVisual(parent, true);

    //    nodeVisual.ParentLink = null;
    //}

    //private void populateParentLink(NodeVisual nodeVisual)
    //{
    //    if (nodeVisual.ParentLink == null) return;
    //    generateNodeVisual(nodeVisual.ParentLink.ParentNodeVisual.Dto, null, nodeVisual);
    //}

    //private void populateChildLinks(NodeVisual nodeVisual)
    //{
    //    foreach (var linkVisual in nodeVisual.ChildLinks)
    //    {
    //        if(linkVisual.ChildNodeVisual == null) generateNodeVisual(linkVisual.ChildDto, linkVisual, null);
    //    }
    //}

    //private NodeVisual generateNodeVisual(GraphNode node, LinkVisual linkVisual, NodeVisual existingChild)
    //{
    //    var nodeVisual = nodeVisualPool.Get();
    //    if (linkVisual == null && existingChild == null)
    //    {
    //        nodeVisual.transform.SetParent(Offset, false);
    //    }
    //    else if(existingChild != null)
    //    {

    //    }
    //    else if(linkVisual != null)
    //    {
    //        linkVisual.ChildNodeVisual = nodeVisual;
    //        nodeVisual.ParentLink = linkVisual;
    //        nodeVisual.transform.SetParent(linkVisual.ChildNodePlacement, false);
    //    }

    //    nodeVisual.SetDto(node);
    //    generateChildLinks(nodeVisual);

    //    return nodeVisual;
    //}

    //private void generateChildLinks(NodeVisual nodeVisual)
    //{
    //    if (nodeVisual.ChildLinks.Count > 0) return;

    //    float rotation = 0;
    //    float rotationStep = 30;
    //    foreach(var node in nodeVisual.Dto.ChildNodes)
    //    {
    //        var linkVisual = linkVisualPool.Get();
    //        linkVisual.transform.SetParent(nodeVisual.LinksContainer, false);
    //        linkVisual.transform.localRotation = Quaternion.Euler(rotation, 90, 0);
    //        linkVisual.ParentNodeVisual = nodeVisual;
    //        linkVisual.ChildDto = node;
    //        nodeVisual.ChildLinks.Add(linkVisual);

    //        rotation += rotationStep;
    //    }
    //}
    #endregion

    private void centerViewOnNode(GraphNode<VisualNode> node)
    {
        //Center view on selected node
        var rotOffset = TargetPosition.rotation * Quaternion.Inverse(node.Item.Visual.transform.rotation);
        var targetRot = rotOffset * Offset.rotation;
        var originalRot = Offset.rotation;
        Offset.rotation = targetRot;
        var targetPosition = Offset.position + (TargetPosition.position - node.Item.Visual.transform.position);
        Offset.rotation = originalRot;

        startAnimateOffsetTo(targetPosition, targetRot);
    }

    private void startAnimateOffsetTo(Vector3 targetPosition, Quaternion targetRotation)
    {
        if (animateOffsetCoroutine != null) StopCoroutine(animateOffsetCoroutine);
        animateOffsetCoroutine = StartCoroutine(animateOffsetTo(targetPosition, targetRotation));
    }

    private IEnumerator animateOffsetTo(Vector3 targetPosition, Quaternion targetRotation)
    {
        float duration = 0.25f;
        float elapsed = 0;
        float t;

        var startingPosition = Offset.position;
        var startingRotation = Offset.rotation;

        while (elapsed <= duration)
        {
            t = TransitionCurve.Evaluate(elapsed / duration);

            Offset.position = Vector3.Lerp(startingPosition, targetPosition, t);
            Offset.rotation = Quaternion.Slerp(startingRotation, targetRotation, t);

            yield return null;
            elapsed += Time.deltaTime;
        }

        Offset.position = targetPosition;
        Offset.rotation = targetRotation;
    }

    private GraphNode<NodeDto> getMockGraph()
    {
        var graph = new GraphNode<NodeDto>(new NodeDto("My Collections"), 
            new GraphNode<NodeDto>(new NodeDto("UCS"),
                new GraphNode<NodeDto>(new NodeDto("Improv for Engineers"),
                    new GraphNode<NodeDto>(new NodeDto("Classroom"), 
                        new GraphNode<NodeDto>(new NodeDto("Board Room")),
                        new GraphNode<NodeDto>(new NodeDto("Theater")),
                        new GraphNode<NodeDto>(new NodeDto("Hotel")))),
                new GraphNode<NodeDto>(new NodeDto("Other Such Classes"))
            ),

            new GraphNode<NodeDto>(new NodeDto("Medtronic"), 
                new GraphNode<NodeDto>(new NodeDto("Quantum Doodad"))),

            new GraphNode<NodeDto>(new NodeDto("Arts District"),
                new GraphNode<NodeDto>(new NodeDto("Missy's Gallery")),
                new GraphNode<NodeDto>(new NodeDto("Linquist Gallery")),
                new GraphNode<NodeDto>(new NodeDto("Music Machines"), 
                    new GraphNode<NodeDto>(new NodeDto("Music Machine 1")),
                    new GraphNode<NodeDto>(new NodeDto("Music Machine 2")),
                    new GraphNode<NodeDto>(new NodeDto("Music Machine 3"))))
        );

        //graph.GetChild("Arts District").GetChild("Music Machines").GetChild("Music Machine 2").AddNode(mapRoot);
        return graph;
    }

    private GraphNode<NodeDto> getGenericMockGraph()
    {
        var graph = new GraphNode<NodeDto>(new NodeDto("My Collections"),
            new GraphNode<NodeDto>(new NodeDto("Paramotor"),
                new GraphNode<NodeDto>(new NodeDto("Airfields"),
                    new GraphNode<NodeDto>(new NodeDto("Kansas"),
                        new GraphNode<NodeDto>(new NodeDto("Gardner")),
                        new GraphNode<NodeDto>(new NodeDto("Louisburg")),
                        new GraphNode<NodeDto>(new NodeDto("3-EX")))),
                new GraphNode<NodeDto>(new NodeDto("Pictures"))
            ),

            new GraphNode<NodeDto>(new NodeDto("Family"),
                new GraphNode<NodeDto>(new NodeDto("Vacations"))),

            new GraphNode<NodeDto>(new NodeDto("Freelance / Art"),
                new GraphNode<NodeDto>(new NodeDto("The Met - Projection Mapping")),
                new GraphNode<NodeDto>(new NodeDto("How to Train Your Dragon")),
                new GraphNode<NodeDto>(new NodeDto("Installations"),
                    new GraphNode<NodeDto>(new NodeDto("Virtual Music Machine #3")),
                    new GraphNode<NodeDto>(new NodeDto("Rocking Racers")),
                    new GraphNode<NodeDto>(new NodeDto("Flying Circus"))))
        );

        //graph.GetChild("Arts District").GetChild("Music Machines").GetChild("Music Machine 2").AddNode(mapRoot);
        return graph;
    }

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
                selectNode(nodeVisual.Node);
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

public class NodeDto
{
    public string Name;

    public NodeDto(string Name)
    {
        this.Name = Name;
    }

    public override string ToString()
    {
        return Name ?? base.ToString();
    }
}

public class VisualNode
{
    public NodeDto Dto;
    public bool IsVisible;
    public bool AreLinksVisible;
    public bool IsLayoutDetermined;
    public Vector3 Position;
    public Quaternion Rotation;

    public ObjectPool<NodeVisual> VisualPool;
    public NodeVisual Visual;
    public List<LinkVisual> ChildLinkVisuals = new List<LinkVisual>();

    public override string ToString()
    {
        return Dto?.ToString() ?? base.ToString();
    }
}

public class GraphNode<T> where T: class
{
    public List<GraphNode<T>> ChildNodes = new List<GraphNode<T>>();
    public GraphNode<T> ParentNode;

    public T Item;

    public GraphNode() { }

    public GraphNode(T Item, params GraphNode<T>[] Nodes)
    {
        this.Item = Item;

        foreach(var node in Nodes)
        {
            AddNode(node);
        }
    }

    public void AddNode(GraphNode<T> Node)
    {
        ChildNodes.Add(Node);
        Node.ParentNode = this;
    }

    public GraphNode<T> GetChild(T Item)
    {
        return ChildNodes.FirstOrDefault(i => i.Item == Item);
    }

    public GraphNode<D> Project<D>(Func<T, D> projector) where D: class
    {
        return new GraphNode<D>(projector(Item), ChildNodes.Select(child => child.Project(projector)).ToArray());
    }

    public IEnumerable<GraphNode<T>> Flatten()
    {
        yield return this;
        foreach(var child in ChildNodes)
        {
            foreach(var item in child.Flatten())
            {
                yield return item;
            }
        }
    }

    public override string ToString()
    {
        return Item?.ToString() ?? base.ToString();
    }
}
