using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PortalPresenter : MonoBehaviour, IContentItemPresenter, IHandlePointerEvent
{
    [SerializeField] TMPro.TMP_Text Label;
    [SerializeField] Animator StateAnimator;

    private Type[] dtoTypes = { typeof(RoomLinkContentItemDto) };
    public Type[] DtoTypes => dtoTypes;
    public GameObject GameObject => gameObject;

    private RoomLinkContentItemDto dto;

    public string Id => dto?.Id;

    public CollectionContentItemDto ContentDto => dto;

    public async Task LoadFromDto(CollectionContentItemDto Dto)
    {
        dto = Dto as RoomLinkContentItemDto;
        Label.text = dto.DisplayName;

        if(dto.Placement != null)
        {
            transform.position = dto.Placement.Position;
            transform.rotation = dto.Placement.Rotation;
            transform.localScale = dto.Placement.Scale * Vector3.one;
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
        RoomManager.Instance.LoadRoomUrl(dto.Url);
    }

    public void OnTriggerEnd(UIPointer Sender)
    {
    }
}
