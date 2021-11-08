using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//public interface ILayoutContainer : ILayoutItem
//{
//    public void UpdateLayout();
//}

public abstract class LayoutContainer : MonoBehaviour, ILayoutItem
{
    public abstract Bounds GetBounds();

    public abstract void UpdateLayout();

    protected IEnumerable<ILayoutItem> GetChildLayoutItems()
    {
        return transform.GetChildren().SelectNotNull(i => i.GetComponent<ILayoutItem>());
    }

    protected virtual void UpdateChildrenLayouts()
    {
        foreach(var child in GetChildLayoutItems())
        {
            child.UpdateLayout();
        }
    }
}