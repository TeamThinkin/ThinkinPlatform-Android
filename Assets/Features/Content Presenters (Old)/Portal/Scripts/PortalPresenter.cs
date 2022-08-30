using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PortalPresenter : MonoBehaviour, IContentItemPresenter, IHandlePointerEvent
{
    [SerializeField] TMPro.TMP_Text Label;
    [SerializeField] Animator StateAnimator;
    [SerializeField] GameObject DoorVisual;
    [SerializeField] BoxCollider Collider;

    private Type[] dtoTypes = { typeof(RoomLinkContentItemDto) };
    public Type[] DtoTypes => dtoTypes;
    public GameObject GameObject => gameObject;

    private RoomLinkContentItemDto dto;

    public string Id => dto?.Id;

    public bool HasVisual { get; private set; }

    public CollectionContentItemDto ContentDto => dto;

    public async Task LoadFromDto(CollectionContentItemDto Dto, bool IsSymbolic)
    {
        dto = Dto as RoomLinkContentItemDto;

        HasVisual = !IsSymbolic;

        if (IsSymbolic) return;

        Label.text = dto.DisplayName;

        if(dto.Placement != null)
        {
            transform.position = dto.Placement.Position;
            transform.rotation = dto.Placement.Rotation;
            transform.localScale = dto.Placement.ScaleSized;
        }

        switch(dto.DisplayType)
        {
            case RoomDisplayTypeEnum.Hidden:
                DoorVisual.SetActive(false);
                Collider.size = Vector3.one;
                Collider.center = Vector3.zero;
                break;
            default:
                break;
        }
    }

    public void OnHoverStart(IUIPointer Sender, RaycastHit RayInfo)
    {
        StateAnimator.SetBool("Is Partially Open", true);
    }

    public void OnHoverEnd(IUIPointer Sender)
    {
        if(StateAnimator != null) StateAnimator.SetBool("Is Partially Open", false);
    }

    public void OnGripStart(IUIPointer Sender, RaycastHit RayInfo)
    {
    }

    public void OnGripEnd(IUIPointer Sender)
    {
    }

    public void OnTriggerStart(IUIPointer Sender, RaycastHit RayInfo)
    {
        DestinationPresenter.Instance.DisplayUrl(dto.Url);
    }

    public void OnTriggerEnd(IUIPointer Sender)
    {
    }
}
