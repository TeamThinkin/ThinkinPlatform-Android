using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keyboard : MonoBehaviour
{
    private class KeyInfo
    {
        public float KeyDownTime;
        public bool IsHandled;

        public KeyInfo(float KeyDownTime)
        {
            this.KeyDownTime = KeyDownTime;
            this.IsHandled = false;
        }
    }

    [SerializeField] private KeyboardLayout _layout;
    public KeyboardLayout Layout => _layout;

    [SerializeField] private Transform _sizeReference;
    public Transform SizeReference => _sizeReference;

    [SerializeField] private float LongPressDuration = 0.25f;

    public EditableText Text = new EditableText();

    private Dictionary<KeyboardKey, KeyInfo> keyDownTime = new Dictionary<KeyboardKey, KeyInfo>();

    public static Keyboard Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void OnKeyDown(KeyboardKey Key)
    {
        keyDownTime.Add(Key, new KeyInfo(Time.time));
    }

    private void Update()
    {
        foreach(var entry in keyDownTime)
        {
            if(!entry.Value.IsHandled && entry.Key.Special == SpecialKeyboardKey.None && Time.time - entry.Value.KeyDownTime >= LongPressDuration)
            {
                entry.Value.IsHandled = true;
                Text.AddText(entry.Key.SecondaryKey);
            }
        }
    }

    public void OnKeyUp(KeyboardKey Key)
    {
        var keyInfo = keyDownTime[Key];
        keyDownTime.Remove(Key);

        if (keyInfo.IsHandled) return;

        if (Key.Special == SpecialKeyboardKey.Backspace)
            Text.Backspace();
        else if (Key.Special == SpecialKeyboardKey.None)
            Text.AddText(Key.MainKey);
    }
}
