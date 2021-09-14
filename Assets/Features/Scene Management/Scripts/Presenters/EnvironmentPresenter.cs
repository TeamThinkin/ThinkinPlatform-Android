using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentPresenter : MonoBehaviour, IContentItemPresenter
{
    private Type[] dtoTypes = { typeof(EnvironmentContentItemDto) };
    public Type[] DtoTypes => dtoTypes;

    private EnvironmentContentItemDto dto;

    public string Id => dto?.Id;

    public void SetDto(CollectionContentItemDto Dto)
    {
        dto = Dto as EnvironmentContentItemDto;
        AppSceneManager.Instance.LoadRemoteScene(dto.Url);
    }
}
