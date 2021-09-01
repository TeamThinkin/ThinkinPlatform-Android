using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text Label;
    [SerializeField] Animator StateAnimator;

    private string destinationUrl;

    public void SetModel(RoomInfo room)
    {
        destinationUrl = room.EnvironmentUrl;
        Label.text = room.DisplayName;
    }

    public void OnHoverStart()
    {
        StateAnimator.SetBool("Is Partially Open", true);
    }

    public void OnHoverEnd()
    {
        StateAnimator.SetBool("Is Partially Open", false);
    }

    public void OnActivated()
    {
        AppSceneManager.Instance.LoadRemoteScene(destinationUrl);
    }
}
