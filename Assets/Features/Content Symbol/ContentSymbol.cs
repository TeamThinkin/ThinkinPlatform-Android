using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ContentSymbol : MonoBehaviour
{
    [SerializeField] private GameObject DefaultVisual;
    [SerializeField] private TMP_Text Label;
    [SerializeField] private Material HighlightMaterial;
    [SerializeField] private Renderer SymbolRenderer;
    [SerializeField] private BlockLayoutItem _layoutItem;

    private bool _isHighlighted;
    public bool IsHighlighted
    {
        get { return _isHighlighted; }
        set
        {
            _isHighlighted = value;
            SymbolRenderer.sharedMaterial = _isHighlighted ? HighlightMaterial : normalMaterial;
        }
    }
    
    public CollectionContentItemDto Dto => dto;
    public ILayoutItem LayoutItem => _layoutItem;

    private CollectionContentItemDto dto;
    private Material normalMaterial;

    public void SetDto(CollectionContentItemDto Dto)
    {
        this.dto = Dto;
        Label.text = dto.DisplayName;
    }

    public void UpdateFromDto()
    {
        Label.text = dto.DisplayName;
    }

    private void Awake()
    {
        normalMaterial = SymbolRenderer.sharedMaterial;
        IsHighlighted = IsHighlighted;
    }
}
