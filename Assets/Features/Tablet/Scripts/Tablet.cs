using Normal.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tablet : MonoBehaviour
{
    [SerializeField] private LayoutContainer MenuContentContainer;
    [SerializeField] private LayoutContainer MenuScrollArea;

    private void Start()
    {
        MenuScrollArea.UpdateLayout();
    }
}
