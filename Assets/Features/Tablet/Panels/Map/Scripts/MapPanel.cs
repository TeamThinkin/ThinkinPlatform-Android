using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapPanel : TabPanel
{
    [SerializeField] private DropDownBox MapsDropDownBox;
    [SerializeField] private MapView MapContents;
    [SerializeField] private TabPanel SelectEnvironmentPanel;

    public RegistryEntryDto SelectedMapDto { get; private set; }

    protected override void OnShow()
    {
        base.OnShow();

        if (UserInfo.CurrentUser != null && UserInfo.CurrentUser != UserInfo.UnknownUser)
        {
            loadMapList();
        }

        UserInfo.OnCurrentUserChanged += UserInfo_OnCurrentUserChanged;
        MapsDropDownBox.SelectedItemChanged += MapsDropDownBox_SelectedItemChanged;
    }


    protected override void OnHide()
    {
        base.OnHide();
        UserInfo.OnCurrentUserChanged -= UserInfo_OnCurrentUserChanged;
    }

    private void OnDestroy()
    {
        UserInfo.OnCurrentUserChanged -= UserInfo_OnCurrentUserChanged;
        MapsDropDownBox.SelectedItemChanged -= MapsDropDownBox_SelectedItemChanged;
    }

    public void NavigateHome()
    {
        Debug.Log("Navigating home");
        RoomManager.Instance.LoadUrl(UserInfo.CurrentUser.HomeRoomUrl);
    }

    public void NewDestinationButtonClicked()
    {
        Debug.Log("Attempting to switch to: " + SelectEnvironmentPanel.gameObject.name);
        ParentTabView.ShowTab(SelectEnvironmentPanel);
    }

    private void UserInfo_OnCurrentUserChanged(UserInfo obj)
    {
        loadMapList();
    }

    private void MapsDropDownBox_SelectedItemChanged(ListItemDto obj)
    {
        if(SelectedMapDto == obj.Value)
        {
            Debug.Log("Skipping MapsDropdown SelectedItemChanged because its the same shit that was already selected");
            return;
        }
        Debug.Log("Maps panel sees that the selected map has changed to: " + obj.Text);
        SelectedMapDto = obj.Value as RegistryEntryDto;
        populateMap(SelectedMapDto.Url);
    }

    private async void loadMapList()
    {
        var mapDtos = await WebAPI.Maps();
        var list = mapDtos.Select(i => new ListItemDto() { Value = i, Text = i.DisplayName });
        MapsDropDownBox.SetItems(list);
    }

    private async void populateMap(string mapUrl)
    {
        Debug.Log("Populating map: " + mapUrl);
        MapContents.LoadCollection(mapUrl);
    }
}
