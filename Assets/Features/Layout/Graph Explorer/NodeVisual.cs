using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NodeVisual : MonoBehaviour// : ButtonInteractable
{
    [SerializeField] private TMPro.TMP_Text Label;
    [SerializeField] private Animator NodeAnimator;
    [SerializeField] private GameObject Link;

    public NodeViewModel NodeViewModel { get; private set; }

    private System.Action onCloseAnimationCompleteCallback;

    public void SetNodeViewModel(NodeViewModel NodeViewModel) 
    {
        this.NodeViewModel = NodeViewModel;
        Label.text = NodeViewModel.Node.ToString();
        gameObject.name = Label.text;
        Link.SetActive(NodeViewModel.Node.ParentNode != null);
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

    //protected override void OnActivated(ActivateEventArgs args)
    //{
    //    base.OnActivated(args);
    //    NodeViewModel.ParentController.SelectNode(NodeViewModel.Node);
    //}
}
