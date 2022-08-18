using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConfigPanel : TabPanel
{
    [SerializeField] private TMPro.TMP_Text CurrentUserDisplayNameLabel;
    [SerializeField] private TMPro.TMP_Text ConnectedUsersLabel;

    public void LogoutButtonPressed()
    {
        DeviceRegistrationController.Logout();
        updateCurrentUserInfo();
    }

    private void OnEnable()
    {
        RoomManager.Instance.OnUserListChanged += Instance_OnUserListChanged;
        updateCurrentUserInfo();
        updateConnectedUsers();
    }

    private void OnDisable()
    {
        RoomManager.Instance.OnUserListChanged -= Instance_OnUserListChanged;
    }

    private void Instance_OnUserListChanged()
    {
        updateConnectedUsers();
    }

    private void updateCurrentUserInfo()
    {
        if(UserInfo.CurrentUser != null && UserInfo.CurrentUser != UserInfo.UnknownUser)
        {
            CurrentUserDisplayNameLabel.text = UserInfo.CurrentUser.DisplayName;
        }
        else
        {
            CurrentUserDisplayNameLabel.text = "<Logged Out>";
        }
    }

    private void updateConnectedUsers()
    {
        ConnectedUsersLabel.text = string.Join('\n', RoomManager.Instance.ConnectedUsers.Select(i => i.displayName).ToArray());
    }
}
