using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class EnvironmentPresenter : MonoBehaviour, IContentItemPresenter
{
    private Type[] dtoTypes = { typeof(EnvironmentContentItemDto) };
    public Type[] DtoTypes => dtoTypes;
    public GameObject GameObject => gameObject;

    private EnvironmentContentItemDto dto;


    public string Id => dto?.Id;

    public bool HasVisual => false;

    public CollectionContentItemDto ContentDto => dto;

    public async Task LoadFromDto(CollectionContentItemDto Dto, bool IsSymbolic)
    {
        dto = Dto as EnvironmentContentItemDto;
        if (!IsSymbolic) await AppSceneManager.LoadRemoteScene(dto.Url);
    }
}
