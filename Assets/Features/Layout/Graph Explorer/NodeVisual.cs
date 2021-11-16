using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NodeVisual : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text Label;
    [SerializeField] private Animator NodeAnimator;
    [SerializeField] private GameObject Link;

    public GraphNode<VisualNode> Node { get; private set; }

    private System.Action onCloseAnimationCompleteCallback;

    public void SetNode(GraphNode<VisualNode> Node) 
    {
        this.Node = Node;
        Label.text = Node.Item.Dto.Name;
        gameObject.name = Node.Item.Dto.Name;
        Link.SetActive(Node.ParentNode != null);
    }

    public void OnCloseAnimationComplete() //Called from Animation
    {
        onCloseAnimationCompleteCallback?.Invoke();
    }

    public void AnimateClosed(System.Action OnComplete)
    {
        NodeAnimator.SetBool("Is Open", false);
        onCloseAnimationCompleteCallback = OnComplete;
    }
}
