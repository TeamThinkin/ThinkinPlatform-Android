using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILayoutItem
{
    Bounds GetBounds();
    void UpdateLayout();
}