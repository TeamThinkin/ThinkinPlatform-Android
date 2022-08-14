using AngleSharp.Dom;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[ElementPresenter("portal", "Presenters/Portal/Portal", false)]
public class PortalElementPresenter : ElementPresenterBase, IHandlePointerEvent
{
    public enum DisplayTypeEnum
    {
        Default,
        Hidden
    }

    [SerializeField] TMPro.TMP_Text Label;
    [SerializeField] Animator StateAnimator;
    [SerializeField] GameObject DoorVisual;
    [SerializeField] BoxCollider Collider;

    public bool HasVisual { get; private set; }

    public string Title { get; private set; }
    public string Href { get; private set; }
    public DisplayTypeEnum DisplayType { get; private set; }
    public PlacementInfo Placement { get; private set; }

    public override void ParseDataElement(IElement ElementData)
    {
        Label.text = Title = ElementData.Attributes["title"].Value;
        Placement = GetPlacementInfo(ElementData);
        DisplayType = GetEnumFromAttribute<DisplayTypeEnum>(ElementData.Attributes["type"]);
        Href = TransformRelativeUrlToAbsolute(ElementData.Attributes["href"].Value, ElementData);

        ApplyPlacement(Placement, transform);
    }    

    public override async Task Initialize()
    {
        bool IsSymbolic = false; //TODO: figure out what to do with the symbol thing
    
        HasVisual = !IsSymbolic;

        if (IsSymbolic) return;

        switch (DisplayType)
        {
            case DisplayTypeEnum.Hidden:
                DoorVisual.SetActive(false);
                Collider.size = Vector3.one;
                Collider.center = Vector3.zero;
                break;
            default:
                break;
        }
    }

    public void OnHoverStart(UIPointer Sender, RaycastHit RayInfo)
    {
        StateAnimator.SetBool("Is Partially Open", true);
    }

    public void OnHoverEnd(UIPointer Sender)
    {
        if(StateAnimator != null) StateAnimator.SetBool("Is Partially Open", false);
    }

    public void OnGripStart(UIPointer Sender, RaycastHit RayInfo)
    {
    }

    public void OnGripEnd(UIPointer Sender)
    {
    }

    public void OnTriggerStart(UIPointer Sender, RaycastHit RayInfo)
    {
        DestinationPresenter.Instance.DisplayUrl(Href);
    }

    public void OnTriggerEnd(UIPointer Sender)
    {
    }
}
