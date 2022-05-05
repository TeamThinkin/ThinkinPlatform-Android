using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapPanel : TabPanel
{
    [SerializeField] private DropDownBox MapsDropDownBox;
    [SerializeField] private MapView MapContents;
    [SerializeField] private TabPanel SelectEnvironmentPanel;
    [SerializeField] private NewMapDestinationWizard NewDestinationWizard;

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

    public async void NavigateHome()
    {
        await RoomManager.Instance.LoadRoomUrl(UserInfo.CurrentUser.HomeRoomUrl);
    }

    public void NewDestinationButtonClicked()
    {
        NewDestinationWizard.StartWizard();
    }

    private void UserInfo_OnCurrentUserChanged(UserInfo obj)
    {
        loadMapList();
    }

    private void MapsDropDownBox_SelectedItemChanged(ListItemDto obj)
    {
        if(SelectedMapDto == obj.Value)
        {
            return;
        }
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
        await MapContents.LoadCollection(mapUrl);
    }
}
