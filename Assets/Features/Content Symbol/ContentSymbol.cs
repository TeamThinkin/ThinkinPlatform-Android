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
    [SerializeField] private Transform VisualContainer;
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
    private IContentItemPresenter visual;

    public async void SetDto(CollectionContentItemDto Dto)
    {
        this.dto = Dto;
        Label.text = dto.DisplayName;

        ///////////////////

        if (visual != null) Destroy(visual.GameObject);

        visual = await PresenterFactory.Instance.Instantiate(Dto, VisualContainer, true);
        DefaultVisual.SetActive(visual == null || !visual.HasVisual);
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
