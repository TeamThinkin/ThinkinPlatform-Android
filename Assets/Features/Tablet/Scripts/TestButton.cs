using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TestButton : MonoBehaviour
{
    public TextMeshPro Label;

    public void DoTheThing()
    {
        Debug.Log("The thing has been done: " + Random.Range(0, 100));
        Label.text = Random.Range(0, 100).ToString();
    }

    public void HoverStart()
    {
        transform.localScale = Vector3.one * 1.1f;
    }

    public void HoverEnd()
    {
        transform.localScale = Vector3.one;
    }
}
