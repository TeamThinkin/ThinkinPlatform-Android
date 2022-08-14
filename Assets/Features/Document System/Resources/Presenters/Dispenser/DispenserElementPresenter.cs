using AngleSharp.Dom;
using AngleSharp.XPath;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;

[ElementPresenter("dispenser", "Presenters/Dispenser/Dispenser", false)]
public class DispenserElementPresenter : ElementPresenterBase
{
    private string src;

    public override void ParseDataElement(IElement ElementData)
    {
        src = ElementData.Attributes["src"].Value;
    }

    public override async Task Initialize()
    {
        Debug.Log("Doing dispenser shit with " + src);

        var address = new AssetUrl(src);
        var request = UnityEngine.Networking.UnityWebRequestAssetBundle.GetAssetBundle(address.CatalogUrl, 0);
        await request.SendWebRequest().GetTask();
        var bundle = UnityEngine.Networking.DownloadHandlerAssetBundle.GetContent(request);
        var names = bundle.GetAllAssetNames();

        transform.position = Vector3.up * 2;
        foreach(var name in names)
        {
            Debug.Log(name);
            var prefab = bundle.LoadAsset<GameObject>(name);
            var item = Instantiate(prefab, this.transform);
            item.transform.localPosition = UnityEngine.Random.insideUnitSphere;
            item.transform.localScale = 0.1f * Vector3.one;
        }        
    }

    //private static async Task loadRemoteScene(string SceneUrl)
    //{
    //    if (currentSceneUrl == SceneUrl) return;

    //    var address = new AssetUrl(SceneUrl);

    //    if (currentCatalogUrl != address.CatalogUrl)
    //    {
    //        currentAssetBundle?.Unload(true);
    //        var request = UnityEngine.Networking.UnityWebRequestAssetBundle.GetAssetBundle(address.CatalogUrl, 0);
    //        await request.SendWebRequest().GetTask();
    //        currentAssetBundle = UnityEngine.Networking.DownloadHandlerAssetBundle.GetContent(request);
    //    }

    //    string scenePath = address.AssetPath;
    //    if (string.IsNullOrEmpty(scenePath)) scenePath = currentAssetBundle.GetAllScenePaths()[0];

    //    await SceneManager.LoadSceneAsync(scenePath, LoadSceneMode.Single).GetTask();

    //    currentScene = scenePath;
    //    currentSceneUrl = SceneUrl;
    //    currentSceneIsRemote = true;
    //}
}
