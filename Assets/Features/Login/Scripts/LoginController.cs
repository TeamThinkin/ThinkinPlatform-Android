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

    public class CodewordDto
    {
        [JsonProperty("uid")]
        public string UID { get; set; }

        [JsonProperty("codeword")]
        public string Codeword { get; set; }
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

        socket = SocketIOController.Instance;

        socket.On("connect", onSocketConnected);
        WebSocketListener.OnSetUser += WebSocketListener_OnSetUser;

        if (socket.IsConnected) sendAccountLinkingCode();
    }

    private void OnDestroy()
    {
        socket.Emit("clearAccountLinkingCode", codeword);
        socket.Off("connect", onSocketConnected);
        WebSocketListener.OnSetUser -= WebSocketListener_OnSetUser;
    }

    private void onSocketConnected(SocketIOEvent e)
    {
        sendAccountLinkingCode();
    }

    private void sendAccountLinkingCode()
    {
        Debug.Log("Sending account linking code");
        socket.Emit("accountLinkingCode", JsonConvert.SerializeObject(new CodewordDto() { UID = AppController.UID, Codeword = codeword }));
    }

    private void WebSocketListener_OnSetUser(UserDto newUser)
    {     
        PrelinkElements.SetActive(false);
        PostlinkElements.SetActive(true);

        //UserInfo.CurrentUser = new UserInfo() { DisplayName = newUser.DisplayName, Id = newUser.Id, AvatarUrl = newUser.AvatarUrl };
    }

    public void OnContinueButtonPressed()
    {
        AppSceneManager.Instance.LoadLocalScene("Home Room");
    }
}
