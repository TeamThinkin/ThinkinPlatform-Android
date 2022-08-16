using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class LocalScenePresenter : MonoBehaviour, IContentItemPresenter
{
    private Type[] dtoTypes = { typeof(LocalSceneContentItemDto) };
    public Type[] DtoTypes => dtoTypes;
    public GameObject GameObject => gameObject;

    private LocalSceneContentItemDto dto;

    public string Id => dto?.Id;

    public bool HasVisual => false;

    public CollectionContentItemDto ContentDto => dto;

    public async Task LoadFromDto(CollectionContentItemDto Dto, bool IsSymbolic)
    {
        dto = Dto as LocalSceneContentItemDto;
        if(!IsSymbolic) await AppSceneManager.LoadLocalScene(dto.Path);
    }
}
