using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ExpandingRegionSampler
{
    public LayoutSolverParams Params;

    private float radius;
    private float radiusIncrement;
    private float radiusMax;
    private Vector3 searchOrigin;

    public bool IsIntersecting;

    public void StartSolve(LayoutSolverParams Params)
    {
        this.Params = Params;
        radius = 0; 
        radiusMax = Params.LayoutArea.extents.magnitude;
        radiusIncrement = 0.01f; // radiusMax / 1000;
        searchOrigin = Params.NewItemBounds.center;
    }

    public bool Iterate(int Count)
    {
        for (int i = 0; i < Count; i++)
        {
            radius += radiusIncrement;
            Params.NewItemBounds.center = searchOrigin + ((Vector3)Random.insideUnitCircle * radius);
            IsIntersecting = Params.ExistingItems.Any(i => i.Intersects(Params.NewItemBounds));
            if (!IsIntersecting) return true;
        }
        return false;
    }
}

public class LayoutSolverParams
{
    public Bounds NewItemBounds;
    public Bounds[] ExistingItems;
    public Bounds LayoutArea;
}
