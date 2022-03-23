using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [SerializeField] private Transform RoomItemContainer;

    public static event Action OnRoomLoaded;
    public static event Action OnRoomUnloaded;

    public static List<IContentItemPresenter> ContentItems = new List<IContentItemPresenter>();

    public static RoomManager Instance { get; private set; }

    public static int? CurrentRoomId;

    private Dictionary<string, Task<IContentItemPresenter[]>> pendingRequests = new Dictionary<string, Task<IContentItemPresenter[]>>();

    private void Awake()
    {
        Instance = this;
    }


    //public void LoadUrl(string Url)
    //{
    //    var newRoomId = Url.GetHashCode();
    //    if (newRoomId == CurrentRoomId) return;

    //    Debug.Log("Loading new room url: " + Url);

    //    WebSocketListener.Socket.Emit("userLocationChanged", Url);

    //    CurrentRoomId = newRoomId;

    //    var dtoTask = WebAPI.GetCollectionContents(Url);
    //    TransitionController.Instance.HideScene(async () =>
    //    {
    //        ContentItems.Clear();
    //        OnRoomUnloaded?.Invoke();
    //        RoomItemContainer.ClearChildren();

    //        var dtos = await dtoTask;
    //        ContentItems.AddRange(await CollectionManager.LoadDtosIntoContainer(RoomItemContainer, dtos));

    //        OnRoomLoaded?.Invoke();
    //        TransitionController.Instance.RevealScene();
    //    });
    //}



    public async Task<IContentItemPresenter[]> LoadRoomUrl(string Url)
    {
        var newRoomId = Url.GetHashCode();
        if (newRoomId == CurrentRoomId) return ContentItems.Where(i => i.ContentDto.CollectionUrl == Url).ToArray();

        Debug.Log("Loading new room url: " + Url);

        WebSocketListener.Socket.Emit("userLocationChanged", Url);

        CurrentRoomId = newRoomId;

        var dtoTask = WebAPI.GetCollectionContents(Url);
        await TransitionController.Instance.HideScene();
        
        ContentItems.Clear();
        OnRoomUnloaded?.Invoke();
        RoomItemContainer.ClearChildren();

        var dtos = await dtoTask;
        dtos = dtos.WhereNotNull().ToArray();

        var items = await CollectionManager.LoadDtosIntoContainer(RoomItemContainer, dtos);
        ContentItems.AddRange(items);

        OnRoomLoaded?.Invoke();
        TransitionController.Instance.RevealScene();

        return items;
    }

    public async Task<IContentItemPresenter[]> LoadCollection(string Url)
    {
        if(pendingRequests.ContainsKey(Url))
        {
            return await pendingRequests[Url];
        }
        else
        {
            var requestTask = CollectionManager.LoadUrlIntoContainer(RoomItemContainer, Url);
            pendingRequests.Add(Url, requestTask);
            return await requestTask;
        }
    }

    public void UnloadCollection(string Url)
    {
        var hitList = ContentItems.Where(i => i.ContentDto.CollectionUrl == Url).ToArray();
        foreach (var item in hitList)
        {
            Destroy(item.GameObject);
            ContentItems.Remove(item);
        }
    }
}
