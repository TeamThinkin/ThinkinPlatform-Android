using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AbsoluteLayoutDataPresenter : MonoBehaviour, IContentItemPresenter
{
    private Type[] dtoTypes = { typeof(AbsoluteLayoutDataContentItemDto) };
    public Type[] DtoTypes => dtoTypes;

    public GameObject GameObject => gameObject;

    public string Id { get; set; }

    public CollectionContentItemDto ContentDto => Dto;
    public AbsoluteLayoutDataContentItemDto Dto { get; private set; }

    public async Task LoadFromDto(CollectionContentItemDto Dto)
    {
        this.Dto = Dto as AbsoluteLayoutDataContentItemDto;
    }
}
