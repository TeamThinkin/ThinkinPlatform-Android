using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class MapItem : MonoBehaviour // ButtonInteractable
{
    [SerializeField] private ContentSymbol _symbol;
    [SerializeField] private ButtonBehavoir Button;
    [SerializeField] private ConstrainedGrabbleBehavior Grabbable;

    public DestinationLinkContentItemDto Dto { get; private set; }
    public ContentSymbol Symbol => _symbol;

    private void Start()
    {
        Button.Clicked += Button_Clicked;
    }

    public void SetDto(DestinationLinkContentItemDto Dto)
    {
        this.Dto = Dto;
        Symbol.SetDto(Dto);
    }

    public void ToggleEditable(bool IsEditable)
    {
        Button.enabled = !IsEditable;
        Grabbable.enabled = IsEditable;
    }


    private void Button_Clicked(ActivateEventArgs obj)
    {
        Debug.Log("Map item clicked: " + Dto.DisplayName);
        RoomManager.Instance.LoadRoomUrl(Dto.Url);
    }
}
