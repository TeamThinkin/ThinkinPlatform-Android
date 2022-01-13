using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapView : MonoBehaviour
{
    [SerializeField] private Transform ItemContainer;
    [SerializeField] private ScrollArea ScrollArea;
    [SerializeField] private GameObject MapItemPrefab;

    private TypedObjectPool<MapItem> itemPool;

    public IEnumerable<MapItem> Items => itemPool.ActiveItems;
    public string MapUrl { get; private set; }

    private void Start()
    {
        itemPool = new TypedObjectPool<MapItem>(MapItemPrefab);
    }

    public async void LoadCollection(string Url)
    {
        itemPool.Clear();

        MapUrl = Url;

        var links = await CollectionManager.GetCollectionContents<DestinationLinkContentItemDto>(Url);

        foreach (var linkDto in links)
        {
            AddItemFromDto(linkDto);
        }

        ScrollArea.UpdateLayout();
        ScrollArea.CenterContent();
    }

    public void AddItemFromDto(DestinationLinkContentItemDto linkDto)
    {
        var item = itemPool.Get();
        item.SetDto(linkDto);
        item.transform.SetParent(ScrollArea.ContentContainer.transform, false);
        item.transform.localPosition = linkDto.Placement.Position;
        item.transform.localScale = linkDto.Placement.Scale * Vector3.one;
    }
}
