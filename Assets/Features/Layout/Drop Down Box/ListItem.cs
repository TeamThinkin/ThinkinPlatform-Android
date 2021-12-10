using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ListItemDto
{
    public object Value { get; set; }
    public string Text { get; set; }
}

public class ListItem : ButtonInteractable
{
    [SerializeField] TMPro.TMP_Text Label;

    public DropDownBox ParentListControl;
    public ListItemDto Dto { get; private set; }

    public void SetDto(ListItemDto Dto)
    {
        this.Dto = Dto;
        Label.text = Dto.Text;
    }

    protected override void OnActivated(ActivateEventArgs args)
    {
        base.OnActivated(args);
        ParentListControl.ListItem_Selected(this);
    }
}
