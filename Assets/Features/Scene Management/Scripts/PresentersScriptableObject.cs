using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ContentItemPresenters", menuName = "ScriptableObjects/Presenters List")]
public class PresentersScriptableObject : ScriptableObject
{
    public GameObject[] Presenters;
}
