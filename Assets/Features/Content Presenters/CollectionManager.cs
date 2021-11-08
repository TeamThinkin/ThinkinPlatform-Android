using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class CollectionManager
{
    public static async Task<IContentItemPresenter[]> LoadUrlIntoContainer(Transform ContentContainer, string Url)
    {
        var dtos = await WebAPI.GetCollectionContents(Url);
        return await LoadDtosIntoContainer(ContentContainer, dtos);
    }

    public static async Task<IContentItemPresenter[]> LoadDtosIntoContainer(Transform ContentContainer, IEnumerable<CollectionContentItemDto> Dtos)
    {
        var items = await Task.WhenAll(Dtos.Select(dto => PresenterFactory.Instance.Instantiate(dto)));
        items = items.Where(i => i != null).ToArray();

        foreach(var item in items)
        {
            item.GameObject.transform.SetParent(ContentContainer);
        }
        return items;
    }
}
