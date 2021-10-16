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

    public async void LoadUrl(string Url)
    {
        CurrentRoomId = Url.GetHashCode();

        TransitionController.Instance.HideScene(async () =>
        {
            OnRoomUnloaded?.Invoke();
            RoomItemContainer.ClearChildren();
            ContentItems.Clear();
            var dtos = await WebAPI.GetCollectionContents(Url);
            var items = await Task.WhenAll(dtos.Select(dto => PresenterFactory.Instance.Instantiate(dto)));
            for (int i = 0; i < dtos.Length; i++)
            {
                var item = items[i];
                var dto = dtos[i];

                if (item != null)
                {
                    item.GameObject.name = dto.Id;
                    item.GameObject.transform.SetParent(RoomItemContainer);
                    ContentItems.Add(item);
                }
                else Debug.Log("Skipping unrecognized item: " + dto.MimeType);
            }
            OnRoomLoaded?.Invoke();
            TransitionController.Instance.RevealScene();
        });
    }
}
