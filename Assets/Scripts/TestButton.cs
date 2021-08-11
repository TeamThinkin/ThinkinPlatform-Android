using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.InputSystem;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.SceneManagement;

public class TestButton : MonoBehaviour
{
    private XRIDefaultInputActions XRInputActions;

    public static event Action OnEnvironmentLoaded;

    public string SceneUrl1 = "http://localhost:9000/{PLATFORM}/catalog_ThinkinContent.json#Assets/Scenes/Classroom.unity";
    public string SceneUrl2 = "http://localhost:9000/{PLATFORM}/catalog_ThinkinContent.json#Assets/Scenes/Classroom.unity";

    private AsyncOperationHandle currentSceneHandle;
    private IResourceLocator currentResourceLocator;
    private string currentScene;
    private string currentCatalogUrl;

    private void Awake()
    {
        XRInputActions = new XRIDefaultInputActions();
        XRInputActions.UIControls.TestButton1.performed += TestButton1_performed;
        XRInputActions.UIControls.TestButton1.Enable();

        XRInputActions.UIControls.TestButton2.performed += TestButton2_performed;
        XRInputActions.UIControls.TestButton2.Enable();

        Caching.ClearCache();
    }

    private void TestButton1_performed(InputAction.CallbackContext obj)
    {
        Debug.Log("Test button fired");
        //LoadScene("https://storage.googleapis.com/matriculate-assets/{PLATFORM}/catalog_ThinkinContent.json#Assets/Scenes/Classroom.unity");
        LoadScene(SceneUrl1);
    }

    private void TestButton2_performed(InputAction.CallbackContext obj)
    {
        Debug.Log("Test button fired");
        //LoadScene("https://storage.googleapis.com/matriculate-assets/{PLATFORM}/catalog_ThinkinContent.json#Assets/Scenes/Classroom.unity");
        LoadScene(SceneUrl2);
    }

    public void OnSelect()
    {
        transform.position += Vector3.up;
        LoadScene("https://storage.googleapis.com/matriculate-assets/{PLATFORM}/catalog_ThinkinContent.json#Assets/Scenes/Classroom.unity");
    }
    public void LoadScene(string sceneUrl)
    {
        StartCoroutine(doLoadScene(sceneUrl));
    }

    public IEnumerator doLoadScene(string sceneUrl) //Note: I had problems using async operations on the original Quest - mbell, 11/28/20 
    {
        if (currentScene != sceneUrl)
        {
            UnloadScene();

            var address = new AddressableUrl(sceneUrl);

            if (currentCatalogUrl != address.CatalogUrl)
            {
                var loadCatalogHandle = Addressables.LoadContentCatalogAsync(address.CatalogUrl);
                yield return loadCatalogHandle;
                currentResourceLocator = loadCatalogHandle.Result;
                currentCatalogUrl = address.CatalogUrl;
            }

            if (currentResourceLocator.Locate(address.AssetPath, typeof(UnityEngine.ResourceManagement.ResourceProviders.SceneInstance), out IList<IResourceLocation> locations))
            {
                //var sceneLocation = new LocalizingResourceLocation(locations[0], sceneUrl);
                var sceneLocation = locations[0];
                currentSceneHandle = Addressables.LoadSceneAsync(sceneLocation, LoadSceneMode.Additive);
                yield return currentSceneHandle;
                currentScene = sceneUrl;
                OnEnvironmentLoaded?.Invoke();
            }
            else
            {
                Debug.LogError("No locations found for scene: " + address.AssetPath);
            }
        }
    }

    public void UnloadScene()
    {
        if (currentScene != null) Addressables.UnloadSceneAsync(currentSceneHandle);
        currentScene = null;
    }
}