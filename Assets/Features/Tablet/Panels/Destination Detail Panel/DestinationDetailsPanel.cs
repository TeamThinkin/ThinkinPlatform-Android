using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DestinationDetailsPanel : TabPanel
{
    [SerializeField] private NewMapDestinationWizard Wizard;
    [SerializeField] private MapView MapContents;
    [SerializeField] private Textbox NameTextbox;

    public string MapUrl;

    private MapItem newMapItem;
    private ExpandingRegionSampler solver = new ExpandingRegionSampler();

    public string DestinationName
    {
        get { return NameTextbox.Text; }
        set { NameTextbox.Text = value; }
    }

    public Vector3 Location
    {
        get;
        set;
    }

    private void Awake()
    {
        NameTextbox.Changed += NameTextbox_Changed;
    }

    private void NameTextbox_Changed(Textbox sender)
    {
        if (newMapItem == null) return;
        Wizard.ItemDto.DisplayName = DestinationName;
        newMapItem.Symbol.UpdateFromDto();
    }

    protected override async void OnShow()
    {
        base.OnShow();

        DestinationName = Wizard.ItemDto.DisplayName;
        await MapContents.LoadCollection(MapUrl);
    
        newMapItem = MapContents.AddItemFromDto(Wizard.ItemDto);
        newMapItem.Symbol.IsHighlighted = true;
        if (Wizard.ItemDto.Placement == null) findInitialPosition();
        MapContents.ContentContainer.CenterViewOnItem(newMapItem.transform);
    }

    private Vector3 getInitialPositionGuess()
    {
        var isFirstItem = true;
        int count = 0;
        Vector3 position = Vector3.zero;
        foreach (var item in MapContents.Items)
        {
            if (item == newMapItem) continue;
            if (isFirstItem)
            {
                position = item.Dto.Placement.Position;
            }
            else
            {
                position += item.Dto.Placement.Position;
            }
            isFirstItem = false;
            count++;
        }
        position /= count;
        return position;
    }

    private void findInitialPosition()
    {
        Wizard.ItemDto.Placement = new PlacementDto();
        Wizard.ItemDto.Placement.Position = newMapItem.transform.localPosition = getInitialPositionGuess();
        Wizard.ItemDto.Placement.Scale = 1;

        var options = new LayoutSolverParams();
        var otherItems = MapContents.Items.Where(i => i != newMapItem).ToArray();
        var existingItemBounds = new Bounds[otherItems.Length];
        for(int i=0;i<otherItems.Length;i++)
        {
            var item = otherItems[i];
            var bounds = item.GetComponentInChildren<BlockLayoutItem>().GetBounds();
            bounds.center += item.transform.localPosition;
            existingItemBounds[i] = bounds;
        }

        options.ExistingItems = existingItemBounds;
        options.LayoutArea = new Bounds(Vector3.zero, MapContents.ContentContainer.transform.localScale);
        options.NewItemBounds = newMapItem.Symbol.LayoutItem.GetBounds();
        options.NewItemBounds.center = Wizard.ItemDto.Placement.Position;

        solver.StartSolve(options);
        solver.Iterate(10000);
        Debug.Log("Solve is intersecting: " + solver.IsIntersecting);

        Wizard.ItemDto.Placement.Position = newMapItem.transform.localPosition = solver.Params.NewItemBounds.center;
    }
}
