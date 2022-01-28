using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FocusManager
{
    public static IFocusItem CurrentFocusItem { get; private set; }

    public static void SetFocus(IFocusItem Item)
    {
        if (Item != CurrentFocusItem && CurrentFocusItem != null) CurrentFocusItem.OnFocusEnd();

        CurrentFocusItem = Item;
        CurrentFocusItem.OnFocusStart();
    }
}

public interface IFocusItem
{
    GameObject gameObject { get; }

    void OnFocusStart();
    void OnFocusEnd();
}
