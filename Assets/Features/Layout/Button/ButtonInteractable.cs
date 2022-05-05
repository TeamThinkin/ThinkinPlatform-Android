using Autohand;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class ButtonInteractable : HandTouchEvent
{
    [SerializeField] protected Transform Visuals;
    [SerializeField] protected bool StartPressed = false;
    [SerializeField] protected Vector3 PressOffset = new Vector3(0, 0, 0.05f);
    [SerializeField] protected bool IsToggle = true;
    [SerializeField] protected TMPro.TMP_Text Label;
    [SerializeField] protected AudioClip PressedAudio;
    [SerializeField] protected AudioClip UnpressedAudio;
    [SerializeField] protected AudioSource AudioPlayer;
    [SerializeField] protected Material InactiveMaterial;
    [SerializeField] protected Material ActiveMaterial;
    [SerializeField] protected Renderer BackgroundRenderer;

    [Space]
    [SerializeField] protected UnityHandEvent OnPressed;
    [SerializeField] protected UnityHandEvent OnUnpressed;

    public event Action<ButtonInteractable> OnPressedEvent;
    public event Action<ButtonInteractable> OnUnpressedEvent;

    private bool _isPressed = false;
    public bool IsPressed
    {
        get { return _isPressed; }
        set
        {
            _isPressed = value;
            updateState();
        }
    }

    private void Awake()
    {
        if (BackgroundRenderer != null) BackgroundRenderer.sharedMaterial = InactiveMaterial;
    }

    protected override void OnTouch(Hand hand, Collision collision)
    {
        base.OnTouch(hand, collision);

        if (!collision.InvolvesPrimaryFingerTip()) return; //Only accept input from pointer finger tips to hopefully filter out accidental touches

        if (IsToggle)
        {
            if (!_isPressed)
                Pressed(hand);
            else if (_isPressed)
                Released(hand);
        }
        else if (!_isPressed)
            Pressed(hand);
    }

    protected override void OnUntouch(Hand hand, Collision collision)
    {
        base.OnUntouch(hand, collision);
    
        if (_isPressed && !IsToggle) Released(hand);
    }

    protected virtual void Pressed(Hand hand)
    {
        if (!_isPressed) Visuals.localPosition = PressOffset;
        
        IsPressed = true;
        AudioPlayer?.PlayOneShot(PressedAudio);
        OnPressed?.Invoke(hand);
        OnPressedEvent?.Invoke(this);
    }

    protected virtual void Released(Hand hand)
    {
        if (_isPressed) Visuals.localPosition = Vector3.zero;

        IsPressed = false;
        AudioPlayer?.PlayOneShot(UnpressedAudio);
        OnUnpressed?.Invoke(hand);
        OnUnpressedEvent?.Invoke(this);
    }

    private void updateState()
    {
        Visuals.localPosition = IsPressed ? PressOffset : Vector3.zero;
        if (BackgroundRenderer != null) BackgroundRenderer.sharedMaterial = _isPressed ? ActiveMaterial : InactiveMaterial;
    }
}
