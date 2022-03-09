using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Textbox : ButtonInteractable, IFocusItem
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

    protected override void OnDestroy()
    {
        base.OnDestroy();
        Keyboard.Instance.Text.ValueChanged -= Text_ValueChanged;
    }

    protected override void OnActivated(ActivateEventArgs args)
    {
        base.OnActivated(args);
        Debug.Log("Textbox activated: " + gameObject.name);
        Keyboard.Instance.ShowForInput(this);
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
}
