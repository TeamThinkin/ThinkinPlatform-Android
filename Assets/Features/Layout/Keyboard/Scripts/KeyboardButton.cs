using Autohand;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardButton : ButtonInteractable
{
    [SerializeField] private Transform Background;
    [SerializeField] private TMPro.TMP_Text PrimaryLabel;

    public Keyboard Keyboard;
    public Vector3 LayoutLocalPosition;
    public Vector3 LayoutLocalScale;
    public KeyboardKey KeyInfo;

    public void SetWidth(float width)
    {
        Background.localScale = new Vector3(width, Background.localScale.y, Background.localScale.z);
    }

    protected override void onInteractionStart(Hand hand)
    {
        base.onInteractionStart(hand);

        Keyboard.OnKeyDown(this);
        //transform.localPosition = LayoutLocalPosition + Vector3.forward * LayoutLocalScale.x * 0.5f;
    }


    protected override void onInteractionEnd(Hand hand)
    {
        base.onInteractionEnd(hand);

        Keyboard.OnKeyUp(this);
        //transform.localPosition = LayoutLocalPosition;
    }

    public void UpdateText()
    {
        if (!string.IsNullOrEmpty(KeyInfo.DisplayText))
            PrimaryLabel.text = KeyInfo.DisplayText;
        else
            PrimaryLabel.text = Keyboard.IsCapitals ? KeyInfo.MainKey.ToUpper() : KeyInfo.MainKey;
    }
}
