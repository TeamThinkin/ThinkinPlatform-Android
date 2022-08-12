using Normal.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NetworkItemSync : RealtimeComponent<NetworkItemSyncModel>
{
    public static Dictionary<string, NetworkItemSync> Syncs { get; private set; } = new Dictionary<string, NetworkItemSync>();

    [SerializeField] private RealtimeView NetworkItem;
    [SerializeField] private RealtimeTransform NetworkTransform;

    public string LocalKey { get; private set; }

    public GameObject TargetItem { get; private set; }

    private void OnDestroy()
    {
        var entry = Syncs.FirstOrDefault(i => i.Value == this);
        if (entry.Value == this)
        {
            Syncs.Remove(entry.Key);
        }
    }

    private static string getTargetItemKey(GameObject targetItem)
    {
        return targetItem.name;// .GetPath();
    }

    public static NetworkItemSync CreateOrFind(GameObject TargetItem)
    {
        if (!AppController.Instance.RealtimeNetwork.connected) return null;

        string key = getTargetItemKey(TargetItem); 

        if (Syncs.ContainsKey(key))
        {
            var sync = Syncs[key];
            sync.RequestTransformOwnership();
            return sync;
        }
        else
        {
            var sync = Normal.Realtime.Realtime.Instantiate("Prefabs/NetworkItemSync", Normal.Realtime.Realtime.InstantiateOptions.defaults).GetComponent<NetworkItemSync>();
            sync.SetKey(key);
            sync.RequestTransformOwnership();
            Syncs.Add(sync.LocalKey, sync);
            return sync;
        }
    }


    private void Update()
    {
        if (TargetItem == null) return;

        if (NetworkTransform.isOwnedRemotelySelf)
            copyTransform(this.transform, TargetItem.transform);
        else
            copyTransform(TargetItem.transform, this.transform);
    }


    public void SetKey(string Key)
    {
        LocalKey = Key;
        model.key = Key;
    }

    public void RequestTransformOwnership()
    {
        NetworkItem.RequestOwnership();
        NetworkTransform.RequestOwnership();
    }

    public void Destroy()
    {
        if(this != null && this.gameObject != null) Normal.Realtime.Realtime.Destroy(this.gameObject);
    }

    public static bool Exists(string Key)
    {
        return Syncs.ContainsKey(Key);
    }

    protected override void OnRealtimeModelReplaced(NetworkItemSyncModel previousModel, NetworkItemSyncModel currentModel)
    {
        base.OnRealtimeModelReplaced(previousModel, currentModel);
        if(previousModel != null)
        {
            previousModel.keyDidChange -= Model_keyDidChange;
        }
        if(currentModel != null)
        {
            currentModel.keyDidChange += Model_keyDidChange;
            Model_keyDidChange(currentModel, currentModel.key);
            if (currentModel.key != null && !Syncs.ContainsKey(currentModel.key)) Syncs.Add(currentModel.key, this);
        }  
    }

    private void Model_keyDidChange(NetworkItemSyncModel model, string value)
    {
        if (!string.IsNullOrEmpty(model.key))
            TargetItem = GameObject.Find(model.key);
        else
            TargetItem = null;
    }

    private void copyTransform(Transform sourceItem, Transform destinationItem)
    {
        destinationItem.position = sourceItem.position;
        destinationItem.rotation = sourceItem.rotation;
    }
}
