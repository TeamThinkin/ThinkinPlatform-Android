using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Layout3d : MonoBehaviour, IContentItemPresenter
{
    private Type[] dtoTypes = { typeof(Layout3dContentItemDto) };
    public Type[] DtoTypes => dtoTypes;

    private string Url;

    private Layout3dContentItemDto dto;

    public string Id => dto?.Id;
    
    public void SetDto(CollectionContentItemDto Dto)
    {
        dto = Dto as Layout3dContentItemDto;

        //Invoke("executeLayout", 2);
        executeLayout();
    }

    private async void executeLayout()
    {
        Debug.Log("Layout requesting " + dto.Url);

        var dtos = await WebAPI.GetCollectionContents(dto.Url);
        foreach (ItemTransformDto dto in dtos)
        {
            var item = GameObject.Find(dto.ItemId);
            if (item)
            {
                item.transform.localPosition = new Vector3(dto.Position.x, dto.Position.y, dto.Position.z);
                item.transform.localRotation = new Quaternion(dto.Rotation.x, dto.Rotation.y, dto.Rotation.z, dto.Rotation.w);
                item.transform.localScale = new Vector3(dto.Position.x, dto.Position.y, dto.Position.z);
            }
            else Debug.Log("Couldnt find layout item: " + dto.ItemId);
        }
    }
}
