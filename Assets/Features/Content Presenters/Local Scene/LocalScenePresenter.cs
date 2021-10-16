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

    public async Task LoadFromDto(CollectionContentItemDto Dto)
    {
        dto = Dto as LocalSceneContentItemDto;
        await AppSceneManager.Instance.LoadLocalScene(dto.Path);
    }
}
