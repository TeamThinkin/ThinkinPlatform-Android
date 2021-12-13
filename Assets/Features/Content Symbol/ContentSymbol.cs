using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ContentSymbol : MonoBehaviour
{
    [SerializeField] private GameObject DefaultVisual;
    [SerializeField] private TMP_Text Label;

    private CollectionContentItemDto dto;
    public CollectionContentItemDto Dto => dto;

    public void SetDto(CollectionContentItemDto Dto)
    {
        this.dto = Dto;
        Label.text = dto.DisplayName;
    }
}
