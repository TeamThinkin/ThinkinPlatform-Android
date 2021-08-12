using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInfoPresenter : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text Label;

    private void Awake()
    {
        if(UserInfo.CurrentUser != null)
        {
            Label.text = "Logged in\n" + UserInfo.CurrentUser.DisplayName + "\n" + UserInfo.CurrentUser.UID;
        }
        else
        {
            Label.text = "Not logged in";
        }
    }
}
