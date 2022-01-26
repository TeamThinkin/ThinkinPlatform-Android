using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestinationDetailsPanel : TabPanel
{
    [SerializeField] private MapView MapContents;
    [SerializeField] private TMPro.TMP_Text NameTextbox;

    public string MapUrl;

    public string DestinationName
    {
        get { return NameTextbox.text; }
        set { NameTextbox.text = value; }
    }

    public Vector3 Location
    {
        get;
        set;
    }

    protected override void OnShow()
    {
        base.OnShow();
        
        MapContents.LoadCollection(MapUrl);
    }
}
