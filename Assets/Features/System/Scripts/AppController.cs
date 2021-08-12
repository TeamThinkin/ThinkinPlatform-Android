using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppController : MonoBehaviour
{
    void Start()
    {
        if(UserInfo.CurrentUser == null)
            AppSceneManager.Instance.LoadLocalScene("Login");
        else
            AppSceneManager.Instance.LoadLocalScene("Home Room");
    }
}
