using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppController : MonoBehaviour
{
    void Start()
    {
        UserInfo.OnCurrentUserChanged += UserInfo_OnCurrentUserChanged;

        if(UserInfo.CurrentUser == null)
            AppSceneManager.Instance.LoadLocalScene("Login");
        else
            AppSceneManager.Instance.LoadLocalScene("Home Room");
    }

    private void OnDestroy()
    {
        UserInfo.OnCurrentUserChanged -= UserInfo_OnCurrentUserChanged;
    }

    private void UserInfo_OnCurrentUserChanged(UserInfo obj)
    {
        if(UserInfo.CurrentUser == null) AppSceneManager.Instance.LoadLocalScene("Login");
    }

    public void GoToHomeRoom()
    {
        AppSceneManager.Instance.LoadLocalScene("Home Room");
    }
}
