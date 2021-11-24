using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ScrollArea : LayoutContainer
{
    [SerializeField] private LayoutContainer ContentContainer;
    [SerializeField] private Transform LayoutAreaReference;

    private const float shrinkWindow = 0.05f;

    private Transform[] children;
    private Vector3 refPoint;

    private void Update()
    {
        scaleOutOfBoundsItems();
    }

    private void scaleOutOfBoundsItems()
    {
        if (children == null || children.Length == 0) return;
        foreach(var child in children)
        {
            refPoint = LayoutAreaReference.InverseTransformPoint(child.position);
            child.localScale = Vector3.one *
                refPoint.x.Remap(-0.5f - shrinkWindow, -0.5f + shrinkWindow, 0, 1, true) *
                refPoint.x.Remap(0.5f - shrinkWindow, 0.5f + shrinkWindow, 1, 0, true) *
                refPoint.y.Remap(-0.5f - shrinkWindow, -0.5f + shrinkWindow, 0, 1, true) *
                refPoint.y.Remap(0.5f - shrinkWindow, 0.5f + shrinkWindow, 1, 0, true);
        }
    }


    public void SetScrollPosition(Vector3 LocalPosition)
    {
        ContentContainer.transform.localPosition = LocalPosition;
        constrainScrollPosition();
    }

    public void OffsetScrollPosition(Vector3 Offset)
    {
        ContentContainer.transform.localPosition += Offset;
        constrainScrollPosition();
    }

    private void constrainScrollPosition()
    {
        var contentBounds = ContentContainer.GetBounds();
        contentBounds.center += ContentContainer.transform.localPosition;

        var layoutMin = transform.InverseTransformPoint(LayoutAreaReference.TransformPoint(Vector3.one * -0.5f));
        var layoutMax = transform.InverseTransformPoint(LayoutAreaReference.TransformPoint(Vector3.one * 0.5f));
        var position = ContentContainer.transform.localPosition;

        
        if (contentBounds.min.x > layoutMin.x)
            position.x = layoutMin.x + contentBounds.size.x;
        else if (contentBounds.max.x < layoutMax.x)
            position.x = layoutMax.x;

        //TODO: this doesnt work properly with vertical scrolling
        //if (contentBounds.min.y > layoutMin.y)
        //    position.y = layoutMin.y + contentBounds.size.y;
        //else if (contentBounds.max.y < layoutMax.y)
        //    position.y = layoutMax.y;

        ContentContainer.transform.localPosition = position;
    }

    public override Bounds GetBounds()
    {
        return ContentContainer.GetBounds();
    }

    public override void UpdateLayout()
    {
        ContentContainer.UpdateLayout();
        constrainScrollPosition();
        children = ContentContainer.transform.GetChildren().ToArray();
    }
}