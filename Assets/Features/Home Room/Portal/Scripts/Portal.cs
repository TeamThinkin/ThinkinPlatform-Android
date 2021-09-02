using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text Label;
    [SerializeField] Animator StateAnimator;

    private RoomInfo room;

    public void SetModel(RoomInfo room)
    {
        this.room = room;
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
        AppSceneManager.Instance.LoadRoom(room);
    }
}
