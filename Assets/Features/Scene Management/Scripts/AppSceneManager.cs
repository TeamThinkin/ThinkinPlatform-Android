using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
//using UnityEngine.AddressableAssets;
//using UnityEngine.AddressableAssets.ResourceLocators;
//using UnityEngine.ResourceManagement.AsyncOperations;
//using UnityEngine.ResourceManagement.ResourceLocations;
//using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class AppSceneManager : MonoBehaviour
{
    [SerializeField] private bool ClearCacheOnAwake;
    [SerializeField] private Transform RoomItemContainer;

    public static event Action OnEnvironmentLoaded;
    public static event Action OnEnvironmentUnloaded;

    public static AppSceneManager Instance { get; private set; }

    //private static AsyncOperationHandle<SceneInstance> currentSceneHandle;
    //private static IResourceLocator currentResourceLocator;
    private static AssetBundle currentAssetBundle;
    private static bool currentSceneIsRemote;
    private static string currentScene;
    private static string currentSceneUrl;
    private static string currentCatalogUrl;
    private static bool isLoadingLocalScene;
    private static bool isLoading;


    private void Awake()
    {
        Instance = this;
        if(ClearCacheOnAwake) Caching.ClearCache();
    }

    public async Task LoadRemoteScene(string SceneUrl)
    {
        if (isLoading) return;
        if (currentScene == SceneUrl) return;

        isLoading = true;
        await UnloadScene();
        currentScene = SceneUrl;
        await loadRemoteScene(SceneUrl);
        isLoading = false;
    }

    public async Task LoadLocalScene(string SceneName)
    {
        if (isLoading) return;
        if (currentScene == SceneName) return;

        isLoading = true;
        for (int i=0;i<SceneManager.sceneCount;i++)
        {
            if (SceneManager.GetSceneAt(i).name == SceneName) return;
        }

        await UnloadScene();

        isLoadingLocalScene = true;
        StartCoroutine(loadLocalSceneRoutine(SceneName));

        await Task.Run(() =>
        {
            while (isLoadingLocalScene)
            {
                Thread.Sleep(10);
            }
        });

        currentScene = SceneName;
        currentSceneUrl = null;
        currentSceneIsRemote = false;
        OnEnvironmentLoaded?.Invoke();

        isLoading = false;
    }

    private IEnumerator loadLocalSceneRoutine(string SceneName)
    {
        yield return null;

        var task = SceneManager.LoadSceneAsync(SceneName, LoadSceneMode.Single);
        while (!task.isDone)
        {
            //Debug.Log("Load Local Scene Progress: " + task.progress);
            yield return null;
        }
        isLoadingLocalScene = false;
    }

    private async Task loadRemoteScene(string SceneUrl)
    {
        if (currentSceneUrl == SceneUrl)
        {
            Debug.Log("Skipping loadRemoteScene because the currentScaneUrl and SceneUrl match");
            return;
        }

        Debug.Log("Loading remote scene.... " + SceneUrl);

        var address = new AddressableUrl(SceneUrl);

        if(currentCatalogUrl != address.CatalogUrl)
        {
            currentAssetBundle?.Unload(true);
            var request = UnityEngine.Networking.UnityWebRequestAssetBundle.GetAssetBundle(address.CatalogUrl, 0);
            await request.SendWebRequest().GetTask();
            currentAssetBundle = UnityEngine.Networking.DownloadHandlerAssetBundle.GetContent(request);
        }

        string scenePath = address.AssetPath;
        if (string.IsNullOrEmpty(scenePath)) scenePath = currentAssetBundle.GetAllScenePaths()[0];

        await SceneManager.LoadSceneAsync(scenePath, LoadSceneMode.Single).GetTask();
        //await SceneManager.LoadSceneAsync(scenePath, LoadSceneMode.Additive).GetTask();

        currentScene = scenePath; // //SceneUrl;
        currentSceneUrl = SceneUrl;
        currentSceneIsRemote = true;
        OnEnvironmentLoaded?.Invoke();
    }

    //private async Task loadRemoteScene(string SceneUrl)
    //{
    //    //var address = new AddressableUrl(SceneUrl);
    //    //if (currentCatalogUrl != address.CatalogUrl)
    //    //{
    //    //    currentResourceLocator = await Addressables.LoadContentCatalogAsync(address.CatalogUrl).GetTask();
    //    //    currentCatalogUrl = address.CatalogUrl;
    //    //}

    //    //if (currentResourceLocator.Locate(address.AssetPath, typeof(UnityEngine.ResourceManagement.ResourceProviders.SceneInstance), out IList<IResourceLocation> locations))
    //    //{
    //    //    var sceneLocation = new LocalizingResourceLocation(locations[0], SceneUrl);
    //    //    currentSceneHandle = Addressables.LoadSceneAsync(sceneLocation, LoadSceneMode.Additive);
    //    //    await currentSceneHandle.GetTask();

    //    //    currentScene = SceneUrl;
    //    //    currentSceneIsRemote = true;
    //    //    OnEnvironmentLoaded?.Invoke();
    //    //}
    //    //else
    //    //{
    //    //    Debug.LogError("No locations found for scene: " + address.AssetPath);
    //    //}
    //}

    public async Task UnloadScene()
    {
        OnEnvironmentUnloaded?.Invoke();
        return; //Since switching to single scene instead of additive. This method should not be needed

        if (currentScene == null) return;

        //if (currentSceneIsRemote)
        //{
        //    await Addressables.UnloadSceneAsync(currentSceneHandle).GetTask();
        //}
        //else
        //{
        //    await SceneManager.UnloadSceneAsync(currentScene).GetTask();
        //}
        Debug.Log("Unloading scene: " + currentScene);
        await SceneManager.UnloadSceneAsync(currentScene).GetTask();

        currentScene = null;
        OnEnvironmentUnloaded?.Invoke();
    }
}
