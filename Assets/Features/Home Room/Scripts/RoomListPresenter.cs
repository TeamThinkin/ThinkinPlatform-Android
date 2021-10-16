using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomListPresenter : MonoBehaviour
{
    [SerializeField] GameObject PortalPrefab;
    [SerializeField] Transform PortalContainer;
    [SerializeField] Transform[] SpawnPoints;

    private void Start()
    {
        AppSceneManager.OnItemsLoaded += AppSceneManager_OnItemsLoaded;
    }

    private void OnEnable()
    {
        AppSceneManager_OnItemsLoaded();
    }

    private void OnDestroy()
    {
        AppSceneManager.OnItemsLoaded -= AppSceneManager_OnItemsLoaded;
    }

    private void AppSceneManager_OnItemsLoaded()
    {
        Debug.Log("RoomListPresenter sees new items");
        var roomLinks = AppSceneManager.ContentItems.Where(i => i.DtoTypes.Contains(typeof(RoomLinkContentItemDto)));

        int i = 0;
        foreach(var roomLink in roomLinks)
        {
            //var portal = roomLink.GameObject.GetComponent<PortalPresenter>();
            roomLink.GameObject.transform.position = SpawnPoints[i].position;
            roomLink.GameObject.transform.rotation = SpawnPoints[i].rotation;
            i++;
        }
    }

    //public void SetModel(IEnumerable<RoomInfo> Rooms)
    //{
    //    PortalContainer.ClearChildren();

    //    int i = 0;
    //    foreach(var room in Rooms)
    //    {
    //        var portal = Instantiate(PortalPrefab).GetComponent<PortalPresenter>();
    //        portal.transform.SetParent(PortalContainer);
    //        portal.transform.position = SpawnPoints[i].position;
    //        portal.transform.rotation = SpawnPoints[i].rotation;
    //        portal.SetModel(room);
    //        i++;
    //    }
    //}
}
