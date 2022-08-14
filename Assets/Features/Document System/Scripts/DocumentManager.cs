using AngleSharp;
using AngleSharp.Dom;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public static class DocumentManager
{
    public static async Task<IDocument> FetchDocument(string Url)
    {
        var source = await getRequest(Url);
        var config = Configuration.Default;
        var context = BrowsingContext.New(config);
        var document = await context.OpenAsync(req => req.Content(source).Address(Url));
        
        return document;
    }

    private static async Task<string> getRequest(string Url)
    {
        Debug.Log("Get request: " + Url);
        using (var request = new UnityWebRequest(Url, "GET"))
        {
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("User-Agent", "Thinkin/" + Application.version);
            if (UserInfo.CurrentUser != null) request.SetRequestHeader("auth", UserInfo.CurrentUser.AuthToken);
            request.downloadHandler = new DownloadHandlerBuffer();

            await request.SendWebRequest().GetTask();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Request error: " + request.error);
                return null;
            }
            else
            {
                return request.downloadHandler.text;
            }
        }
    }
}

public static class IElementPresenterExtensions
{
    public static IEnumerable<IElementPresenter> All(this IElementPresenter Presenter)
    {
        yield return Presenter;
        foreach(var child in Presenter.DOMChildren)
        {
            foreach(var item in child.All())
            {
                yield return item;
            }
        }
    }
}
