using Normal.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum SyncDirectionEnum
{
    SyncToTarget,
    TargetToSync
}

public class NetworkItemSync : RealtimeComponent<NetworkItemSyncModel>
{
    public static Dictionary<string, NetworkItemSync> Syncs { get; private set; } = new Dictionary<string, NetworkItemSync>();

    [SerializeField] private RealtimeTransform NetworkTransform;

    public string LocalKey { get; private set; }

    public GameObject TargetItem { get; private set; }

    public SyncDirectionEnum SyncDirection = SyncDirectionEnum.SyncToTarget;

    private void OnDestroy()
    {
        Debug.Log("Sync gameobject being destroyed");

        var entry = Syncs.FirstOrDefault(i => i.Value == this);
        if (entry.Value == this)
        {
            Debug.Log("Removing from Syncs list");
            Syncs.Remove(entry.Key);
        }
    }

    private static string getTargetItemKey(GameObject targetItem)
    {
        return targetItem.name;// .GetPath();
    }

    public static NetworkItemSync CreateOrFind(GameObject TargetItem)
    {
        if (!RoomManager.Instance.RealtimeNetwork.connected) return null;

        string key = getTargetItemKey(TargetItem); 

        if (Syncs.ContainsKey(key))
        {
            Debug.Log("Network sync already present: " + key);
            var sync = Syncs[key];
            sync.RequestTransformOwnership();
            return sync;
        }
        else
        {
            Debug.Log("Creating new network sync.");
            var sync = Normal.Realtime.Realtime.Instantiate("Prefabs/NetworkItemSync", Normal.Realtime.Realtime.InstantiateOptions.defaults).GetComponent<NetworkItemSync>();
            Debug.Log("new network sync created");
            sync.SyncDirection = SyncDirectionEnum.TargetToSync;
            sync.SetKey(key);
            sync.RequestTransformOwnership();
            Syncs.Add(sync.LocalKey, sync);
            return sync;
        }
    }

    public void SetKey(string Key)
    {
        LocalKey = Key;
        model.key = Key;
    }

    public void RequestTransformOwnership()
    {
        Debug.Log("Requesting transform ownership");
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

    //protected override NetworkItemSyncModel CreateModel()
    //{
    //    var model = base.CreateModel();
    //    if (LocalKey != null)
    //    {
    //        Debug.Log("Creating Network item Model with local key");
    //        model.key = LocalKey;
    //    }
    //    return model;
    //}

    protected override void OnRealtimeModelReplaced(NetworkItemSyncModel previousModel, NetworkItemSyncModel currentModel)
    {
        base.OnRealtimeModelReplaced(previousModel, currentModel);
        if(previousModel != null)
        {
            previousModel.keyDidChange -= Model_keyDidChange;
        }
        if(currentModel != null)
        {
            Debug.Log("Model replaced");

            if (LocalKey != null)
            {
                Debug.Log("Creating Network item Model with local key");
                //model.key = LocalKey;
            }

            currentModel.keyDidChange += Model_keyDidChange;
            Model_keyDidChange(currentModel, currentModel.key);
            if (currentModel.key != null && !Syncs.ContainsKey(currentModel.key))
            {
                Debug.Log("NetworkItemSync adding to sync list from within ModelReplace method. Presuming this is a remotely initiated sync");
                Syncs.Add(currentModel.key, this);
            }
        }  

    }

    private void Model_keyDidChange(NetworkItemSyncModel model, string value)
    {
        if (!string.IsNullOrEmpty(model.key))
        {
            Debug.Log("Model key changed: " + model.key);
            TargetItem = GameObject.Find(model.key);
        }
        else
        {
            TargetItem = null;
        }
    }

    private void Update()
    {
        if (TargetItem == null) return;

        switch(SyncDirection)
        {
            case SyncDirectionEnum.TargetToSync:
                copyTransform(TargetItem.transform, this.transform);
                break;
            case SyncDirectionEnum.SyncToTarget:
                copyTransform(this.transform, TargetItem.transform);
                break;
        }
    }

    private void copyTransform(Transform sourceItem, Transform destinationItem)
    {
        destinationItem.position = sourceItem.position;
        destinationItem.rotation = sourceItem.rotation;
    }
}
