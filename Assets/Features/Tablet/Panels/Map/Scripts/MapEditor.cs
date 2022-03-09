using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapEditor : MonoBehaviour
{
    [SerializeField] private MapView View;
    [SerializeField] private GameObject[] RestingPanels;
    [SerializeField] private GameObject[] EditingPanels;

    private DestinationLinkContentItemDto[] originalDtos;

    private void Start()
    {
        EditingPanels.SetActive(false);
        RestingPanels.SetActive(true);
    }

    #region -- UI Events --
    public void StartEditing() 
    {
        RestingPanels.SetActive(false);
        EditingPanels.SetActive(true);

        preserveOriginalData();
        enableSymbolEditing();
    }

    public void SaveEdits()
    {
        Debug.Log("Saving map edits");
        EditingPanels.SetActive(false);
        RestingPanels.SetActive(true);
        disableSymbolEditing();

        var changes = Changeset<DestinationLinkContentItemDto>.Compare(originalDtos, View.Items.Select(i => i.Dto));

        var existingItems = ItemsFromDtos(changes.MatchedItems);
        foreach(var existingItem in existingItems)
        {
            existingItem.Dto.Placement.Position = existingItem.transform.localPosition;
            existingItem.Dto.Placement.Scale = existingItem.transform.localScale.x;
            WebAPI.UpdateCollectionItem(View.MapUrl, existingItem.Dto);
        }
    }

    public void CancelEdits()
    {
        EditingPanels.SetActive(false);
        RestingPanels.SetActive(true);
        disableSymbolEditing();

        var changes = Changeset<DestinationLinkContentItemDto>.Compare(originalDtos, View.Items.Select(i => i.Dto));

        var existingItems = ItemsFromDtos(changes.MatchedItems);
        foreach(var existingItem in existingItems)
        {
            existingItem.transform.localPosition = existingItem.Dto.Placement.Position;
            existingItem.transform.localScale = existingItem.Dto.Placement.Scale * Vector3.one;
        }

        var newItems = ItemsFromDtos(changes.NewItems);
        foreach(var newItem in newItems)
        {
            Destroy(newItem.gameObject);
        }

        foreach(var removedDto in changes.RemovedItems)
        {
            View.AddItemFromDto(removedDto);
        }
    }
    #endregion

    private IEnumerable<MapItem> ItemsFromDtos(IEnumerable<DestinationLinkContentItemDto> dtos)
    {
        return dtos.SelectNotNull(dto => View.Items.FirstOrDefault(item => item.Dto.Id == dto.Id));
    }

    private void preserveOriginalData()
    {
        originalDtos = View.Items.Select(i => i.Dto).ToArray();
    }



    private void enableSymbolEditing()
    {
        foreach (var item in View.Items)
        {
            item.ToggleEditable(true);
        }
    }

    private void disableSymbolEditing()
    {
        foreach (var item in View.Items)
        {
            item.ToggleEditable(false);
        }
    }
}

public class Changeset<T>
{
    public T[] NewItems;
    public T[] RemovedItems;
    public T[] MatchedItems;

    public static Changeset<T> Compare(IEnumerable<T> firstSet, IEnumerable<T> secondSet)
    {
        var changeset = new Changeset<T>();
        changeset.NewItems = secondSet.Where(i => !firstSet.Contains(i)).ToArray();
        changeset.RemovedItems = firstSet.Where(i => !secondSet.Contains(i)).ToArray();
        changeset.MatchedItems = firstSet.Where(i => secondSet.Contains(i)).ToArray();
        return changeset;
    }
}