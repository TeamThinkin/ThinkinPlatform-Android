using System.Collections;
using UnityEngine;

public class ElementPresenterAttribute : System.Attribute
{
    public string Tag { get; private set; }
    public string PrefabPath { get; private set; }


    public ElementPresenterAttribute(string Tag, string PrefabPath)
    {
        this.Tag = Tag.ToUpper();
        this.PrefabPath = PrefabPath;
    }
}