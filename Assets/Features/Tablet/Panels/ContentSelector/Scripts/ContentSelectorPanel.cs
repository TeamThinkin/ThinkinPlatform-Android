using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ContentSelectorPanel : TabPanel
{
    public string[] ContentTypeFilters;

    [SerializeField] private MapPanel MapPanel;
    [SerializeField] private GameObject BusyIndicator;
    [SerializeField] private GameObject ListItemPrefab;
    [SerializeField] private GameObject TagButtonPrefab;
    [SerializeField] private LayoutContainer ContentItemsContainer;
    [SerializeField] private LayoutContainer TagsContainer;

    private Queue<bool> busyQueue = new Queue<bool>();
    private List<string> requestedUrls = new List<string>();
    private List<string> tags = new List<string>();
    private List<CollectionContentItemDto> contentItemDtos = new List<CollectionContentItemDto>();
    private TypedObjectPool<ContentListItem> contentItemVisualPool;
    private TypedObjectPool<ToggleButton> tagButtonPool;
    private ToggleButton activeTagButton;
    private string tagFilter;
    private ContentListItem selectedListItem;

    private void Start()
    {
        contentItemVisualPool = new TypedObjectPool<ContentListItem>(ListItemPrefab, contentItemVisualPool_Get, contentItemVisualPool_Released);
        tagButtonPool = new TypedObjectPool<ToggleButton>(TagButtonPrefab, tagButtonPool_Get, tagButtonPool_Released);
        BusyIndicator.SetActive(false);

        UserInfo.OnCurrentUserChanged += UserInfo_OnCurrentUserChanged;
    }

    private void tagButtonPool_Get(ToggleButton item)
    {
        item.activated.AddListener(tagButton_Clicked);
    }

    private void tagButtonPool_Released(ToggleButton item)
    {
        item.activated.RemoveListener(tagButton_Clicked);
    }

    private void contentItemVisualPool_Get(ContentListItem item)
    {
        item.activated.AddListener(contentItem_Clicked);
    }

    private void contentItemVisualPool_Released(ContentListItem item)
    {
        item.activated.RemoveListener(contentItem_Clicked);
        item.IsItemSelected = false;
        if (selectedListItem == item) selectedListItem = null;
    }


    private void OnDestroy()
    {
        UserInfo.OnCurrentUserChanged -= UserInfo_OnCurrentUserChanged;
    }

    public void OnBackButton_Pressed()
    {
        ParentTabView.ShowTab(MapPanel);
    }

    public void OnNextButton_Pressed()
    {
        if (selectedListItem == null) return;

        Debug.Log("Moving on with: " + selectedListItem.Dto.DisplayName);
    }

    private void UserInfo_OnCurrentUserChanged(UserInfo obj)
    {
        fetchContent();
    }

    private void contentItem_Clicked(ActivateEventArgs e)
    {
        if(selectedListItem != null) selectedListItem.IsItemSelected = false;
       
        selectedListItem = e.interactable as ContentListItem;
        selectedListItem.IsItemSelected = true;
    }

    private void tagButton_Clicked(ActivateEventArgs e)
    {
        var button = e.interactable as ToggleButton;
        var tag = button.Key as string;
        if (activeTagButton != null) activeTagButton.IsToggleActive = false;
        if (tag != tagFilter)
        {
            activeTagButton = button;
            activeTagButton.IsToggleActive = true;
            tagFilter = tag;
        }
        else
        {
            activeTagButton = null;
            tagFilter = null;
        }
        refreshContentItemVisuals();
    }

    protected override void OnShow()
    {
        base.OnShow();

        fetchContent();
    }

    private void fetchContent()
    {
        if (!UserInfo.IsLoggedIn) return;
        requestedUrls.Clear();
        tags.Clear();
        tagButtonPool.Clear();
        contentItemDtos.Clear();
        contentItemVisualPool.Clear();
        fetchCollection(CollectionManager.PublicCollectionUrl);
        fetchCollection(CollectionManager.UserRootCollectionUrl);
    }

    private void startBusy()
    {
        if(busyQueue.Count == 0)
        {
            BusyIndicator.SetActive(true);
        }
        busyQueue.Enqueue(true);
    }

    private void completeBusy()
    {
        busyQueue.Dequeue();
        if(busyQueue.Count == 0)
        {
            BusyIndicator.SetActive(false);
        }
    }

    private async void fetchCollection(string url)
    {
        if(requestedUrls.Contains(url)) return;

        startBusy();
        requestedUrls.Add(url);

        var contentItems = await CollectionManager.GetCollectionContents(url);
        if (contentItems != null)
        {
            foreach (var contentItem in contentItems)
            {
                if (ContentTypeFilters == null || ContentTypeFilters.Length == 0 || ContentTypeFilters.Contains(contentItem.Type))
                {
                    addContentItemDto(contentItem);
                    addContentItemVisual(contentItem);
                }

                if (contentItem.MimeType == "link/collection")
                {
                    var linkItem = contentItem as CollectionLinkContentItemDto;
                    fetchCollection(linkItem.Url);
                }
            }

            ContentItemsContainer.UpdateChildrenLayouts();
            ContentItemsContainer.UpdateLayout();
            TagsContainer.UpdateChildrenLayouts();
            TagsContainer.UpdateLayout();
        }

        completeBusy();
    }

    private void refreshContentItemVisuals()
    {
        contentItemVisualPool.Clear();
        foreach (var dto in contentItemDtos)
        {
            addContentItemVisual(dto);
        }
        ContentItemsContainer.UpdateChildrenLayouts();
        ContentItemsContainer.UpdateLayout();
    }

    private void addContentItemDto(CollectionContentItemDto dto)
    {
        if (contentItemDtos.Contains(dto)) return;
        contentItemDtos.Add(dto);

        if (dto.Tags != null)
        {
            foreach (var tag in dto.Tags)
            {
                if (!tags.Contains(tag)) addTag(tag);
            }
        }
    }

    private void addContentItemVisual(CollectionContentItemDto dto)
    {
        if (string.IsNullOrEmpty(tagFilter) || (dto.Tags != null && dto.Tags.Any(i => i == tagFilter)))
        {
            var item = contentItemVisualPool.Get();
            item.SetDto(dto);
            item.transform.SetParent(ContentItemsContainer.ContentContainer.transform, false);
        }
    }

    private void addTag(string tag)
    {
        tags.Add(tag);

        var button = tagButtonPool.Get();
        button.Key = tag;
        button.Text = tag;
        button.transform.SetParent(TagsContainer.ContentContainer.transform, false);
    }
}
