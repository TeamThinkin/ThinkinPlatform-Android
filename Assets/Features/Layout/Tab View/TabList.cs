using Autohand;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TabList : MonoBehaviour
{
    [SerializeField] private TabView ParentTabView;

    private ButtonInteractable[] buttons;
    private ButtonInteractable activeButton;

    private void Start()
    {
        ParentTabView.SelectedTabChanged += ParentTabView_SelectedTabChanged;
        buttons = transform.GetChildren().SelectNotNull(i => i.GetComponent<ButtonInteractable>()).ToArray();
        foreach (var button in buttons)
        {
            button.IsPressed = false;
            button.OnPressedEvent += Button_OnPressedEvent;
        }
    }

    private void OnDestroy()
    {
        ParentTabView.SelectedTabChanged -= ParentTabView_SelectedTabChanged;
        foreach (var button in buttons)
        {
            button.OnPressedEvent -= Button_OnPressedEvent;
        }
    }

    private void Button_OnPressedEvent(ButtonInteractable sender)
    {
        ParentTabView.SelectTabByIndex(buttons.Single(i => i == sender).transform.GetSiblingIndex());
    }

    private void ParentTabView_SelectedTabChanged()
    {
        if(activeButton != null) activeButton.IsPressed = false;
        if (ParentTabView.SelectedTabIndex >= buttons.Length) return;

        activeButton = buttons[ParentTabView.SelectedTabIndex];
        activeButton.IsPressed = true;
    }
}
