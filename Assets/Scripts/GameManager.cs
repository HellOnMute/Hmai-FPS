using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance;

    List<Player> playerList;

    // General Game Settings
    private string gameName;
    private GameType gameType;
    private Conditions conditions;
    private TimeCondition timeCondition;
    private ScoreCondition scoreCondition;
    private PlayableMap selectedMap;

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

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public override void OnJoinedRoom()
    {
        // Update player list?
        //Player[] copiedList;
        //playerList.CopyTo(copiedList.ToList());

        //for (int i = 0; i < copiedList.Length; i++)
        //{

        //}
    }

    #region Setup Game Settings
    void SetGameSettings()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        gameName = RoomManager.Instance.RoomName;
        gameType = RoomManager.Instance.GameType;
        conditions = RoomManager.Instance.Conditions;
        timeCondition = RoomManager.Instance.Time;
        scoreCondition = RoomManager.Instance.Score;
        selectedMap = RoomManager.Instance.SelectedMap;
    }
    
    public void GetGameSettingsFromMaster()
    {
        if (PhotonNetwork.IsMasterClient)
            return;

        photonView.RPC("SendGameSettings", RpcTarget.Others);
    }

    [PunRPC]
    public void SendGameSettings()
    {
        if (PhotonNetwork.IsMasterClient)
            photonView.RPC("UpdateGameSettings", RpcTarget.Others, gameName, gameType, conditions, timeCondition, scoreCondition, selectedMap);
    }

    [PunRPC]
    private void UpdateGameSettings(string gameName, GameType gameType, Conditions conditions, TimeCondition timeCondition, ScoreCondition scoreCondition, PlayableMap selectedMap)
    {
        this.gameName = gameName;
        this.gameType = gameType;
        this.conditions = conditions;
        this.timeCondition = timeCondition;
        this.scoreCondition = scoreCondition;
        this.selectedMap = selectedMap;
    }
    #endregion
}
