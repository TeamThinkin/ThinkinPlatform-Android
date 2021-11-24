using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CollectionList : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text Label;

    public void SetItems(CollectionContentItemDto[] Items)
    {
        Label.text = string.Join("\n", Items.Select(i => "- " + i.DisplayName));
    }
}
