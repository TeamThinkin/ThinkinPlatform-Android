using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TabView : MonoBehaviour
{
    public event Action SelectedTabChanged;

    private TabPanel[] panels;

    public int SelectedTabIndex { get; private set; } = -1;

    private void Start()
    {
        panels = transform.GetChildren().SelectNotNull(i => i.GetComponent<TabPanel>()).ToArray();
        foreach(var panel in panels)
        {
            panel.Hide(true);
        }

        SelectTab(0);
    }

    public void SelectTab(int TabIndex)
    {
        if (TabIndex == SelectedTabIndex) return;
        if (TabIndex >= panels.Length) return;
        
        if(SelectedTabIndex > -1)
        {
            panels[SelectedTabIndex].Hide();
        }

        SelectedTabIndex = TabIndex;
        panels[SelectedTabIndex].Show();
        SelectedTabChanged?.Invoke();
    }
}
