﻿using UnityEngine;
using System;
using System.Collections;
using UnitySocketIO.SocketIO;
using UnitySocketIO.Events;

public class SocketIOController : MonoBehaviour {
        
    public SocketIOSettings settings;
    public static SocketIOController Instance { get; private set; }

    BaseSocketIO socketIO;

    public string SocketID { get { return socketIO.SocketID; } }

    public bool IsConnected
    {
        get { return socketIO.IsConnected; }
    }

    void Awake() {
        Instance = this;
        if(Application.platform == RuntimePlatform.WebGLPlayer) {
            socketIO = gameObject.AddComponent<WebGLSocketIO>();
        }
        else {
            socketIO = gameObject.AddComponent<NativeSocketIO>();
        }
        if(settings.autoConnect)
        {
            socketIO.Init(settings);
            socketIO.Connect();
        }
    }

    public void Init()
    {
        socketIO.Init(settings);
    }

    public void Connect() {
        socketIO.Connect();
    }

    public void Close() {
        socketIO.Close();
    }

    public void Emit(string e) {
        socketIO.Emit(e);
    }
    public void Emit(string e, Action<string> action) {
        socketIO.Emit(e, action);
    }
    public void Emit(string e, string data) {
        socketIO.Emit(e, data);
    }
    public void Emit(string e, string data, Action<string> action) {
        socketIO.Emit(e, data, action);
    }

    public void On(string e, Action<SocketIOEvent> callback) {
        socketIO.On(e, callback);
    }
    public void Off(string e, Action<SocketIOEvent> callback) {
        socketIO.Off(e, callback);
    }



}