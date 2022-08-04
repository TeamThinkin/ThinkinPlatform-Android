using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CollectionList : MonoBehaviour
{
    [SerializeField] private GameObject ContentListItemPrefab;
    [SerializeField] private LayoutContainer ItemsContainer;

    private List<ContentListItem> listItems = new List<ContentListItem>();

    public void SetItems(CollectionContentItemDto[] Items)
    {
        if (Items == null) return;

        clearListItems();

        foreach(var dto in Items)
        {
            var listItem = Instantiate(ContentListItemPrefab, ItemsContainer.ContentContainer.transform).GetComponent<ContentListItem>();
            listItem.SetDto(dto);
            listItems.Add(listItem);
        }

        ItemsContainer.UpdateLayout();
    }

    private void clearListItems()
    {
        foreach(var item in listItems)
        {
            DestroyImmediate(item.gameObject);
        }

        listItems.Clear();
    }
}
