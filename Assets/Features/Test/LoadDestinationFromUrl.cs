using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadDestinationFromUrl : MonoBehaviour
{
    [SerializeField] private string Url;
    [SerializeField] private DestinationPresenter DestinationPresenter;
    
    async void Start()
    {
        await DestinationPresenter.DisplayUrl(Url);
    }
}
