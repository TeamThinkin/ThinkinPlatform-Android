using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentListItem : MonoBehaviour, IHandlePointerEvent
{
    [SerializeField] private GameObject SelectedIndicator;
    [SerializeField] private GameObject HoverIndicator;
    [SerializeField] private ContentSymbol Symbol;

    public CollectionContentItemDto Dto { get; private set; }

    public bool IsItemSelected
    {
        get { return SelectedIndicator.activeSelf; }
        set { SelectedIndicator.SetActive(value); }
    }

    public bool IsItemHovered
    {
        get { return HoverIndicator.activeSelf; }
        private set { HoverIndicator.SetActive(value); }
    }

    private void Start()
    {
        IsItemSelected = false;
        IsItemHovered = false;
    }

    public void SetDto(CollectionContentItemDto Dto)
    {
        this.Dto = Dto;
        Symbol.SetDto(Dto);
    }

    public void OnHoverStart(UIPointer Sender, RaycastHit RayInfo)
    {
        IsItemHovered = true;
    }

    public void OnHoverEnd(UIPointer Sender)
    {
        IsItemHovered = false;
    }

    public void OnGripStart(UIPointer Sender, RaycastHit RayInfo)
    {
        Debug.Log("Content List Item: Grip Start");
    }

    public void OnGripEnd(UIPointer Sender)
    {
    }

    public void OnTriggerStart(UIPointer Sender, RaycastHit RayInfo)
    {
        Debug.Log("Content List Item: Trigger Press");
    }

    public void OnTriggerEnd(UIPointer Sender)
    {
    }
}
