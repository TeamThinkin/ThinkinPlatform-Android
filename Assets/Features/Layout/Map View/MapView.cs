using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class MapView : MonoBehaviour
{
    [SerializeField] private Transform ItemContainer;
    [SerializeField] private ScrollArea ScrollArea;
    [SerializeField] private GameObject MapItemPrefab;
    [SerializeField] private bool IsEditable;

    private TypedObjectPool<MapItem> itemPool;

    public IEnumerable<MapItem> Items => itemPool.ActiveItems;
    public string MapUrl { get; private set; }
    public ScrollArea ContentContainer => ScrollArea;

    private void Awake()
    {
        itemPool = new TypedObjectPool<MapItem>(MapItemPrefab);
        ToggleSymbolEditing(IsEditable);
    }

    public async Task LoadCollection(string Url)
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

    public MapItem AddItemFromDto(DestinationLinkContentItemDto linkDto)
    {
        var item = itemPool.Get();
        item.SetDto(linkDto);
        item.transform.SetParent(ScrollArea.ContentContainer.transform, false);
        if (linkDto.Placement != null)
        {
            item.transform.localPosition = linkDto.Placement.Position;
            item.transform.localScale = linkDto.Placement.Scale * Vector3.one;
        }
        item.ToggleEditable(IsEditable);
        return item;
    }

    public void ToggleSymbolEditing(bool IsEditable)
    {
        foreach (var item in Items)
        {
            item.ToggleEditable(IsEditable);
        }
    }
}
