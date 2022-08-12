using Autohand;
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
    public static event Action OnRoomLoaded;
    public static event Action OnRoomUnloaded;

    [SerializeField] private Transform _roomItemContainer;
    public Transform RoomItemContainer => _roomItemContainer;

    public static RoomManager Instance { get; private set; }
    public static int? CurrentRoomId { get; private set; }

    private Dictionary<string, Task<IContentItemPresenter[]>> pendingRequests = new Dictionary<string, Task<IContentItemPresenter[]>>();

    private void Awake()
    {
        Instance = this;
    }
    
    public async Task LoadRoomUrl(string Url)
    {
        Url = WebAPI.NormalizeCollectionUrl(Url);
        var newRoomId = Url.GetHashCode();
        if (newRoomId == CurrentRoomId) return;

        WebSocketListener.Socket.Emit("userLocationChanged", Url);

        CurrentRoomId = newRoomId;

        var documentTask = DocumentManager.FetchUrlDocument(Url);

        await TransitionController.Instance.HideScene();

        stashPlayer();

        OnRoomUnloaded?.Invoke();
        _roomItemContainer.ClearChildren();

        var document = await documentTask;
        await DocumentManager.LoadDocument(document, _roomItemContainer);

        OnRoomLoaded?.Invoke();
        TransitionController.Instance.RevealScene();

        releaseStashedPlayer();
    }

    private void stashPlayer()
    {
        var player = AutoHandPlayer.Instance;
        player.body.isKinematic = true;
        player.body.velocity = Vector3.zero;
        player.body.angularVelocity = Vector3.zero;
        player.SetPosition(Vector3.one * -10000);
    }

    private void releaseStashedPlayer()
    {
        AutoHandPlayer.Instance.body.isKinematic = false;
    }
}
