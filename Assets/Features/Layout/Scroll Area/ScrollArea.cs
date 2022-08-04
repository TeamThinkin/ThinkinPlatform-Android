using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScrollArea : LayoutContainer
{
    [SerializeField] private GameObject _contentContainer;
    [SerializeField] private Transform ZoomContainer;
    [SerializeField] private Transform LayoutAreaReference;
    [SerializeField] private Transform ScrollIndicator;
    [SerializeField] private Transform ScrollRangeIndicator;
    [SerializeField] private Transform ScrollPositionIndicator;
    [SerializeField] private float _zoom = 1;
    [SerializeField] private float MinZoom = 0.05f;
    [SerializeField] private float MaxZoom = 10f;
    [SerializeField] private bool PreventOverscroll;
    [SerializeField] private bool CenterUndersizedContent;
    [SerializeField] private float LayoutPadding;
    [SerializeField] private bool HideOutOfBoundsItems = true;

    private const float shrinkWindow = 0.05f;

    private Vector3 refPoint;
    private Bounds layoutBounds = new Bounds();
    private Bounds contentBounds = new Bounds();
    private Bounds adjustedContentBounds = new Bounds();
    private LayoutContainer contentLayoutContainer;

    public override GameObject ContentContainer => _contentContainer;

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
        contentLayoutContainer = _contentContainer.GetComponent<LayoutContainer>();
        updateLayoutBounds();
        UpdateLayout();
    }

    private void Update()
    {
        ZoomContainer.localScale = Vector3.one * Zoom;
        if(HideOutOfBoundsItems) scaleOutOfBoundsItems();
    }


    public void CenterContent()
    {
        if (!hasContentBounds()) return;
        var center = -contentBounds.center;
        //center.z = contentBounds.max.z;
        center.z = 0;
        ZoomToFitContents();
        SetScrollPosition(center);
    }

    public void ZoomToFitContents()
    {
        //TODO: implement this
        Zoom = 0.5f;
    }

    public void CenterViewOnItem(Transform Item)
    {
        var center = -Item.localPosition;
        //center.z = contentBounds.min.z;
        center.z = 0;
        SetScrollPosition(center);
    }

    public void SetScrollPosition(Vector3 LocalPosition)
    {
        ContentContainer.transform.localPosition = LocalPosition;
        constrainScrollPosition();
        updateScrollIndicator();
    }

    public void OffsetScrollPosition(Vector3 LocalOffset)
    {
        if (LocalOffset.sqrMagnitude < Mathf.Epsilon) return;
        ContentContainer.transform.localPosition += LocalOffset * (1 / Zoom);
        constrainScrollPosition();
        updateScrollIndicator();
    }

    private void updateScrollIndicator()
    {
        if (ScrollRangeIndicator == null || ScrollPositionIndicator == null) return;

        if (isAdjustContentSmallerThanLayout())
        {
            ScrollIndicator.gameObject.SetActive(false);
        }
        else
        {
            float v = ContentContainer.transform.localPosition.x;
            float minEdge = (layoutBounds.min.x + adjustedContentBounds.extents.x - contentBounds.center.x) * (1 / Zoom);
            float maxEdge = (layoutBounds.max.x - adjustedContentBounds.extents.x - contentBounds.center.x) * (1 / Zoom);
            float p = ((v - minEdge) / (maxEdge - minEdge)).Remap(1, 0, -0.5f, 0.5f, true);
            ScrollPositionIndicator.position = ScrollRangeIndicator.TransformPoint(Vector3.right * p);
            ScrollIndicator.gameObject.SetActive(true);
        }
    }


    private void updateLayoutBounds()
    {
        layoutBounds.center = LayoutAreaReference.localPosition;
        layoutBounds.size = LayoutAreaReference.localScale - (LayoutPadding * Vector3.one);
    }

    private void updateContentBounds()
    {
        if (contentLayoutContainer != null)
        {
            contentBounds = contentLayoutContainer.GetBounds();
        }
        else
        {
            var layoutItems = ContentContainer.transform.GetChildren().SelectNotNull(i => i.GetComponent<ILayoutItem>()).ToArray();
            bool isFirstItem = true;
            foreach (Transform childTransform in ContentContainer.transform)
            {
                var layoutItem = childTransform.GetComponentInChildren<ILayoutItem>();
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
            if(isAdjustContentSmallerThanLayout())
            {
                //Content is smaller than the layout area
                if(CenterUndersizedContent)
                    position.x = layoutBounds.center.x - contentBounds.center.x;
                else if (adjustedContentBounds.min.x < layoutBounds.min.x)
                    position.x = getAlignToMinEdgePosition();
                else if (adjustedContentBounds.max.x > layoutBounds.max.x)
                    position.x = getAlignToMaxEdgePosition();
            }
            else
            {
                //Content is larger than the layout area
                if (adjustedContentBounds.min.x > layoutBounds.min.x)
                    position.x = getAlignToMinEdgePosition();
                else if (adjustedContentBounds.max.x < layoutBounds.max.x)
                    position.x = getAlignToMaxEdgePosition();
            }
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
        if (contentLayoutContainer != null) contentLayoutContainer.UpdateLayout();

        updateContentBounds();
        constrainScrollPosition();
        updateScrollIndicator();
    }

    private bool isAdjustContentSmallerThanLayout()
    {
        return adjustedContentBounds.size.x < layoutBounds.size.x;
    }

    private float getAlignToMinEdgePosition()
    {
        return (layoutBounds.min.x + adjustedContentBounds.extents.x - contentBounds.center.x) * (1 / Zoom); //align to the min edge
    }

    private float getAlignToMaxEdgePosition()
    {
        return (layoutBounds.max.x - adjustedContentBounds.extents.x - contentBounds.center.x) * (1 / Zoom); //align to the max edge
    }
}
