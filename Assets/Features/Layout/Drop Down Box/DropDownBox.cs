using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DropDownBox : MonoBehaviour
{
    [SerializeField] private ButtonInteractable ToggleButton;
    [SerializeField] private GameObject List;
    [SerializeField] private AnimationCurve ToggleAnimationCurve;
    [SerializeField] private float ToggleAnimationDuration = 1f;
    [SerializeField] private LayoutContainer ListItemsContainer;
    [SerializeField] private TMPro.TMP_Text SelectedLabel;
    [SerializeField] private GameObject ListItemPrefab;
    [SerializeField] private Transform ListBackground;
    
    private Coroutine animateListCoroutine;
    private bool isListVisible;
    private TypedObjectPool<ListItem> listItems;

    public event Action<ListItemDto> SelectedItemChanged;

    private ListItemDto _selectedItem;
    public ListItemDto SelectedItem
    {
        get { return _selectedItem; }
        set
        {
            if (value == _selectedItem) return;
            _selectedItem = value;
            SelectedLabel.text = _selectedItem.Text;
            SelectedItemChanged?.Invoke(value);
        }
    }

    private void Start()
    {
        listItems = new TypedObjectPool<ListItem>(ListItemPrefab);

        List.transform.localScale = new Vector3(1, 0, 1);
        List.SetActive(false);

        ToggleButton.activated.AddListener(ToggleButton_OnClick);
        ListItemsContainer.UpdateLayout();

        //loadMockOptions();
    }

    private void loadMockOptions()
    {
        var items = new[]
        {
            new ListItemDto() { Value = 101, Text = "Matt" },
            new ListItemDto() { Value = 102, Text = "Kim" },
            new ListItemDto() { Value = 103, Text = "Shuai" },
        };

        SetItems(items);
    }

    private void OnDestroy()
    {
        ToggleButton.activated.RemoveListener(ToggleButton_OnClick);
    }

    public void SetItems(IEnumerable<ListItemDto> Items)
    {
        listItems.Clear();
        foreach(var item in Items)
        {
            var listItem = listItems.Get();
            listItem.ParentListControl = this;
            listItem.SetDto(item);
            listItem.transform.SetParent(ListItemsContainer.transform, false);
        }

        ListItemsContainer.UpdateLayout();
        var listBounds = ListItemsContainer.GetBounds();
        ListBackground.localScale = new Vector3(1, listBounds.size.y, 1);

        SelectedItem = listItems.ActiveItems.First().Dto;
    }

    private void ToggleButton_OnClick(ActivateEventArgs e)
    {
        if (isListVisible) hideList(); else showList();
    }

    public void ListItem_Selected(ListItem Item)
    {
        if (isListVisible) hideList();
        SelectedItem = Item.Dto;
    }

    private void showList()
    {
        List.SetActive(true);
        animateList(1);
        isListVisible = true;
    }

    private void hideList()
    {
        animateList(0, () => List.SetActive(false));
        isListVisible = false;
    }

    private void animateList(float targetScale, Action onCompleteCallback = null)
    {
        AnimationHelper.StartAnimation(this, ref animateListCoroutine, ToggleAnimationDuration, List.transform.localScale.y, targetScale, i => List.transform.localScale = Vector3.one.Scale(1, i, 1), onCompleteCallback, ToggleAnimationCurve);
    }
}