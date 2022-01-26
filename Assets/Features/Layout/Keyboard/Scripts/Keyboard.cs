using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keyboard : MonoBehaviour
{
    [SerializeField] private KeyboardLayout _layout;
    public KeyboardLayout Layout => _layout;

    [SerializeField] private Transform _sizeReference;
    public Transform SizeReference => _sizeReference;

    public Textbox FocusedItem;

    public void OnKeyDown(KeyboardKey Key)
    {
        if (FocusedItem == null) return;
    }

    public void OnKeyUp(KeyboardKey Key)
    {
        if (FocusedItem == null) return;
        FocusedItem.KeyPressed(Key.MainKey, Key.Special);
    }
}
