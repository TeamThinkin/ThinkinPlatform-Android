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

/*
Old Collection based Urls

Classroom
https://thinkin-api.glitch.me/v1/auth/collection/test-room

Board Room
destination-5b676cd5-3a65-407a-94bc-7cf7edcd9577

Theater
destination-c17b835e-631c-4001-871f-5bd511b702f5

Hotel
destination-1e496332-8b15-466b-aede-efc0e81ec1ef

Campus
destination-b4a9935d-7854-4540-9787-1a00a1d1f8b4

https://usc-intervrse.glitch.me/campus
https://usc-intervrse.glitch.me/theater
https://usc-intervrse.glitch.me/hotel
https://usc-intervrse.glitch.me/boardroom
https://usc-intervrse.glitch.me/classroom

*/