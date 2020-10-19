using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;
using Photon.Realtime;
using System.Linq;
using TMPro;
using UnityEngine.UI;

public class RoomManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    Transform roomListContent, playerListContent;

    [SerializeField]
    GameObject roomListItemPrefab, playerListItemPrefab, startGameButton;

    [SerializeField]
    TextMeshProUGUI mapName, gameType, condition;

    [SerializeField]
    Image mapImage;

    [SerializeField]
    List<PlayableMap> playableMaps;

    public static RoomManager Instance;

    // Settings
    public string RoomName { get; private set; }
    public GameType GameType { get; private set; } = GameType.Deathmatch;
    public Conditions Conditions { get; private set; } = Conditions.Score;
    public TimeCondition Time { get; private set; } = TimeCondition.NotSelected;
    public ScoreCondition Score { get; private set; } = ScoreCondition.Fifteen;
    public PlayableMap SelectedMap { get; private set; }
    public List<PlayableMap> Maps => playableMaps;

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

    public PlayableMap GetMapByName(string name)
    {
        return playableMaps.First(x => x.name == name);
    }

    #region Room Settings
    public void SetRoomName(string name)
    {
        RoomName = name;
    }

    public void SetGameType(int type)
    {
        GameType = (GameType)type;
    }

    public void SetConditions(int conditions)
    {
        Conditions = (Conditions)conditions;
    }

    public void SetTime(int time)
    {
        Time = (TimeCondition)time;
    }

    public void SetScore(int score)
    {
        Score = (ScoreCondition)score;
    }

    public void SetMap(int map)
    {
        SelectedMap = playableMaps[map];
    }

    public void SetMap(string mapName)
    {
        SelectedMap = playableMaps.First(x => x.mapName == mapName);
    }
    #endregion

    #region Room
    public void CreateRoom()
    {
        Launcher.Instance.CreateRoom(RoomName, GetRoomOptions());
    }

    public void JoinedRoom()
    {
        Player[] players = PhotonNetwork.PlayerList;
        
        foreach (Transform transform in playerListContent)
        {
            Destroy(transform.gameObject);
        }

        for (int i = 0; i < players.Count(); i++)
        {
            Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().Setup(players[i]);
        }

        SetupRoom();
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient); // If new host <-
    }

    void SetupRoom() // WIP
    {
        var room = Helper.ConvertRoomInfoToDict(PhotonNetwork.CurrentRoom);

        mapName.text = room.MapName;
        gameType.text = room.GameType;
        condition.text = room.Condition;
        mapImage.sprite = room.Image;

        SetMap(room.MapName);

        startGameButton.SetActive(PhotonNetwork.IsMasterClient); // If host <-
    }

    public RoomOptions GetRoomOptions() // Show options in lobby
    {
        var options = new RoomOptions();
        options.MaxPlayers = 10; // Cap at 10, fix later
        options.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();
        options.CustomRoomProperties.Add("GameType", GameType);
        options.CustomRoomProperties.Add("Conditions", Conditions);
        options.CustomRoomProperties.Add("Time", Time);
        options.CustomRoomProperties.Add("Score", Score);
        options.CustomRoomProperties.Add("Map", SelectedMap.mapName);
        options.CustomRoomPropertiesForLobby = new string[] { "GameType", "Conditions", "Time", "Score", "Map" };
        return options;
    }
    #endregion

    #region Lobby
    public void UpdateRoomList(List<RoomInfo> roomList)
    {
        foreach (Transform transform in roomListContent)
        {
            Destroy(transform.gameObject);
        }
        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].RemovedFromList)
                continue;
            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetupRoom(roomList[i]);
        }
    }
    #endregion

    #region Scene Load
    public override void OnEnable() // Called when we load a scene
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable() // Called when we unload a scene
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.buildIndex == 1) // We're in game scene
        {
            //PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
        }
    }
    #endregion

    #region Game
    public void StartGame()
    {
        Launcher.Instance.StartGame(SelectedMap.mapName);
    }
    #endregion
}
