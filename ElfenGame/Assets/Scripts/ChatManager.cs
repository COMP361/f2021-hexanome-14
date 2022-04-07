using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using TMPro;
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

    private static ChatManager instance = null;

    private bool chatVisible
    {
        get
        {
            return chatPanel.activeSelf;
        }
    }

    private string chatContent = "";

    private string groupName = "";

    private string lastGroupName = "";

    private int newMessages = 0;

    [SerializeField] private GameObject chatPanel;
    [SerializeField] InputField toInputField;
    [SerializeField] InputField msgInputField;
    [SerializeField] Text chatContentText;
    [SerializeField] private GameObject messageIndicator;
    [SerializeField] private TextMeshProUGUI txt;


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

    public void JoinGroup(string groupName)
    {
        chatClient.Subscribe(groupName);
        this.groupName = groupName;
    }

    public void LeaveGroup()
    {
        if (groupName != "")
        {
            lastGroupName = groupName;
            chatClient.Unsubscribe(new string[] { groupName });
            groupName = "";
        }
    }

    public void OnDisconnected()
    {
        Debug.Log("Disconnected From Chat");
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        if (channelName == groupName)
        {
            channelName = "Game";
        }
        for (int i = 0; i < senders.Length; ++i)
        {
            chatContent += $"({channelName}) {senders[i]}: {messages[i]}\n";
            newMessages++;
        }
        updateContent();
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        chatContent += $"({channelName}) {sender}: {message}\n";
        newMessages++;
        updateContent();
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        throw new System.NotImplementedException();
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        foreach (string channel in channels)
        {
            if (channel == groupName)
            {
                chatContent += $"You have joined Game chat.\n";
            }
            else
            {
                chatContent += $"You have joined {channel} chat.\n";
            }
        }
        newMessages++;
        updateContent();
    }

    public void OnUnsubscribed(string[] channels)
    {
        foreach (string channel in channels)
        {
            if (channel == lastGroupName)
            {
                chatContent += $"You have left Game chat.\n";
            }
            else
            {
                chatContent += $"You have left {channel} chat.\n";
            }
        }
        newMessages++;
        updateContent();
    }

    public void OnUserSubscribed(string channel, string user)
    {
        Debug.Log($"{user} joined {channel}.");
        if (channel == groupName)
        {
            chatContent += $"{user} has joined Game chat.\n";
        }
        else
        {
            chatContent += $"{user} joined {channel} chat.\n";
        }
        newMessages++;
        updateContent();
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        Debug.Log($"{user} left {channel}.");
        if (channel == groupName)
        {
            chatContent += $"{user} has left Game chat.\n";
        }
        else
        {
            chatContent += $"{user} has left {channel} chat.\n";
        }
        newMessages++;
        updateContent();
    }

    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(this);
        if (instance == null)
        {
            instance = this;
            chatClient = new ChatClient(this);
            chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, PhotonNetwork.AppVersion, new AuthenticationValues(GameConstants.username));
        }
        else
        {
            Destroy(gameObject);
        }
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

        if ((recipient == "" && groupName == "") || recipient == "General")
        {
            chatClient.PublishMessage("General", msg);
        }
        else if (recipient == "" || recipient == "Game")
        {
            if (groupName != "")
            {
                chatClient.PublishMessage(groupName, msg);
            }
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
        newMessages = 0;
        UpdateNumMessages();

        chatPanel.SetActive(true);
    }

    public void SetChatInvisible()
    {
        chatPanel.SetActive(false);
    }

    public void ToggleChat()
    {
        if (chatVisible)
        {
            SetChatInvisible();
        }
        else
        {
            setChatVisible();
        }
    }

    private void updateContent()
    {
        chatContentText.text = chatContent;
        UpdateNumMessages();
    }
    public void UpdateNumMessages()
    {
        if (newMessages > 0 && !chatVisible)
        {
            messageIndicator.SetActive(true);
            if (newMessages > 9)
            {
                txt.text = "9+";
            }
            else
            {
                txt.text = newMessages.ToString();
            }
        }
        else
        {
            messageIndicator.SetActive(false);
        }
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
}