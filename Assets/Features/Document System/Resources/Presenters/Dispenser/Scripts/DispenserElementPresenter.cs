using AngleSharp.Dom;
using AngleSharp.XPath;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using Autohand;

[ElementPresenter("dispenser", "Presenters/Dispenser/Dispenser", false)]
public class DispenserElementPresenter : ElementPresenterBase
{
    public class ItemInfo
    {
        public string AssetSourceUrl;
        public string AssetName;
        public GameObject Prefab;
        public GameObject Instance;
        public Vector3 LocalPosition;
        public ConnectingLine Line;
    }

    [SerializeField] private TMPro.TMP_Text Label;
    [SerializeField] private GameObject LinePrefab;
    [SerializeField] private ScrollGestureZone GestureInput;
    [SerializeField] private Transform ContentContainer;
    [SerializeField] private Transform LayoutReference;
    [SerializeField] private PhysicMaterial DefaultMaterial;
    [SerializeField] private float ItemSize = 1;
    [SerializeField] private int RowCount = 3;
    [SerializeField] private Vector3 JitterScale = Vector3.one;
    [SerializeField] private float Scroll;
    [SerializeField] private float ScrollMargin;

    public bool IsPolar;

    private string src;
    private string title;
    private PlacementInfo placement;
    private ItemInfo[] items;
    private float minScroll;
    private float maxScroll;
    private int itemCounter;
    private DispenserSync sync;

    public override void ParseDataElement(IElement ElementData)
    {
        src = ElementData.Attributes["src"]?.Value;
        title = ElementData.Attributes["title"]?.Value;
        placement = GetPlacementInfo(ElementData);
    }

    public override async Task Initialize()
    {
        Label.text = title;
        ApplyPlacement(placement, this.transform);
        ContentContainer.ClearChildren();

        if (!string.IsNullOrEmpty(src))
        {
            //var address = new AssetUrl(src);
            //var request = UnityEngine.Networking.UnityWebRequestAssetBundle.GetAssetBundle(address.CatalogUrl, 0);
            //await request.SendWebRequest().GetTask();
            //var bundle = UnityEngine.Networking.DownloadHandlerAssetBundle.GetContent(request);

            var bundle = await AssetBundleManager.LoadAssetBundle(src);
            var names = bundle.GetAllAssetNames().Take(30);

            items = names.Select(i => getItem(i, bundle)).ToArray(); //TODO: need to manage how these things get in and out of memory

            updateLayout();
        }
    }

    public override void CreateNetworkSync()
    {
        base.CreateNetworkSync();

        sync = DispenserSync.FindOrCreate(this);
    }

    public void AttachNetworkSync(DispenserSync Sync)
    {
        this.sync = Sync;
    }

    public int GetNextItemId()
    {
        if (sync != null) sync.RequestOwnership();
        return itemCounter++;
    }

    private ItemInfo getItem(string assetName, AssetBundle bundle)
    {
        var item = new ItemInfo();

        item.AssetName = assetName;
        item.AssetSourceUrl = src + "#" + item.AssetName;
        item.Prefab = bundle.LoadAsset<GameObject>(item.AssetName); //TODO: move this over to the async version

        var instance = Instantiate(item.Prefab, ContentContainer);
        instance.transform.localScale = ItemSize * Vector3.one;
        instance.transform.localRotation = Quaternion.Euler(0, 180, 0);
        var dispenserItem = instance.AddComponent<DispenserItem>();
        dispenserItem.SetItemInfo(item);
        dispenserItem.ParentDispenser = this;

        //var colliders = instance.GetComponentsInChildren<Collider>();
        //foreach(var collider in colliders)
        //{
        //    collider.enabled = false;
        //}

        item.Instance = instance;

        item.Line = Instantiate(LinePrefab, ContentContainer).GetComponent<ConnectingLine>();
        item.Line.TargetItem = item.Instance.transform;
        return item;
    }

    private void Awake()
    {
        GestureInput.OnUserInput += GestureInput_OnUserInput;
    }
    private void OnDestroy()
    {
        GestureInput.OnUserInput -= GestureInput_OnUserInput;
    }

    private void Update()
    {
        handleSync();
        trackGestureInput();
        updateLayout();
        updateScroll();
    }

    private void GestureInput_OnUserInput()
    {
        if (sync != null) sync.RequestOwnership();
    }

    private void handleSync()
    {
        if (sync == null) return;

        if(sync.isOwnedRemotelySelf)
        {
            Scroll = sync.Model.scrollValue;
            itemCounter = sync.Model.counter;
        }
        else
        {
            sync.Model.scrollValue = Scroll;
            sync.Model.counter = itemCounter;
        }
    }

    private void trackGestureInput()
    {
        Scroll += GestureInput.ScrollValue;
    }

    private void updateScroll()
    {
        //TODO: this code and updateLayout() need to be evaluated for cleanliness
        if (items == null) return;

        var width = LayoutReference.localScale.x;
        var left = Scroll - width / 2;
        var right = Scroll + width / 2;

        if (IsPolar)
        {
            var circumference = LayoutReference.localPosition.y * 2 * Mathf.PI;
            var rot = (Scroll / circumference) * 360;
            
            ContentContainer.localPosition = Vector3.zero;
            ContentContainer.localRotation = Quaternion.Euler(0, 180, 0); // rot);
        }
        else
        {
            ContentContainer.localPosition = Vector3.right * Scroll;
            ContentContainer.localRotation = Quaternion.Euler(0, 180, 0);
        }

        foreach (var item in items)
        {
            if (IsPolar) item.Instance.transform.localPosition = FromPolar(item.LocalPosition + Vector3.left * Scroll);
            if (item.LocalPosition.x >= left && item.LocalPosition.x <= right)
            {
                var leftScale = item.LocalPosition.x.Remap(left, left + ItemSize, 0, 1, true);
                var rightScale = item.LocalPosition.x.Remap(right - ItemSize, right, 1, 0, true);
                var scale = Mathf.Min(leftScale, rightScale);
                item.Instance.transform.localScale = scale * ItemSize * Vector3.one;

                item.Instance.SetActive(true);
                item.Line.gameObject.SetActive(true);
                item.Line.SetSize(scale);
            }
            else
            {
                item.Instance.SetActive(false);
                item.Line.gameObject.SetActive(false);
            }
        }

        minScroll = items.Min(i => i.LocalPosition.x) + ScrollMargin;
        maxScroll = items.Max(i => i.LocalPosition.x) - ScrollMargin;
        Scroll = Mathf.Clamp(Scroll, minScroll, maxScroll);
    }

    private void updateLayout()
    {
        if (items == null) return;
        if (RowCount <= 0) RowCount = 1;

        var referenceTop = transform.InverseTransformPoint(LayoutReference.transform.TransformPoint(Vector3.up * 0.5f)).y - (ItemSize / 2) - (JitterScale.y * ItemSize);
        var referenceBottom = transform.InverseTransformPoint(LayoutReference.transform.TransformPoint(Vector3.up * -0.5f)).y + (ItemSize / 2) + (JitterScale.y * ItemSize);

        var prevState = Random.state;
        Random.InitState(123);

        Vector3 jitter;

        int rowIndex = 0;
        float y;
        for (int i=0;i<items.Length;i++)
        {
            var item = items[i];

            rowIndex = (rowIndex + Random.Range(1, RowCount)) % RowCount;
            y = Mathf.Lerp(referenceBottom, referenceTop, (float)rowIndex / (float)(RowCount - 1));

            jitter = Random.insideUnitSphere;
            jitter.Scale(JitterScale * ItemSize);
            item.LocalPosition = new Vector3(i * ItemSize, y, 0) + jitter;
            
            item.Instance.transform.localPosition = IsPolar ? FromPolar(item.LocalPosition) : item.LocalPosition;
            item.Instance.transform.localScale = ItemSize * Vector3.one;
        }

        Random.state = prevState;
    }

    private Vector3 FromPolar(Vector3 input)
    {
        float r = input.y;
        float lat = input.x;
        float lon = input.z;

        return new Vector3(
            r * Mathf.Sin(lat) * Mathf.Cos(lon),
            r * Mathf.Cos(lat),
            r * Mathf.Sin(lat) * Mathf.Sin(lon)
        );
    }

    private void checkPhysicsMaterials(GameObject item)
    {
        var colliders = item.GetComponentsInChildren<Collider>();
        foreach(var collider in colliders)
        {
            if (collider.sharedMaterial == null) collider.sharedMaterial = DefaultMaterial;
        }
    }
}
