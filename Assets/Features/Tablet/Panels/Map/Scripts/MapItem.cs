using Autohand;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapItem : HandTouchEvent
{
    [SerializeField] private ContentSymbol _symbol;
    //[SerializeField] private ButtonBehavoir Button;
    //[SerializeField] private ConstrainedGrabbleBehavior Grabbable;

    public DestinationLinkContentItemDto Dto { get; private set; }
    public ContentSymbol Symbol => _symbol;

    private void Start()
    {
        //Button.Clicked += Button_Clicked;
    }

    public void SetDto(DestinationLinkContentItemDto Dto)
    {
        this.Dto = Dto;
        Symbol.SetDto(Dto);
    }

    public void ToggleEditable(bool IsEditable)
    {
        //Button.enabled = !IsEditable;
        //Grabbable.enabled = IsEditable;
    }

    protected async override void OnTouch(Hand hand, Collision collision)
    {
        base.OnTouch(hand, collision);

        if (!collision.InvolvesPrimaryFingerTip()) return; //Only accept input from pointer finger tips to hopefully filter out accidental touches
        
        await RoomManager.Instance.LoadRoomUrl(Dto.Url);
    }
}
