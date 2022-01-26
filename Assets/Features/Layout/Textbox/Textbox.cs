using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Textbox : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text Label;

    public string Text
    {
        get { return Label.text; }
        set { Label.text = value; }
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
}
