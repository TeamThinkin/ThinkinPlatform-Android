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

    //protected override void OnRealtimeModelReplaced(TabletNetworkSyncModel previousModel, TabletNetworkSyncModel currentModel)
    //{
    //    base.OnRealtimeModelReplaced(previousModel, currentModel);
    //}

    public void SetSource(Tablet SourceTablet)
    {
        IsSourceItem = true;
        this.sourceTablet = SourceTablet;
        NetworkTransform.RequestOwnership();
        Visual.SetActive(false);
        Debug.Log("Source tablet set: " + this.sourceTablet);
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
