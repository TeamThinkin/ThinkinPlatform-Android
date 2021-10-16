using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ImagePresenter : MonoBehaviour, IContentItemPresenter
{
    private Type[] dtoTypes = { typeof(ImageContentItemDto) };
    public Type[] DtoTypes => dtoTypes;
    public GameObject GameObject => gameObject;


    private ImageContentItemDto dto;

    public string Id => dto?.Id;


    public async Task LoadFromDto(CollectionContentItemDto Dto)
    {
        dto = Dto as ImageContentItemDto;
    }
}
