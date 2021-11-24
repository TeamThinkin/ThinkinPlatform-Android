using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class CollectionNodeDto : INodeItem<CollectionNodeDto>
{
    public string Url { get; set; }
    public string DisplayName { get; set; }
    public bool IsItemsPopulated { get; set; }
    public CollectionContentItemDto[] Items { get; set; }

    public GraphNode<CollectionNodeDto> Node { get; set; }

    public CollectionNodeDto() { }
    public CollectionNodeDto(string DisplayName)
    {
        this.DisplayName = DisplayName;
    }

    public async Task PopulateItems() 
    {
        Items = (await CollectionManager.GetCollectionContents(Url)).WhereNotNull().ToArray();
        IsItemsPopulated = true;
        await Node.FormChildNodes();
    }

    public void SetNode(GraphNode<CollectionNodeDto> Node)
    {
        this.Node = Node;
    }

    public override string ToString()
    {
        return DisplayName ?? Url ?? base.ToString();
    }
}

public static class CollectionManager
{
    private static GraphNode<CollectionNodeDto> _userRootCollection = new GraphNode<CollectionNodeDto>() { Item = new CollectionNodeDto() { DisplayName = "Your Collections", Url = UserRootCollectionUrl } };
    public static GraphNode<CollectionNodeDto> UserRootCollection => _userRootCollection;

    public static string UserRootCollectionUrl
    {
        get
        {
            if (UserInfo.CurrentUser == null) return null;
            return WebAPI.HomeServerApiBaseUrl + "auth/collection/user-" + UserInfo.CurrentUser.Id;
        }
    }

    public static async Task FormChildNodes(this GraphNode<CollectionNodeDto> parentNode)
    {
        if (parentNode == null) Debug.LogError("FormChildNodes parentNode is null");
        if (parentNode.Item == null) Debug.LogError("FormChildNodes parentNode.Item is null");

        if (!parentNode.Item.IsItemsPopulated) await parentNode.Item.PopulateItems();

        if (parentNode.Item.Items == null) Debug.LogError("Items is null");

        
        var links = parentNode.Item.Items.Where(i => i != null && i.MimeType == "link/collection").Select(i => i as CollectionLinkContentItemDto);
        foreach (var link in links)
        {
            var existingNode = parentNode.ChildNodes.FirstOrDefault(i => i.Item.Url == link.Url);
            if (existingNode == null)
            {
                var childNode = new GraphNode<CollectionNodeDto>() { Item = new CollectionNodeDto() { DisplayName = link.DisplayName, Url = link.Url } };
                parentNode.AddChildNode(childNode);
            }
        }
    }

    public static async Task<CollectionContentItemDto[]> GetCollectionContents(string Url)
    {
        var dtos = await WebAPI.GetCollectionContents(Url);
        //TODO: implement local caching layer
        return dtos;
    }
    public static async Task<IContentItemPresenter[]> LoadUrlIntoContainer(Transform ContentContainer, string Url)
    {
        var dtos = await GetCollectionContents(Url);
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

//public abstract class CollectionCommand
//{
//    public string CollectionUrl { get; set; }
//    public abstract void Execute();
//}

//public class CreateCommand : CollectionCommand
//{
//    public override void Execute()
//    {
//    }
//}

//public class DeleteCommand : CollectionCommand
//{
//    public override void Execute()
//    {
//    }
//}
