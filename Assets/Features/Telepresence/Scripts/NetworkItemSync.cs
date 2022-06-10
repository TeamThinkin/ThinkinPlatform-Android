using Normal.Realtime;
using System.Collections;
using System.Collections.Generic;
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
        if (!string.IsNullOrEmpty(LocalKey))
            Syncs.Remove(LocalKey);
    }

    public static NetworkItemSync Create(GameObject TargetItem)
    {
        if (!RoomManager.Instance.RealtimeNetwork.connected) return null;

        string key = TargetItem.name; // .GetPath();

        if (Syncs.ContainsKey(key))
        {
            var sync = Syncs[key];
            sync.RequestTransformOwnership();
            return sync;
        }
        else
        {
            var sync = Normal.Realtime.Realtime.Instantiate("Prefabs/NetworkItemSync", Normal.Realtime.Realtime.InstantiateOptions.defaults).GetComponent<NetworkItemSync>();
            Debug.Log("Creating new network sync. Heres the key: " + key);
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
        NetworkTransform.RequestOwnership();
    }

    public void Destroy()
    {
        Normal.Realtime.Realtime.Destroy(this.gameObject);
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
        }  
    }

    private void Model_keyDidChange(NetworkItemSyncModel model, string value)
    {
        if (!string.IsNullOrEmpty(model.key))
        {
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
