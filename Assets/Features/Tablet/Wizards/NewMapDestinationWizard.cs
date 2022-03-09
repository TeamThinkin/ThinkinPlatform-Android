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

    public DestinationLinkContentItemDto ItemDto { get; private set; }

    private int stepIndex;

    public void StartWizard()
    {
        ItemDto = new DestinationLinkContentItemDto();
        TargetMap = MapPanel.SelectedMapDto;
        DetailsPanel.MapUrl = TargetMap.Url;
        stepIndex = 0;
        showPanel();
    }

    public void Next()
    {
        switch(stepIndex)
        {
            case 0: //Moving from Environment Select to Details
                if (!EnvironmentSelectorPanel.ValidateInput()) return;
                Environment = EnvironmentSelectorPanel.SelectedListItem.Dto  as EnvironmentContentItemDto;
                ItemDto.DisplayName = Environment.DisplayName;
                break;
            case 1: //Moving from Details back to MapView
                ItemDto.DisplayName = DetailsPanel.DestinationName;
                //ItemDto.Placement.Position = DetailsPanel.Location; //Details panel updates this
                Debug.Log("Creating item in db: " + ItemDto.DisplayName + " (" + ItemDto.Placement.Position + ") " + Environment.DisplayName + " (" + Environment.Id + ")");
                //TODO: actually create the entry in the DB
                break;
        }
        stepIndex++;
        showPanel();
    }

    public void Back()
    {
        stepIndex--;
        showPanel();
    }

    private void showPanel()
    {
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
