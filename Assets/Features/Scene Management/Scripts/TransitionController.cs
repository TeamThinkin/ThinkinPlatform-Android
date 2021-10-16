using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionController : MonoBehaviour
{
    [SerializeField] Animator TransitionAnimator;

    public event System.Action OnSceneHidden;

    public static TransitionController Instance { get; private set; }

    private System.Action onSceneHiddenCallback;

    private void Awake()
    {
        Instance = this;
    }

    public void HideScene(System.Action OnSceneHiddenCallback = null)
    {
        this.onSceneHiddenCallback = OnSceneHiddenCallback;
        TransitionAnimator.SetBool("IsSolid", true);
    }

    public void RevealScene()
    {
        TransitionAnimator.SetBool("IsSolid", false);
    }

    public void OnSolidfiedEvent()
    {
        onSceneHiddenCallback?.Invoke();
        OnSceneHidden?.Invoke();
    }
}
