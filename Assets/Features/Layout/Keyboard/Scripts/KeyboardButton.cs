using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.XR.Interaction.Toolkit;

public class KeyboardButton : MonoBehaviour // : ButtonInteractable
{
    [SerializeField] private Transform Background;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private TMPro.TMP_Text PrimaryLabel;
    public AudioSource AudioPlayer => _audioSource;

    public Keyboard Keyboard;
    public Vector3 LayoutLocalPosition;
    public Vector3 LayoutLocalScale;
    public KeyboardKey KeyInfo;

    public void SetWidth(float width)
    {
        Background.localScale = new Vector3(width, Background.localScale.y, Background.localScale.z);
    }

    //protected override void OnActivated(ActivateEventArgs args)
    //{
    //    base.OnActivated(args);
    //    Keyboard.OnKeyDown(this);
    //    transform.localPosition = LayoutLocalPosition + Vector3.forward * LayoutLocalScale.x * 0.5f;
    //}

    //protected override void OnDeactivated(DeactivateEventArgs args)
    //{
    //    base.OnDeactivated(args);
    //    Keyboard.OnKeyUp(this);
    //    transform.localPosition = LayoutLocalPosition;
    //}

    public void UpdateText()
    {
        if (!string.IsNullOrEmpty(KeyInfo.DisplayText))
            PrimaryLabel.text = KeyInfo.DisplayText;
        else
            PrimaryLabel.text = Keyboard.IsCapitals ? KeyInfo.MainKey.ToUpper() : KeyInfo.MainKey;
    }
}
