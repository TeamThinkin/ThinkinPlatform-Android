using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestAssetBundle : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //Invoke("test", .1f);
        LoadScene();
    }

    //private void test()
    //{
    //    Debug.Log("Testing...");
    //    StartCoroutine(InstantiateObject());
    //    //StartCoroutine(test2());
    //}

    private async Task LoadScene()
    {
        //string url = "http://192.168.1.156:8000/Windows64/campus";
        string url = "https://storage.googleapis.com/matriculate-assets/Windows64/campus";
        var request = UnityEngine.Networking.UnityWebRequestAssetBundle.GetAssetBundle(url, 0);
        await request.SendWebRequest().GetTask();
        AssetBundle bundle = UnityEngine.Networking.DownloadHandlerAssetBundle.GetContent(request);

        var scenePaths = bundle.GetAllScenePaths();
        SceneManager.LoadSceneAsync(scenePaths[0], LoadSceneMode.Additive);
        
        Debug.Log("complete");
    }


    //IEnumerator InstantiateObject()
    //{
    //    //string url = "file:///" + Application.dataPath + "/AssetBundles/" + assetBundleName;
    //    string url = "http://192.168.1.156:8000/Windows64/campus";
    //    Debug.Log("A");
    //    var request
    //        = UnityEngine.Networking.UnityWebRequestAssetBundle.GetAssetBundle(url, 0);
    //    Debug.Log("B");
    //    yield return request.SendWebRequest();
    //    //yield return request.Send();
    //    Debug.Log("request sent");
    //    AssetBundle bundle = UnityEngine.Networking.DownloadHandlerAssetBundle.GetContent(request);

    //    var scenePaths = bundle.GetAllScenePaths();
    //    //SceneManager.LoadScene(scenePaths[0], LoadSceneMode.Additive);
    //    Debug.Log("Loading...");
    //    SceneManager.LoadSceneAsync(scenePaths[0], LoadSceneMode.Additive);
    //    foreach(var path in scenePaths)
    //    {
    //        Debug.Log(path);
    //    }

    //    bundle.Unload(true);



    //    //GameObject cube = bundle.LoadAsset<GameObject>("Test Item");
    //    //Instantiate(cube);

    //    //GameObject cube = bundle.LoadAsset<GameObject>("Cube");
    //    //GameObject sprite = bundle.LoadAsset<GameObject>("Sprite");
    //    //Instantiate(cube);
    //    //Instantiate(sprite);

    //    Debug.Log("complete");
    //}
}
