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
            Label.text = "Logged in\n" + UserInfo.CurrentUser.DisplayName + "\n" + UserInfo.CurrentUser.Id + "\n" + UserInfo.CurrentUser.AvatarUrl;
        }
        else
        {
            Label.text = "Not logged in";
        }
    }
}
