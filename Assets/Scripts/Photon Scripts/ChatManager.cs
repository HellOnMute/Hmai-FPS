using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ChatMessage
{
    public string Sender { get; set; }
    public string Message { get; set; } = "";
    public float Timer = 0f;
}

public class ChatManager : MonoBehaviourPun
{
    [SerializeField]
    bool inMenu = false;
    [SerializeField]
    int maxMessages = 10;
    [SerializeField]
    GameObject chatMessagePrefab;

    public bool CanChat { get; set; } = false;

    bool isChatting = false;
    List<GameObject> messageGameObjects = new List<GameObject>();

    [SerializeField]
    TMP_InputField chatInput;
    [SerializeField]
    GameObject chatListContainer;

    public static ChatManager Instance;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    private void Start()
    {
        //chatInput = GameObject.FindGameObjectWithTag("ChatListInput").GetComponent<TMP_InputField>();
        //chatListContainer = GameObject.FindGameObjectWithTag("ChatListContainer");
    }

    void Update()
    {
        if (((Input.GetKeyUp(KeyCode.T) || inMenu) && !isChatting))
        {
            isChatting = true;
            chatInput.ActivateInputField();
            EnableMessages();
        }
        if (isChatting && Input.GetKeyDown(KeyCode.Return))
        {
            MakeChatMessage();

            if (inMenu)
                chatInput.ActivateInputField();
        }
    }

    void EnableMessages()
    {
        foreach (var msg in messageGameObjects)
        {
            msg.GetComponent<ChatMessageScript>().IsChatting = true;
        }
    }

    void DisableMessages()
    {
        foreach (var msg in messageGameObjects)
        {
            msg.GetComponent<ChatMessageScript>().IsChatting = false;
        }
    }

    public void MakeChatMessage()
    {
        isChatting = false;
        if (!inMenu)
            DisableMessages();

        if (string.IsNullOrEmpty(chatInput.text))
            return;

        photonView.RPC("SendChat", RpcTarget.All, PhotonNetwork.LocalPlayer, chatInput.text);
        chatInput.text = "";
    }

    public void DestroyAllChatMessages()
    {
        foreach (var msg in messageGameObjects)
        {
            Destroy(msg.gameObject);
        }

        messageGameObjects.Clear();
    }

    [PunRPC]
    void SendChat(Player sender, string message)
    {
        ChatMessage m = new ChatMessage();
        m.Sender = sender.NickName;
        m.Message = $"[{DateTime.Now.ToString("HH:mm:ss")}] {m.Sender}: {message}";
        m.Timer = 10.0f;
        
        var msgGO = Instantiate(chatMessagePrefab, chatListContainer.transform);
        msgGO.GetComponent<ChatMessageScript>().Message = m.Message;
        messageGameObjects.Add(msgGO);

        if (messageGameObjects.Count > maxMessages)
        {
            Destroy(messageGameObjects[0].gameObject);
            messageGameObjects.RemoveAt(0);
        }
    }
}
