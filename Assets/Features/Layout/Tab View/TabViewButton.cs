using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabViewButton : MonoBehaviour // : ButtonInteractable
{
    [SerializeField] private ToggleState Toggle;

    public bool IsActive
    {
        get { return Toggle.CurrentState; }
        set { Toggle.SetState(value); }
    }
}
