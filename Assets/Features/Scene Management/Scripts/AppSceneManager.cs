using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class AppSceneManager : MonoBehaviour
{
    [SerializeField] private bool ClearCacheOnAwake;
    [SerializeField] private Transform RoomItemContainer;

    public static event Action OnEnvironmentLoaded;
    public static event Action OnEnvironmentUnloaded;

    public static AppSceneManager Instance { get; private set; }

    private static AsyncOperationHandle<SceneInstance> currentSceneHandle;
    private static IResourceLocator currentResourceLocator;
    private static bool currentSceneIsRemote;
    private static string currentScene;
    private static string currentCatalogUrl;

    private void Awake()
    {
        Instance = this;
        if(ClearCacheOnAwake) Caching.ClearCache();
    }

    public async Task LoadRemoteScene(string SceneUrl)
    {
        if (currentScene == SceneUrl) return;

        await UnloadScene();
        currentScene = SceneUrl;
        await loadRemoteScene(SceneUrl);
    }

    public async Task LoadLocalScene(string SceneName)
    {
        if (currentScene == SceneName) return;

        for (int i=0;i<SceneManager.sceneCount;i++)
        {
            if (SceneManager.GetSceneAt(i).name == SceneName) return;
        }

        await UnloadScene();

        SceneManager.LoadScene(SceneName, LoadSceneMode.Additive);

        currentScene = SceneName;
        currentSceneIsRemote = false;
        OnEnvironmentLoaded?.Invoke();
    }

    private async Task loadRemoteScene(string SceneUrl)
    {
        var address = new AddressableUrl(SceneUrl);
        if (currentCatalogUrl != address.CatalogUrl)
        {
            currentResourceLocator = await Addressables.LoadContentCatalogAsync(address.CatalogUrl).GetTask();
            currentCatalogUrl = address.CatalogUrl;
        }

        if (currentResourceLocator.Locate(address.AssetPath, typeof(UnityEngine.ResourceManagement.ResourceProviders.SceneInstance), out IList<IResourceLocation> locations))
        {
            var sceneLocation = new LocalizingResourceLocation(locations[0], SceneUrl);
            currentSceneHandle = Addressables.LoadSceneAsync(sceneLocation, LoadSceneMode.Additive);
            await currentSceneHandle.GetTask();

            currentScene = SceneUrl;
            currentSceneIsRemote = true;
            OnEnvironmentLoaded?.Invoke();
        }
        else
        {
            Debug.LogError("No locations found for scene: " + address.AssetPath);
        }
    }

    //private IEnumerator doLoadRemoteScene(string SceneUrl) //Note: I had problems using async operations on the original Quest - mbell, 11/28/20 
    //{
    //    var address = new AddressableUrl(SceneUrl);
    //    if (currentCatalogUrl != address.CatalogUrl)
    //    {
    //        var loadCatalogHandle = Addressables.LoadContentCatalogAsync(address.CatalogUrl);
    //        yield return loadCatalogHandle;
    //        currentResourceLocator = loadCatalogHandle.Result;
    //        currentCatalogUrl = address.CatalogUrl;
    //    }

    //    if (currentResourceLocator.Locate(address.AssetPath, typeof(UnityEngine.ResourceManagement.ResourceProviders.SceneInstance), out IList<IResourceLocation> locations))
    //    {
    //        var sceneLocation = new LocalizingResourceLocation(locations[0], SceneUrl);
    //        //var sceneLocation = locations[0];
    //        currentSceneHandle = Addressables.LoadSceneAsync(sceneLocation, LoadSceneMode.Additive);
    //        yield return currentSceneHandle;
    //        currentScene = SceneUrl;
    //        currentSceneIsRemote = true;
    //        OnEnvironmentLoaded?.Invoke();

    //        //TransitionController.RevealScene();
    //    }
    //    else
    //    {
    //        Debug.LogError("No locations found for scene: " + address.AssetPath);
    //    }
    //}

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
        OnEnvironmentUnloaded?.Invoke();
    }
}
