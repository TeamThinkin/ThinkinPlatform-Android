using AngleSharp.Dom;
using Autohand;
using Normal.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class DestinationPresenter : MonoBehaviour
{
    public static event Action<string> UrlChanged;
    public static event Action OnDestinationLoaded;
    public static event Action OnDestinationUnloaded;

    [SerializeField] private Transform _contentContainer;
    public Transform ContentItemContainer => _contentContainer;

    [SerializeField] private TransitionController _transitionController;

    public static DestinationPresenter Instance { get; private set; }
    public static int? CurrentDestinationId { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public async Task DisplayUrl(string Url)
    {
        //NOTE: Assumes Urls are fully formed and not relative links

        var newRoomId = Url.GetHashCode();
        if (newRoomId == CurrentDestinationId) return;

        UrlChanged?.Invoke(Url);

        CurrentDestinationId = newRoomId;

        var documentTask = DocumentManager.FetchDocument(Url);

        await _transitionController.HideScene();

        stashPlayer();

        OnDestinationUnloaded?.Invoke();
        _contentContainer.ClearChildren();

        var document = await documentTask;
        await loadDocument(document);

        OnDestinationLoaded?.Invoke();
        _transitionController.RevealScene();

        releaseStashedPlayer();
    }

    private void stashPlayer()
    {
        var player = AutoHandPlayer.Instance;
        if(player == null) return;

        player.body.isKinematic = true;
        player.body.velocity = Vector3.zero;
        player.body.angularVelocity = Vector3.zero;
        player.SetPosition(Vector3.one * -10000);
    }

    private void releaseStashedPlayer()
    {
        var player = AutoHandPlayer.Instance;
        if (player == null) return;

        player.body.isKinematic = false;
    }

    private async Task loadDocument(IDocument Document)
    {
        var rootPresenter = ElementPresenterFactory.Instantiate(typeof(RootPresenter), Document.DocumentElement, null);
        rootPresenter.transform.SetParent(_contentContainer);
        rootPresenter.gameObject.name = "Root";

        traverseDOMforPresenters(Document.DocumentElement, rootPresenter);
        await Task.WhenAll(rootPresenter.All().Select(i => i.Initialize()));
    }

    private static void traverseDOMforPresenters(IElement dataElement, IElementPresenter parentPresenter, int depth = 0)
    {
        IElementPresenter currentPresenter = null;

        if (ElementPresenterFactory.HasTag(dataElement.TagName))
            currentPresenter = ElementPresenterFactory.Instantiate(dataElement, parentPresenter);

        foreach (var child in dataElement.Children)
        {
            traverseDOMforPresenters(child, currentPresenter ?? parentPresenter, depth + 1);
        }
    }
}
