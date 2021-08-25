using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public static class WebAPI
{
    private const string ContentBaseUrl = "https://thinkin-setup.glitch.me/api/v1/";
    private const string AuthToken = "3be5f7ac-b5c5-440f-ba9e-fd9b5577a942";

    public static async Task<UserDto> RegisterDevice(string Uid)
    {
        using (var request = new UnityWebRequest(ContentBaseUrl + "device/register", "POST"))
        {
            var json = "{\"uid\": \"" + Uid + "\" }";
            var bytes = System.Text.Encoding.UTF8.GetBytes(json);
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("auth", AuthToken);
            request.uploadHandler = new UploadHandlerRaw(bytes);
            request.downloadHandler = new DownloadHandlerBuffer();

            await request.SendWebRequest().GetTask();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Request error: " + request.error);
                return null;
            }
            else
            {
                return JsonConvert.DeserializeObject<UserDto>(request.downloadHandler.text);
            }
        }
    }
}
