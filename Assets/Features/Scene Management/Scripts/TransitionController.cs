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
        gameObject.SetActive(true);
        this.onSceneHiddenCallback = OnSceneHiddenCallback;

        TransitionAnimator.SetBool("IsSolid", true);
    }

    public void RevealScene()
    {
        TransitionAnimator.SetBool("IsSolid", false);
    }

    public void OnSolidfiedEvent() //Called from Animation Event
    {
        onSceneHiddenCallback?.Invoke();
        OnSceneHidden?.Invoke();
    }

    public void OnDissolvedEvent() //Called from Animation Event
    {
        gameObject.SetActive(false);
    }
}
