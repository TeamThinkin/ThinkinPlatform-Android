using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.SceneManagement;

public class AppSceneManager : MonoBehaviour
{
    public static event Action OnEnvironmentLoaded;
    public static event Action OnEnvironmentUnloaded;

    public static AppSceneManager Instance { get; private set; }

    [SerializeField] private TransitionController TransitionController;
    [SerializeField] private bool ClearCacheOnAwake;
    [SerializeField] private Transform RoomItemContainer;

    private AsyncOperationHandle currentSceneHandle;
    private IResourceLocator currentResourceLocator;
    private bool currentSceneIsRemote;
    private string currentScene;
    private string currentCatalogUrl;
    private RoomInfo currentRoom;
    private Action OnceSceneIsHiddenAction;

    public RoomInfo CurrentRoom => currentRoom;
    

    private void Awake()
    {
        Instance = this;
        if(ClearCacheOnAwake) Caching.ClearCache();
    }

    private void Start()
    {
        TransitionController.OnSceneHidden += TransitionController_OnSceneHidden;
    }

    private void OnDestroy()
    {
        TransitionController.OnSceneHidden -= TransitionController_OnSceneHidden;
    }

    private void TransitionController_OnSceneHidden()
    {
        OnceSceneIsHiddenAction?.Invoke();
        OnceSceneIsHiddenAction = null;
    }

    public async void LoadUrl(string Url)
    {
        Debug.Log("Scene Manager loading url: " + Url);
        RoomItemContainer.ClearChildren();
        var dtos = await WebAPI.GetCollectionContents(Url);
        foreach(var dto in dtos)
        {
            var item = PresenterFactory.Instance.Instantiate(dto);
            if (item != null)
            {
                item.name = dto.Id;
                item.transform.SetParent(RoomItemContainer);
            }
            else Debug.Log("Skipping unrecognized item: " + dto.MimeType);
        }
    }

    public void LoadRoom(RoomInfo Room)
    {
        if (Room == currentRoom) return;

        TransitionController.HideScene();

        OnceSceneIsHiddenAction = new Action(async () =>
        {
            await UnloadScene();
            currentRoom = Room;
            StartCoroutine(doLoadRemoteScene(Room.EnvironmentUrl));
        });
    }

    public void LoadRemoteScene(string SceneUrl)
    {
        if (currentScene == SceneUrl) return;

        TransitionController.HideScene();

        OnceSceneIsHiddenAction = new Action(async () =>
        {
            await UnloadScene();
            currentScene = SceneUrl;
            StartCoroutine(doLoadRemoteScene(SceneUrl));
        });
    }

    public void LoadLocalScene(string SceneName)
    {
        if (currentScene == SceneName) return;

        for (int i=0;i<SceneManager.sceneCount;i++)
        {
            if (SceneManager.GetSceneAt(i).name == SceneName) return;
        }

        TransitionController.HideScene();

        OnceSceneIsHiddenAction = new Action(async () =>
        {
            await UnloadScene();

            SceneManager.LoadScene(SceneName, LoadSceneMode.Additive);

            currentScene = SceneName;
            currentSceneIsRemote = false;
            OnEnvironmentLoaded?.Invoke();

            TransitionController.RevealScene();
        });
    }

    private void loadRemoteScene(string SceneUrl)
    {
        if (currentScene == SceneUrl) return;
        
        TransitionController.HideScene();
            
        OnceSceneIsHiddenAction = new Action(async () =>
        {
            await UnloadScene();
            StartCoroutine(doLoadRemoteScene(SceneUrl));
        });
    }

    private IEnumerator doLoadRemoteScene(string SceneUrl) //Note: I had problems using async operations on the original Quest - mbell, 11/28/20 
    {
        var address = new AddressableUrl(SceneUrl);
        if (currentCatalogUrl != address.CatalogUrl)
        {
            var loadCatalogHandle = Addressables.LoadContentCatalogAsync(address.CatalogUrl);
            yield return loadCatalogHandle;
            currentResourceLocator = loadCatalogHandle.Result;
            currentCatalogUrl = address.CatalogUrl;
        }

        if (currentResourceLocator.Locate(address.AssetPath, typeof(UnityEngine.ResourceManagement.ResourceProviders.SceneInstance), out IList<IResourceLocation> locations))
        {
            var sceneLocation = new LocalizingResourceLocation(locations[0], SceneUrl);
            //var sceneLocation = locations[0];
            currentSceneHandle = Addressables.LoadSceneAsync(sceneLocation, LoadSceneMode.Additive);
            yield return currentSceneHandle;
            currentScene = SceneUrl;
            currentSceneIsRemote = true;
            OnEnvironmentLoaded?.Invoke();

            TransitionController.RevealScene();
        }
        else
        {
            Debug.LogError("No locations found for scene: " + address.AssetPath);
        }
    }

    public async Task UnloadScene()
    {
        if (currentScene == null) return;

        if(currentSceneIsRemote)
        {
            await Addressables.UnloadSceneAsync(currentSceneHandle).GetTask();
        }
        else
        {
            await SceneManager.UnloadSceneAsync(currentScene).GetTask();
        }

        currentScene = null;
        currentRoom = null;
        OnEnvironmentUnloaded?.Invoke();
    }
}
