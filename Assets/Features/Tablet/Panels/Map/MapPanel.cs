using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapPanel : TabPanel
{
    [SerializeField] private DropDownBox MapsDropDownBox;

    protected override void OnShow()
    {
        base.OnShow();

        if (UserInfo.CurrentUser != null && UserInfo.CurrentUser != UserInfo.UnknownUser)
        {
            loadMaps();
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
        loadMaps();
    }

    private void MapsDropDownBox_SelectedItemChanged(ListItemDto obj)
    {
        Debug.Log("Maps panel sees that the selected map has changed to: " + obj.Text);
    }

    private async void loadMaps()
    {
        var mapDtos = await WebAPI.Map();
        MapsDropDownBox.SetItems(mapDtos.Select(i => new ListItemDto() { Value = i, Text = i.DisplayName }));
    }
}
