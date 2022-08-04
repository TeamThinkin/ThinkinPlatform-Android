using System;
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
    private static GraphNode<CollectionNodeDto> _userHomeCollection = new GraphNode<CollectionNodeDto>() { Item = new CollectionNodeDto() { DisplayName = "Home Collection", Url = UserHomeCollectionUrl } };
    public static GraphNode<CollectionNodeDto> UserHomeCollection => _userHomeCollection;

    private static GraphNode<CollectionNodeDto> _publicCollection = new GraphNode<CollectionNodeDto>() { Item = new CollectionNodeDto() { DisplayName = "Public Collection", Url = PublicCollectionUrl } };
    public static GraphNode<CollectionNodeDto> PublicCollection => _publicCollection;

    public static string PublicCollectionUrl => "public";

    public static string UserHomeCollectionUrl => (UserInfo.IsLoggedIn ? "user-" + UserInfo.CurrentUser.Id : null);

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

    public static async Task<CollectionContentItemDto[]> GetCollectionContents(string Url, Func<CollectionContentItemDto, bool> filter = null)
    {
        if (string.IsNullOrEmpty(Url)) return new CollectionContentItemDto[0];
        var dtos = await WebAPI.GetCollectionContents(Url);

        //TODO: implement local caching layer
        if (filter != null) dtos = dtos.Where(filter).ToArray();
        
        foreach (var dto in dtos)
        {
            dto.CollectionUrl = Url;
        }

        return dtos;
    }

    public static async Task<CollectionContentItemDto[]> GetPublicCollectionContents(Func<CollectionContentItemDto, bool> filter = null)
    {
        return await GetCollectionContents("public", filter);
    }

    public static async Task<CollectionContentItemDto[]> GetUserCollectionContents(Func<CollectionContentItemDto, bool> filter = null)
    {
        return await GetCollectionContents("user-" + UserInfo.CurrentUser.Id, filter);
    }

    public static async Task<T[]> GetCollectionContents<T>(string Url) where T: CollectionContentItemDto
    {
        var dtos = await GetCollectionContents(Url);
        return dtos.Where(i => i is T).Select(i => i as T).ToArray();
    }

    public static async Task<IContentItemPresenter[]> LoadUrlIntoContainer(Transform ContentContainer, string Url)
    {
        var dtos = await GetCollectionContents(Url);
        return await LoadDtosIntoContainer(ContentContainer, dtos);
    }

    public static async Task<IContentItemPresenter[]> LoadDtosIntoContainer(Transform ContentContainer, IEnumerable<CollectionContentItemDto> Dtos)
    {
        var items = await Task.WhenAll(Dtos.Select(dto => PresenterFactory.Instance.Instantiate(dto, ContentContainer, false)));
                
        items = items.Where(i => i != null).ToArray();

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
