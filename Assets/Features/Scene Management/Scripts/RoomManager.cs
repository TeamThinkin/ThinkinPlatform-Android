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

    private void Awake()
    {
        Instance = this;
    }

    public void LoadUrl(string Url)
    {
        var newRoomId = Url.GetHashCode();
        if (newRoomId == CurrentRoomId) return;

        CurrentRoomId = newRoomId;

        var dtoTask = WebAPI.GetCollectionContents(Url);
        TransitionController.Instance.HideScene(async () =>
        {            
            ContentItems.Clear();
            OnRoomUnloaded?.Invoke();
            RoomItemContainer.ClearChildren();

            var dtos = await dtoTask;
            ContentItems.AddRange(await CollectionManager.LoadDtosIntoContainer(RoomItemContainer, dtos));
            
            OnRoomLoaded?.Invoke();
            TransitionController.Instance.RevealScene();
        });
    }
}
