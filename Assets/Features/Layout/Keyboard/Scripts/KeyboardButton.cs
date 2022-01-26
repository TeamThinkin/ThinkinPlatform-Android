using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class KeyboardButton : ButtonInteractable
{
    [SerializeField] private Transform Background;
    public Keyboard Keyboard;

    public string PrimaryKey;
    public string SecondaryKey;
    public KeyboardKey KeyInfo;

    public void SetWidth(float width)
    {
        Background.localScale = new Vector3(width, Background.localScale.y, Background.localScale.z);
    }

    protected override void OnActivated(ActivateEventArgs args)
    {
        base.OnActivated(args);
        Debug.Log("Key down: " + KeyInfo.MainKey);
        Keyboard.OnKeyDown(KeyInfo);
    }

    protected override void OnDeactivated(DeactivateEventArgs args)
    {
        base.OnDeactivated(args);
        Debug.Log("Key up: " + KeyInfo.MainKey);
        Keyboard.OnKeyUp(KeyInfo);
    }
}
