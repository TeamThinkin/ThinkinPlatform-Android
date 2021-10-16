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

    public async Task LoadFromDto(CollectionContentItemDto Dto)
    {
        dto = Dto as EnvironmentContentItemDto;
        await AppSceneManager.Instance.LoadRemoteScene(dto.Url);
    }
}
