using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ToggleState))]
public class ToggleButton : ButtonInteractable
{
    [SerializeField] private TMPro.TMP_Text Label;

    private ToggleState toggleState;

    public object Key;
    public string Text 
    {
        get { return Label.text; }
        set { Label.text = value; }
    }

    private void Start()
    {
        toggleState = GetComponent<ToggleState>();
    }

    public bool IsToggleActive
    {
        get { return toggleState.CurrentState; }
        set { toggleState.SetState(value); }
    }
}
