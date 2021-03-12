using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using TMPro;
using UnityEngine.UI;

public class Launcher : MonoBehaviourPunCallbacks
{
    [SerializeField]
    GameObject lobbyBtn;

    [SerializeField]
    TextMeshProUGUI errorText;

    bool leftRoom = false;

    public static Launcher Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {   // Move code?
        Debug.Log("Connecting to Master...");
        PhotonNetwork.ConnectUsingSettings();
    }
    private void Update()
    {
        if (leftRoom && PhotonNetwork.IsConnectedAndReady)
        {
            leftRoom = false;
            JoinLobby();
        }
    }
    public void JoinLobby()
    {
        Debug.Log("Joining Lobby...");
        PhotonNetwork.JoinLobby();
        MenuManager.Instance.OpenMenu("Loading");
    }

    public void LeaveLobby()
    {
        Debug.Log("Leaving Lobby...");
        PhotonNetwork.LeaveLobby();
        MenuManager.Instance.OpenMenu("Loading");
    }

    public void CreateRoom(string roomName, RoomOptions options)
    {
        Debug.Log("Creating Room...");
        PhotonNetwork.CreateRoom(roomName, options);
        MenuManager.Instance.OpenMenu("Loading");
    }

    public void JoinRoom(RoomInfo info)
    {
        Debug.Log("Joining Room...");
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.Instance.OpenMenu("Loading");
    }

    public void LeaveRoom()
    {
        Debug.Log("Leaving Room...");
        PhotonNetwork.LeaveRoom();
        MenuManager.Instance.OpenMenu("Loading");
    }

    public void StartGame(string mapName)
    {
        PhotonNetwork.LoadLevel(mapName);
    }

    #region Photon Callbacks
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        PhotonNetwork.AutomaticallySyncScene = true;
        lobbyBtn.GetComponent<Button>().interactable = true;
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
        MenuManager.Instance.OpenMenu("Game Lobby Menu");
        PhotonNetwork.NickName = SettingsManager.Instance.Nickname;
    }

    public override void OnLeftLobby()
    {
        Debug.Log("Left Lobby");
        MenuManager.Instance.OpenMenu("Title Menu");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "Room Creation Failed: " + message;
        MenuManager.Instance.OpenMenu("Error Menu");
    }

    public override void OnJoinedRoom() // Joined/Create room
    {
        Debug.Log("Joined Room");
        MenuManager.Instance.OpenMenu("Game Room Menu");
        RoomManager.Instance.JoinedRoom();
    }

    public override void OnLeftRoom()
    {
        Debug.Log("Left Room");
        leftRoom = true;
        MenuManager.Instance.OpenMenu("Loading");
        ChatManager.Instance.DestroyAllChatMessages();
    }
    
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        RoomManager.Instance.UpdateRoomList(roomList);
    }
    #endregion
}
