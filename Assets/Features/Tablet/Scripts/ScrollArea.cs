using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ScrollArea : LayoutContainer//XRBaseInteractable
{
    [SerializeField] private LayoutContainer ContentContainer;
    [SerializeField] private Transform LayoutAreaReference;

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

        //var contentLeft = contentBounds.center.x - contentBounds.extents.x;
        //var contentRight = contentBounds.center.x + contentBounds.extents.x;
        //var contentBottom= contentBounds.center.y - contentBounds.extents.y;
        //var contentTop = contentBounds.center.y + contentBounds.extents.y;

        var layoutMin = transform.InverseTransformPoint(LayoutAreaReference.TransformPoint(Vector3.one * -0.5f));
        var layoutMax = transform.InverseTransformPoint(LayoutAreaReference.TransformPoint(Vector3.one * 0.5f));

        Debug.Log("layoutMin.x: " + layoutMin.x.ToString("0.0000"));
        Debug.Log("contentMin.x: " + contentBounds.min.x.ToString("0.0000"));

        //if (contentBounds.max.x - contentBounds.min.x < layoutMax.x - layoutMin.x)
        //    ContentContainer.transform.localPosition = layoutMin.IsolateX();
        if (contentBounds.min.x > layoutMin.x)
            ContentContainer.transform.localPosition = Vector3.right * (layoutMin.x + contentBounds.size.x);
        else if (contentBounds.max.x < layoutMax.x)
            ContentContainer.transform.localPosition = Vector3.right * (layoutMax.x);
        //ContentContainer.transform.localPosition = Vector3.right * (layoutMax.x - contentBounds.size.x);

    }

    public override Bounds GetBounds()
    {
        return ContentContainer.GetBounds();
    }

    public override void UpdateLayout()
    {
        ContentContainer.UpdateLayout();
        constrainScrollPosition();
    }
}
