using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySocketIO.Events;

public class LoginController : MonoBehaviour
{
    public class LinkAccountDto
    {
        [JsonProperty("uid")]
        public string UID { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }
    }

    [SerializeField] private GameObject PrelinkElements;
    [SerializeField] private GameObject PostlinkElements;
    [SerializeField] private TMPro.TMP_Text CodewordLabel;

    private SocketIOController socket;
    private string codeword;

    void Start()
    {
        codeword = CodewordGenerator.GetSimpleCode();
        CodewordLabel.text = codeword;

        PrelinkElements.SetActive(true);
        PostlinkElements.SetActive(false);

        socket = GameObject.FindObjectOfType<SocketIOController>();

        socket.On("connect", onSocketConnected);
        socket.On("linkAccount", onLinkAccount);

        if (socket.IsConnected) sendAccountLinkingCode();
    }

    private void OnDestroy()
    {
        socket.Emit("clearAccountLinkingCode", codeword);
        socket.Off("connect", onSocketConnected);
        socket.Off("linkAccount", onLinkAccount);
    }

    private void onSocketConnected(SocketIOEvent e)
    {
        sendAccountLinkingCode();
    }

    private void sendAccountLinkingCode()
    {
        Debug.Log("Sending account linking code");
        socket.Emit("accountLinkingCode", codeword);
    }

    private void onLinkAccount(SocketIOEvent e)
    {        
        PrelinkElements.SetActive(false);
        PostlinkElements.SetActive(true);

        var dto = JsonConvert.DeserializeObject<LinkAccountDto>(e.data);
        UserInfo.CurrentUser = new UserInfo(dto.DisplayName, dto.UID);
    }

    public void OnContinueButtonPressed()
    {
        AppSceneManager.Instance.LoadLocalScene("Home Room");
    }
}
