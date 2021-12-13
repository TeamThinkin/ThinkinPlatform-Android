using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapView : MonoBehaviour
{
    [SerializeField] private Transform ItemContainer;
    [SerializeField] private ScrollArea ScrollArea;

    private TypedObjectPool<ContentSymbol> symbolPool;

    private void Start()
    {
        symbolPool = new TypedObjectPool<ContentSymbol>(AppController.ContentSymbolPrefab);
    }

    public async void LoadCollection(string Url)
    {
        symbolPool.Clear();

        var links = await CollectionManager.GetCollectionContents<DestinationLinkContentItemDto>(Url);

        foreach (var linkDto in links)
        {
            var symbol = symbolPool.Get();
            symbol.SetDto(linkDto);
            symbol.transform.SetParent(ScrollArea.ContentContainer.transform, false);
            symbol.transform.localPosition = linkDto.Placement.Position;
            symbol.transform.localScale = linkDto.Placement.Scale * Vector3.one;
        }

        ScrollArea.UpdateLayout();
        ScrollArea.CenterContent();
    }
}
