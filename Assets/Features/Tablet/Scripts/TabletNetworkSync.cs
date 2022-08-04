using Normal.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabletNetworkSync : RealtimeComponent<TabletNetworkSyncModel>
{
    [SerializeField] private RealtimeTransform NetworkTransform;
    [SerializeField] private GameObject Visual;

    public bool IsSourceItem;

    private Tablet sourceTablet;

    public void SetSource(Tablet SourceTablet)
    {
        IsSourceItem = true;
        this.sourceTablet = SourceTablet;
        NetworkTransform.RequestOwnership();
        Visual.SetActive(false);
    }

    private void Update()
    {
        if(IsSourceItem)
        {
            transform.position = sourceTablet.transform.position;
            transform.rotation = sourceTablet.transform.rotation;
        }
    }
}
