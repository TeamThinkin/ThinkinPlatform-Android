using Normal.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomManager : RealtimeComponent<RoomManagerModel>
{
    public event Action OnUserListChanged;

    public static RoomManager Instance { get; private set; }

    [SerializeField] private Realtime _normcore;

    public bool IsConnected => _normcore.connected;

    public int ClientId => _normcore.clientID;
    

    public IEnumerable<UserInfoModel> ConnectedUsers
    {
        get
        {
            if (model != null)
            {
                foreach (var item in model.connectedUsers)
                {
                    yield return item;
                }
            }
        }
    }

    private void Awake()
    {
        Instance = this;
        _normcore.didConnectToRoom += _normcore_didConnectToRoom;
    }

    private void OnDestroy()
    {
        _normcore.didConnectToRoom -= _normcore_didConnectToRoom;

        if (model != null)
        {
            model.connectedUsers.modelAdded -= ConnectedUsers_modelAdded;
            model.connectedUsers.modelRemoved -= ConnectedUsers_modelRemoved;
        }
    }

    public void AddUser(UserInfoModel UserInfo)
    {
        if (model == null) return;

        var newEntry = UserInfo.Clone();

        model.connectedUsers.Add(newEntry);
        OnUserListChanged?.Invoke();
    }

    public void RemoveUser(UserInfoModel UserInfo)
    {
        if (model == null) return;
        var entry = model.connectedUsers.SingleOrDefault(i => i.clientId == UserInfo.clientId);
        if (entry != null)
        {
            model.connectedUsers.Remove(entry);
            OnUserListChanged?.Invoke();
        }
    }

    private void _normcore_didConnectToRoom(Realtime realtime)
    {
        bool isFirstOneHere = model.connectedUsers.Count == 0;
        if (isFirstOneHere)
        {
            Debug.Log("We are the first one here. Creating network syncs...");
            foreach (var presenter in DestinationPresenter.Instance.RootPresenter.All())
            {
                presenter.CreateNetworkSync();
            }
        }
    }

    protected override void OnRealtimeModelReplaced(RoomManagerModel previousModel, RoomManagerModel currentModel)
    {
        base.OnRealtimeModelReplaced(previousModel, currentModel);

        if(previousModel != null)
        {
            previousModel.connectedUsers.modelAdded -= ConnectedUsers_modelAdded;
            previousModel.connectedUsers.modelRemoved -= ConnectedUsers_modelRemoved;
        }

        if(currentModel != null)
        {
            currentModel.connectedUsers.modelAdded += ConnectedUsers_modelAdded;
            currentModel.connectedUsers.modelRemoved += ConnectedUsers_modelRemoved;
        }
    }

    private void ConnectedUsers_modelRemoved(Normal.Realtime.Serialization.RealtimeSet<UserInfoModel> set, UserInfoModel model, bool remote)
    {
        OnUserListChanged?.Invoke();
    }

    private void ConnectedUsers_modelAdded(Normal.Realtime.Serialization.RealtimeSet<UserInfoModel> set, UserInfoModel model, bool remote)
    {
        OnUserListChanged?.Invoke();
    }
}
