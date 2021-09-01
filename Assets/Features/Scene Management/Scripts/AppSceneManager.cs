using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.SceneManagement;

public class AppSceneManager : MonoBehaviour
{
    public static event Action OnEnvironmentLoaded;

    public static AppSceneManager Instance { get; private set; }

    [SerializeField] private TransitionController TransitionController;
    [SerializeField] private bool ClearCacheOnAwake;

    private AsyncOperationHandle currentSceneHandle;
    private IResourceLocator currentResourceLocator;
    private bool currentSceneIsRemote;
    private string currentScene;
    private string currentCatalogUrl;

    private Action OnceSceneIsHiddenAction;

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

    public void LoadLocalScene(string SceneName)
    {
        if (currentScene == SceneName) return;

        for (int i=0;i<SceneManager.sceneCount;i++)
        {
            if (SceneManager.GetSceneAt(i).name == SceneName) return;
        }

        TransitionController.HideScene();

        OnceSceneIsHiddenAction = new Action(() =>
        {
            UnloadScene();

            SceneManager.LoadScene(SceneName, LoadSceneMode.Additive);

            currentScene = SceneName;
            currentSceneIsRemote = false;

            TransitionController.RevealScene();
        });
    }

    public void LoadRemoteScene(string SceneUrl)
    {
        TransitionController.HideScene();

        OnceSceneIsHiddenAction = new Action(() =>
        {
            StartCoroutine(doLoadRemoteScene(SceneUrl));
        });
    }

    private IEnumerator doLoadRemoteScene(string SceneUrl) //Note: I had problems using async operations on the original Quest - mbell, 11/28/20 
    {
        if (currentScene != SceneUrl)
        {
            UnloadScene();

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
    }

    public void UnloadScene()
    {
        if (currentScene == null) return;

        if(currentSceneIsRemote)
        {
            Addressables.UnloadSceneAsync(currentSceneHandle);
        }
        else
        {
            SceneManager.UnloadSceneAsync(currentScene);
        }

        currentScene = null;
    }
}
