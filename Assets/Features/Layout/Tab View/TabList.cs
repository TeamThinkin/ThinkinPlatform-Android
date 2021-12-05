using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TabList : MonoBehaviour
{
    [SerializeField] private TabView ParentTabView;

    private TabViewButton[] buttons;
    private TabViewButton activeButton;

    private void Start()
    {
        ParentTabView.SelectedTabChanged += ParentTabView_SelectedTabChanged;
        buttons = transform.GetChildren().SelectNotNull(i => i.GetComponent<TabViewButton>()).ToArray();
        foreach (var button in buttons)
        {
            button.IsActive = false;
            button.activated.AddListener(Button_OnActivated);
        }
    }

    private void OnDestroy()
    {
        ParentTabView.SelectedTabChanged -= ParentTabView_SelectedTabChanged;
        foreach (var button in buttons)
        {
            button?.activated.RemoveListener(Button_OnActivated);
        }
    }

    private void Button_OnActivated(ActivateEventArgs e)
    {
        ParentTabView.SelectTab(buttons.Single(i => i == e.interactable).transform.GetSiblingIndex());
    }

    private void ParentTabView_SelectedTabChanged()
    {
        if(activeButton != null) activeButton.IsActive = false;

        activeButton = buttons[ParentTabView.SelectedTabIndex];
        activeButton.IsActive = true;
    }
}
