using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Textbox : ButtonInteractable, IFocusItem
{
    [SerializeField] private TMPro.TMP_Text Label;
    [SerializeField] private GameObject CaretIndicator;

    public string Text
    {
        get { return Label.text; }
        set { Label.text = value; }
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
        FocusManager.SetFocus(this);
    }

    private void Text_ValueChanged(EditableText keyboardText)
    {
        Debug.Log("Textbox sees new text: " + keyboardText);
        Debug.Log("Caret index: " + keyboardText.CaretPosition);
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
        Keyboard.Instance.Text.Set(Text);
        Keyboard.Instance.Text.ValueChanged += Text_ValueChanged;
        updateCaretPosition();
        CaretIndicator.SetActive(true);
    }

    public void OnFocusEnd()
    {
        Keyboard.Instance.Text.ValueChanged -= Text_ValueChanged;
        CaretIndicator.SetActive(false);
    }

    private void updateCaretPosition()
    {
        var textInfo = Label.GetTextInfo(Label.text);
        Debug.Log("UpdateCaretPosition() Text Length: " + Text.Length + ", Caret index: " + Keyboard.Instance.Text.CaretPosition);
        Vector3 position;
        if(Keyboard.Instance.Text.CaretPosition >= Label.text.Length)
        {
            var charInfo = textInfo.characterInfo[Label.text.Length-1];
            position = Label.transform.TransformPoint(charInfo.bottomRight);
        }
        else
        {
            var charInfo = textInfo.characterInfo[Keyboard.Instance.Text.CaretPosition];
            position = Label.transform.TransformPoint(charInfo.bottomLeft);
        }

        CaretIndicator.transform.position = position;
    }
}
