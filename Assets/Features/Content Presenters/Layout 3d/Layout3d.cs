using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Layout3d : MonoBehaviour, IContentItemPresenter
{
    private Type[] dtoTypes = { typeof(Layout3dContentItemDto) };
    public Type[] DtoTypes => dtoTypes;
    public GameObject GameObject => gameObject;

    private string Url;

    private Layout3dContentItemDto dto;

    public string Id => dto?.Id;

    public bool HasVisual => false;

    public CollectionContentItemDto ContentDto => dto;

    public async Task LoadFromDto(CollectionContentItemDto Dto, bool IsSymbolic)
    {
        dto = Dto as Layout3dContentItemDto;

        if(!IsSymbolic) loadSubContent();

        //var dtos = await WebAPI.GetCollectionContents(dto.Url);
        //foreach (ItemTransformDto dto in dtos)
        //{
        //    var item = GameObject.Find(dto.ItemId);
        //    if (item)
        //    {
        //        item.transform.localPosition = new Vector3(dto.Position.x, dto.Position.y, dto.Position.z);
        //        item.transform.localRotation = new Quaternion(dto.Rotation.x, dto.Rotation.y, dto.Rotation.z, dto.Rotation.w);
        //        item.transform.localScale = new Vector3(dto.Position.x, dto.Position.y, dto.Position.z);
        //    }
        //    else Debug.Log("Couldnt find layout item: " + dto.ItemId);
        //}
    }

    private async void loadSubContent()
    {
        var url = !string.IsNullOrEmpty(dto.Url) ? dto.Url : dto.CollectionUrl;
        //Debug.Log("Layout 3d loading collection: " + url + " | " + dto.Url + " | " + dto.CollectionUrl);
        var items = await RoomManager.Instance.LoadCollection(url);
        var layoutDataItems = items.Where(i => i is AbsoluteLayoutDataPresenter).Select(i => i as AbsoluteLayoutDataPresenter).ToArray();
        foreach(var layoutData in layoutDataItems)
        {
            var item = RoomManager.Instance.RoomItemContainer.Find(layoutData.Dto.ItemKey);
            if (item != null)
            {
                item.localPosition = layoutData.Dto.Placement.Position;
                item.localRotation = layoutData.Dto.Placement.Rotation;
                item.localScale = layoutData.Dto.Placement.Scale * Vector3.one;
            }
            else Debug.Log("Layout3d could not find item: " + layoutData.Dto.ItemKey);
        }
    }
}
