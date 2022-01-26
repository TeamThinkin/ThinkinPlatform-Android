using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewMapDestinationWizard : MonoBehaviour
{
    [SerializeField] private MapPanel MapPanel;
    [SerializeField] private ContentSelectorPanel EnvironmentSelectorPanel;
    [SerializeField] private DestinationDetailsPanel DetailsPanel;

    public RegistryEntryDto TargetMap { get; set; }
    public EnvironmentContentItemDto Environment { get; set; }
    public string DestinationName { get; set; }
    public Vector3 Location { get; set; }

    private int stepIndex;

    public void StartWizard()
    {
        TargetMap = MapPanel.SelectedMapDto;
        DetailsPanel.MapUrl = TargetMap.Url;
        stepIndex = 0;
        showPanel();
    }

    public void Next()
    {
        Debug.Log("Next wizard step. Current index: " + stepIndex);
        switch(stepIndex)
        {
            case 0: //Moving from Environment Select to Details
                if (!EnvironmentSelectorPanel.ValidateInput()) return;
                Environment = EnvironmentSelectorPanel.SelectedListItem.Dto  as EnvironmentContentItemDto;
                break;
            case 1: //Moving from Details back to MapView
                DestinationName = DetailsPanel.DestinationName;
                Location = DetailsPanel.Location;
                //TODO: actually create the entry in the DB
                break;
        }
        stepIndex++;
        showPanel();
    }

    public void Back()
    {
        Debug.Log("Prev wizard step. Current index: " + stepIndex);
        stepIndex--;
        showPanel();
    }

    private void showPanel()
    {
        Debug.Log("Wizard show panel: " + stepIndex);
        switch(stepIndex)
        {
            case -1:
                MapPanel.ShowTab();
                break;
            case 0:
                EnvironmentSelectorPanel.ShowTab();
                break;
            case 1:
                DetailsPanel.ShowTab();
                break;
            case 2:
                MapPanel.ShowTab();
                break;
        }
    }
}
