using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ScrollArea : LayoutContainer
{
    [SerializeField] private GameObject _contentContainer;
    [SerializeField] private Transform ZoomContainer;
    [SerializeField] private Transform LayoutAreaReference;
    [SerializeField] private float _zoom = 1;
    [SerializeField] private float MinZoom = 0.05f;
    [SerializeField] private float MaxZoom = 10f;
    [SerializeField] private bool PreventOverscroll;
    [SerializeField] private bool CenterUndersizedContent;
    [SerializeField] private float LayoutPadding;

    private const float shrinkWindow = 0.05f;

    private Vector3 refPoint;
    private Bounds layoutBounds = new Bounds();
    private Bounds contentBounds = new Bounds();
    private Bounds adjustedContentBounds = new Bounds();

    public GameObject ContentContainer => _contentContainer;

    public float Zoom
    {
        get { return _zoom; }
        set 
        { 
            _zoom = Mathf.Clamp(value, MinZoom, MaxZoom);
        }
    }

    private void Awake()
    {
        updateLayoutBounds();
    }

    private void Update()
    {
        ZoomContainer.localScale = Vector3.one * Zoom;
        scaleOutOfBoundsItems();
    }


    public void CenterContent()
    {
        if (!hasContentBounds()) return;
        SetScrollPosition(-contentBounds.center);
    }

    public void SetScrollPosition(Vector3 LocalPosition)
    {
        ContentContainer.transform.localPosition = LocalPosition;
        constrainScrollPosition();
    }

    public void OffsetScrollPosition(Vector3 LocalOffset)
    {
        if (LocalOffset.sqrMagnitude < Mathf.Epsilon) return;
        ContentContainer.transform.localPosition += LocalOffset * (1 / Zoom);
        constrainScrollPosition();
    }


    private void updateLayoutBounds()
    {
        layoutBounds.center = LayoutAreaReference.localPosition;
        layoutBounds.size = LayoutAreaReference.localScale - (LayoutPadding * Vector3.one);
    }

    private void updateContentBounds()
    {
        var layoutItems = ContentContainer.transform.GetChildren().SelectNotNull(i => i.GetComponent<ILayoutItem>()).ToArray();
        bool isFirstItem = true;
        foreach (Transform childTransform in ContentContainer.transform)
        {
            var layoutItem = childTransform.GetComponent<ILayoutItem>();
            if (layoutItem != null)
            {
                var bounds = layoutItem.GetBounds();
                bounds.center += childTransform.localPosition;

                if (isFirstItem)
                {
                    contentBounds = bounds;
                    isFirstItem = false;
                }
                else
                {
                    contentBounds.Encapsulate(bounds);
                }
            }
        }

        updateAdjustContentBounds();
    }

    private void updateAdjustContentBounds()
    {
        adjustedContentBounds = contentBounds;
        adjustedContentBounds.center += ContentContainer.transform.localPosition;
        adjustedContentBounds.center *= Zoom;
        adjustedContentBounds.size *= Zoom;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(layoutBounds.center, layoutBounds.size);
        Gizmos.color = Color.red;
        Gizmos.DrawCube(adjustedContentBounds.center, adjustedContentBounds.size);
    }

    private void scaleOutOfBoundsItems()
    {
        foreach (Transform child in ContentContainer.transform)
        {
            refPoint = LayoutAreaReference.InverseTransformPoint(child.position);
            child.localScale = Vector3.one *
                refPoint.x.Remap(-0.5f - shrinkWindow, -0.5f + shrinkWindow, 0, 1, true) *
                refPoint.x.Remap(0.5f - shrinkWindow, 0.5f + shrinkWindow, 1, 0, true) *
                refPoint.y.Remap(-0.5f - shrinkWindow, -0.5f + shrinkWindow, 0, 1, true) *
                refPoint.y.Remap(0.5f - shrinkWindow, 0.5f + shrinkWindow, 1, 0, true);
        }
    }

    private bool hasContentBounds()
    {
        return contentBounds.size.x > 0;
    }

    private void constrainScrollPosition()
    {
        if (!hasContentBounds()) return;
        updateAdjustContentBounds();

        var position = ContentContainer.transform.localPosition;

        if (adjustedContentBounds.max.x < layoutBounds.min.x)
            position.x = (layoutBounds.min.x - adjustedContentBounds.extents.x - contentBounds.center.x) * (1 / Zoom);
        else if (adjustedContentBounds.min.x > layoutBounds.max.x)
            position.x = (layoutBounds.max.x + adjustedContentBounds.extents.x - contentBounds.center.x) * (1 / Zoom);

        if (adjustedContentBounds.max.y < layoutBounds.min.y)
            position.y = (layoutBounds.min.y - adjustedContentBounds.extents.y - contentBounds.center.y) * (1 / Zoom);
        else if (adjustedContentBounds.min.y > layoutBounds.max.y)
            position.y = (layoutBounds.max.y + adjustedContentBounds.extents.y - contentBounds.center.y) * (1 / Zoom);

        if(PreventOverscroll)
        {
            if (CenterUndersizedContent && adjustedContentBounds.size.x < layoutBounds.size.x)
                position.x = layoutBounds.center.x - contentBounds.center.x;
            if (adjustedContentBounds.min.x < layoutBounds.min.x)
                position.x = (layoutBounds.min.x + adjustedContentBounds.extents.x - contentBounds.center.x) * (1 / Zoom);
            else if (adjustedContentBounds.max.x > layoutBounds.max.x)
                position.x = (layoutBounds.max.x - adjustedContentBounds.extents.x - contentBounds.center.x) * (1 / Zoom);
        }

        ContentContainer.transform.localPosition = position;
    }

    public override Bounds GetBounds()
    {
        return layoutBounds;
        //return contentLayoutContainer.GetBounds();
    }

    public override void UpdateLayout()
    {
        updateContentBounds();
        constrainScrollPosition();
    }
}
