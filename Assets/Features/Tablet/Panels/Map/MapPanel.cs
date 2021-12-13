using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapPanel : TabPanel
{
    [SerializeField] private DropDownBox MapsDropDownBox;
    [SerializeField] private MapView MapContents;

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

    private void UserInfo_OnCurrentUserChanged(UserInfo obj)
    {
        loadMapList();
    }

    private void MapsDropDownBox_SelectedItemChanged(ListItemDto obj)
    {
        Debug.Log("Maps panel sees that the selected map has changed to: " + obj.Text);
        var selectedMapDto = obj.Value as RegistryEntryDto;
        populateMap(selectedMapDto.Url);
    }

    private async void loadMapList()
    {
        var mapDtos = await WebAPI.Map();
        var list = mapDtos.Select(i => new ListItemDto() { Value = i, Text = i.DisplayName });
        MapsDropDownBox.SetItems(list);
    }

    private async void populateMap(string mapUrl)
    {
        Debug.Log("Populating map: " + mapUrl);
        MapContents.LoadCollection(mapUrl);
    }
}
