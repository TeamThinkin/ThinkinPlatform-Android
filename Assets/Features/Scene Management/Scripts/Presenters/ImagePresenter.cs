using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImagePresenter : MonoBehaviour, IContentItemPresenter
{
    private Type[] dtoTypes = { typeof(ImageContentItemDto) };
    public Type[] DtoTypes => dtoTypes;


    private ImageContentItemDto dto;

    public string Id => dto?.Id;


    public void SetDto(CollectionContentItemDto Dto)
    {
        dto = Dto as ImageContentItemDto;
    }
}

public interface IContentItemPresenter
{
    Type[] DtoTypes { get; }

    string Id { get; }

    void SetDto(CollectionContentItemDto Dto);
}
