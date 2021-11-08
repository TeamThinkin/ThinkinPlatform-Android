using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackPanel : LayoutContainer
{
    [SerializeField] private Vector3 StackDirection = Vector3.down;
    [SerializeField] private Vector3 ItemPadding = Vector3.zero;
    [SerializeField] private Transform BoundsVisualizer;

    private Bounds bounds = new Bounds();

    public override void UpdateLayout()
    {
        Vector3 value = Vector3.zero;
        float length;
        float totalLength = 0;

        bounds.center = value;
        bounds.size = Vector3.zero;

        foreach (Transform child in transform)
        {
            if (!child.gameObject.activeSelf) continue;

            var layoutItem = child.GetComponent<ILayoutItem>();
            if (layoutItem != null)
            {
                var itemBounds = layoutItem.GetBounds();
                itemBounds.size += ItemPadding;
                length = Mathf.Abs(Vector2.Dot(StackDirection, itemBounds.size));
                totalLength += length;

                child.localPosition = value + (StackDirection * (length / 2));
                itemBounds.center = child.localPosition;
                bounds.Encapsulate(itemBounds);

                value += length * StackDirection;
            }
        }

        ////Center contents
        //var offset = StackDirection * (totalLength / -2);
        //foreach (Transform child in transform)
        //{
        //    child.localPosition += offset;
        //}

        BoundsVisualizer.transform.localPosition = bounds.center;
        BoundsVisualizer.transform.localScale = bounds.size;
    }

    public override Bounds GetBounds()
    {
        return bounds;
    }
}