using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PortalPresenter : MonoBehaviour, IContentItemPresenter
{
    [SerializeField] TMPro.TMP_Text Label;
    [SerializeField] Animator StateAnimator;

    private Type[] dtoTypes = { typeof(RoomLinkContentItemDto) };
    public Type[] DtoTypes => dtoTypes;
    public GameObject GameObject => gameObject;

    private RoomLinkContentItemDto dto;

    public string Id => dto?.Id;

    public void OnHoverStart()
    {
        StateAnimator.SetBool("Is Partially Open", true);
    }

    public void OnHoverEnd()
    {
        StateAnimator.SetBool("Is Partially Open", false);
    }

    public void OnActivated()
    {
        RoomManager.Instance.LoadUrl(dto.Url);
    }

    public async Task LoadFromDto(CollectionContentItemDto Dto)
    {
        dto = Dto as RoomLinkContentItemDto;
        Label.text = dto.DisplayName;
    }
}
