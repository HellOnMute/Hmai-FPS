using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public class RoomListItem : MonoBehaviour
{
    [SerializeField]
    Image mapImage;

    [SerializeField]
    TextMeshProUGUI gameName, gameType, gameConditions, playerCount;

    RoomInfo info;

    public void SetupRoom(RoomInfo info)
    {
        this.info = info;
        var room = Helper.ConvertRoomInfoToDict(info);
        gameName.text = room.RoomName;
        gameType.text = room.GameType;
        gameConditions.text = room.Condition;
        playerCount.text = room.PlayerCount;
        mapImage.sprite = room.Image;
    }

    public void OnClickJoinRoom()
    {
        Launcher.Instance.JoinRoom(info);
    }
}
