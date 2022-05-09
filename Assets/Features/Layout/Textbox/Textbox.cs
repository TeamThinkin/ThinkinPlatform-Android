using Autohand;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Textbox : HandTouchEvent, IFocusItem, IHandlePointerEvent
{
    [SerializeField] private TMPro.TMP_Text Label;
    [SerializeField] private GameObject CaretIndicator;

    public event Action<Textbox> Changed;

    public string Text
    {
        get { return Label.text; }
        set 
        {
            if (value == Label.text) return;
            Label.text = value;
            Changed?.Invoke(this);
        }
    }

    private void Start()
    {
        CaretIndicator.SetActive(false);
    }

    protected void OnDestroy()
    {
        Keyboard.Instance.Text.ValueChanged -= Text_ValueChanged;
    }

    protected void OnDisable()
    {
        #pragma warning disable CS0253 // Possible unintended reference comparison; right hand side needs cast
        if (this == Keyboard.Instance.CurrentFocusItem)
            Keyboard.Instance.Close();
        #pragma warning restore CS0253 // Possible unintended reference comparison; right hand side needs cast
    }

    private void OnInteractionStart()
    {
        Keyboard.Instance.ShowForInput(this);
    }

    protected override void OnTouch(Hand hand, Collision collision)
    {
        base.OnTouch(hand, collision);
        OnInteractionStart();
    }

    public void OnTriggerStart(UIPointer Sender, RaycastHit RayInfo)
    {
        OnInteractionStart();
    }

    private void Text_ValueChanged(EditableText keyboardText)
    {
        Text = keyboardText.Value;
        updateCaretPosition();
    }

    public void KeyPressed(string Key, SpecialKeyboardKey SpecialKey)
    {
        switch (SpecialKey)
        {
            case SpecialKeyboardKey.Backspace:
                Text = Text.Substring(0, Text.Length - 1);
                break;
            case SpecialKeyboardKey.None:
                Text += Key;
                break;
        }
    }

    public void OnFocusStart()
    {
        Debug.Log("Focus start: " + gameObject.name);
        Keyboard.Instance.Text.Set(Text);
        Keyboard.Instance.Text.ValueChanged += Text_ValueChanged;
        updateCaretPosition();
        CaretIndicator.SetActive(true);
    }

    public void OnFocusEnd()
    {
        Debug.Log("Focus end: " + gameObject.name);
        Keyboard.Instance.Text.ValueChanged -= Text_ValueChanged;
        CaretIndicator.SetActive(false);
    }

    private void updateCaretPosition()
    {
        var textInfo = Label.GetTextInfo(Label.text);
        Vector3 position;
        if(Keyboard.Instance.Text.CaretPosition >= Label.text.Length)
        {
            if (Label.text.Length > 0)
            {
                var charInfo = textInfo.characterInfo[Label.text.Length - 1];
                position = Label.transform.TransformPoint(charInfo.bottomRight);
            }
            else
            {
                position = Label.transform.position;
            }
        }
        else
        {
            var charInfo = textInfo.characterInfo[Keyboard.Instance.Text.CaretPosition];
            position = Label.transform.TransformPoint(charInfo.bottomLeft);
        }

        CaretIndicator.transform.position = position;
    }

    #region -- Unused Pointer Events --
    public void OnHoverStart(UIPointer Sender, RaycastHit RayInfo)
    {
    }

    public void OnHoverEnd(UIPointer Sender)
    {
    }

    public void OnGripStart(UIPointer Sender, RaycastHit RayInfo)
    {
    }

    public void OnGripEnd(UIPointer Sender)
    {
    }

    public void OnTriggerEnd(UIPointer Sender)
    {
    }
    #endregion
}
