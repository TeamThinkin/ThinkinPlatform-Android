using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionController : MonoBehaviour
{
    [SerializeField] Animator TransitionAnimator;

    public event System.Action OnSceneHidden;

    public void HideScene()
    {
        TransitionAnimator.SetBool("IsSolid", true);
    }

    public void RevealScene()
    {
        TransitionAnimator.SetBool("IsSolid", false);
    }

    public void OnSolidfiedEvent()
    {
        OnSceneHidden?.Invoke();
    }
}
