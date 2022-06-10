using Normal.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [SerializeField] private Transform _roomItemContainer;
    public Transform RoomItemContainer => _roomItemContainer;

    [SerializeField] private Realtime _realtimeNetwork;
    public Realtime RealtimeNetwork => _realtimeNetwork;


    public static event Action OnRoomLoaded;
    public static event Action OnRoomUnloaded;

    public static List<IContentItemPresenter> ContentItems = new List<IContentItemPresenter>();

    public static RoomManager Instance { get; private set; }

    public static int? CurrentRoomId;

    private Dictionary<string, Task<IContentItemPresenter[]>> pendingRequests = new Dictionary<string, Task<IContentItemPresenter[]>>();

    private void Awake()
    {
        Instance = this;

        RealtimeNetwork.didConnectToRoom += RealtimeNetwork_didConnectToRoom;
    }
    private void OnDestroy()
    {
        RealtimeNetwork.didConnectToRoom -= RealtimeNetwork_didConnectToRoom;
    }

    private void RealtimeNetwork_didConnectToRoom(Realtime realtime)
    {
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
        Debug.Log("Loading room url: " + Url);
        var task = loadRoomUrl(Url);
        pendingRequests[Url] = task;
        var result = await task;
        pendingRequests.Remove(Url);
        Debug.Log("Loading room complete");
        return result;
    }

    private async Task<IContentItemPresenter[]> loadRoomUrl(string Url)
    {
        var newRoomId = Url.GetHashCode();
        if (newRoomId == CurrentRoomId) return ContentItems.Where(i => i.ContentDto.CollectionUrl == Url).ToArray();

        WebSocketListener.Socket.Emit("userLocationChanged", Url);

        CurrentRoomId = newRoomId;
        
        var dtoTask = CollectionManager.GetCollectionContents(Url);
        await TransitionController.Instance.HideScene();
        
        ContentItems.Clear();
        OnRoomUnloaded?.Invoke();
        _roomItemContainer.ClearChildren();

        var dtos = await dtoTask;
        dtos = dtos.WhereNotNull().ToArray();

        var items = await CollectionManager.LoadDtosIntoContainer(_roomItemContainer, dtos);
        ContentItems.AddRange(items);

        OnRoomLoaded?.Invoke();
        TransitionController.Instance.RevealScene();

        return items;
    }

    public async Task<IContentItemPresenter[]> LoadCollection(string Url)
    {
        if(pendingRequests.ContainsKey(Url))
        {
            Debug.Log("Loading collection already in progress: " + Url);
            var result = await pendingRequests[Url];
            return result;
        }
        else
        {
            var existingCollectionItems = ContentItems.Where(i => i.ContentDto.CollectionUrl == Url);
            if (existingCollectionItems.Any())
            {
                Debug.Log("Loading collection (returning existing items)");
                return existingCollectionItems.ToArray();
            }
            else
            {
                Debug.Log("Loading collection (fresh call)" + Url);
                var requestTask = CollectionManager.LoadUrlIntoContainer(_roomItemContainer, Url);
                pendingRequests.Add(Url, requestTask);
                var result = await requestTask;
                pendingRequests.Remove(Url);
                return result;
            }
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
