using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public static class WebAPI
{
    private const string ContentBaseUrl = "https://thinkin-api.glitch.me/v1/";

    public static async Task<RegisterDeviceResultDto> RegisterDevice(string Uid)
    {
        return await postRequest<RegisterDeviceResultDto>(ContentBaseUrl + "device/register", new RegisterDeviceRequestDto() { Uid = Uid });
    }

    public static async Task<CollectionContentItemDto[]> GetCollectionContents(string Url)
    {
        return await getRequest<CollectionContentItemDto[]>(Url, new ContentItemDtoConverter());
    }

    public static async Task<TResult> postRequest<TResult>(string Url, object body)
    {
        using (var request = new UnityWebRequest(Url, "POST"))
        {
            var json = body.ToJSON();
            var bytes = System.Text.Encoding.UTF8.GetBytes(json);
            request.SetRequestHeader("Content-Type", "application/json");
            if(UserInfo.CurrentUser != null) request.SetRequestHeader("auth", UserInfo.CurrentUser.AuthToken);
            request.uploadHandler = new UploadHandlerRaw(bytes);
            request.downloadHandler = new DownloadHandlerBuffer();

            await request.SendWebRequest().GetTask();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Request error: " + request.error);
                return default(TResult);
            }
            else
            {
                return JsonConvert.DeserializeObject<TResult>(request.downloadHandler.text);
            }
        }
    }

    private static async Task<T> getRequest<T>(string Url, JsonConverter Converter = null)
    {
        using (var request = new UnityWebRequest(Url, "GET"))
        {
            request.SetRequestHeader("Content-Type", "application/json");
            if (UserInfo.CurrentUser != null) request.SetRequestHeader("auth", UserInfo.CurrentUser.AuthToken);
            request.downloadHandler = new DownloadHandlerBuffer();

            await request.SendWebRequest().GetTask();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Request error: " + request.error);
                return default(T);
            }
            else
            {
                return JsonConvert.DeserializeObject<T>(request.downloadHandler.text, Converter);
            }
        }
    }
}
