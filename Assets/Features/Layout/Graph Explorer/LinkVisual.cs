using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkVisual : MonoBehaviour
{
    [SerializeField] private Transform _childNodePlacement;

    public Transform ChildNodePlacement => _childNodePlacement;

    //public GraphNode ChildDto;
    //public NodeVisual ChildNodeVisual;
    //public NodeVisual ParentNodeVisual;
}
