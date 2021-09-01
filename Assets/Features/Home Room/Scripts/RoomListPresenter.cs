using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomListPresenter : MonoBehaviour
{
    [SerializeField] GameObject PortalPrefab;
    [SerializeField] Transform PortalContainer;
    [SerializeField] Transform[] SpawnPoints;
    
    public void SetModel(IEnumerable<RoomInfo> Rooms)
    {
        PortalContainer.ClearChildren();

        int i = 0;
        foreach(var room in Rooms)
        {
            var portal = Instantiate(PortalPrefab).GetComponent<Portal>();
            portal.transform.SetParent(PortalContainer);
            portal.transform.position = SpawnPoints[i].position;
            portal.transform.rotation = SpawnPoints[i].rotation;
            portal.SetModel(room);
            i++;
        }
    }
}
