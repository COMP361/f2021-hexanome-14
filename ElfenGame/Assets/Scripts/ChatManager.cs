using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour, IChatClientListener
{
    #region singleton 

    private static ChatManager _instance;

    public static ChatManager manager
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ChatManager>();
            }
            return _instance;
        }
    }

    #endregion   

    ChatClient chatClient;

    private bool chatVisible = false;

    private string chatContent = "";

    private int newMessages = 0;

    [SerializeField] private Canvas canvas;
    [SerializeField] InputField toInputField;
    [SerializeField] InputField msgInputField;
    [SerializeField] Text chatContentText;
    

    public void DebugReturn(DebugLevel level, string message)
    {
        Debug.Log(message);
    }

    public void OnChatStateChange(ChatState state)
    {
        Debug.Log($"Chat Status Changed: {state}");
    }

    public void OnConnected()
    {
        Debug.Log("Connected To Chat");
        chatClient.Subscribe("General");
    }

    public void OnDisconnected()
    {
        Debug.Log("Disconnected From Chat");
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        for (int i = 0; i < senders.Length; ++i)
        {
            chatContent += $"({channelName}) {senders[i]}: {messages[i]}\n";
            newMessages++;
        }
        updateContent();
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        chatContent += $"(private) {sender}: {message}\n";
        newMessages++;
        updateContent();
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        throw new System.NotImplementedException();
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        Debug.Log($"Just Subscribed. Total {channels.Length} channels.");
    }

    public void OnUnsubscribed(string[] channels)
    {
        Debug.Log($"Just Unsubscribed. Total {channels.Length} channels.");
    }

    public void OnUserSubscribed(string channel, string user)
    {
        Debug.Log($"{user} joined {channel}.");
        chatContent += $"{user} joined {channel}.";
        updateContent();
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        Debug.Log($"{user} left {channel}.");
        chatContent += $"{user} left {channel}.";
        updateContent();
    }

    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        chatClient = new ChatClient(this);
        chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, PhotonNetwork.AppVersion, new AuthenticationValues(GameConstants.username));
    }

    // Update is called once per frame
    void Update()
    {
        chatClient.Service();
    }

    public void OnSendClicked()
    {
        string recipient = toInputField.text;
        string msg = msgInputField.text;

        if (recipient == "")
        {
            chatClient.PublishMessage("General", msg);
        }
        else
        {
            chatClient.SendPrivateMessage(recipient, msg);
        }

        msgInputField.text = "";
        if (!msgInputField.isFocused)
            msgInputField.Select();
    }

    public bool isActive()
    {
        return chatVisible;
    }

    public void setChatVisible()
    {
        chatVisible = true;

        canvas.gameObject.SetActive(chatVisible);
    }

    public void SetChatInvisible()
    {
        chatVisible = false;

        canvas.gameObject.SetActive(chatVisible);
    }

    private void updateContent()
    {
        chatContentText.text = chatContent;
    }

    public void OnTabPressed()
    {
        if (msgInputField.isFocused)
        {
            toInputField.Select();
            //toInputField
        }
        else
        {
            msgInputField.Select();
        }
    }

    public void OnEnterPressed()
    {
        OnSendClicked();
    }

    public int newMessage()
    {
        return newMessages;
    }

    public void newReset()
    {
        newMessages = 0;
    }
}
