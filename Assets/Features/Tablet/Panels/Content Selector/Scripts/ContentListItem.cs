using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentListItem : MonoBehaviour // : ButtonInteractable
{
    [SerializeField] private GameObject SelectedIndicator;
    [SerializeField] private ContentSymbol Symbol;

    public CollectionContentItemDto Dto { get; private set; }

    public bool IsItemSelected
    {
        get { return SelectedIndicator.activeSelf; }
        set { SelectedIndicator.SetActive(value); }
    }

    private void Start()
    {
        IsItemSelected = false;
    }

    public void SetDto(CollectionContentItemDto Dto)
    {
        this.Dto = Dto;
        Symbol.SetDto(Dto);
    }
}
