using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helper : MonoBehaviour
{
    public static RoomInfoUIConverted ConvertRoomInfoToDict(RoomInfo info)
    {
        var data = new RoomInfoUIConverted();
        data.RoomName = info.Name;
        data.MapName = info.CustomProperties["Map"].ToString();
        data.PlayerCount = $"{info.PlayerCount}/{info.MaxPlayers}";
        Sprite img = RoomManager.Instance.GetMapByName(info.CustomProperties["Map"].ToString()).image;
        data.Image = img;

        switch ((GameType)info.CustomProperties["GameType"])
        {
            case GameType.Deathmatch:
                data.GameType = "Deathmatch";
                break;
            case GameType.TeamDeathmatch:
                data.GameType = "Team Deathmatch";
                break;
            case GameType.CaptureTheFlag:
                data.GameType = "Capture The Flag";
                break;
        }

        if ((Conditions)info.CustomProperties["Conditions"] == Conditions.Score)
        {
            switch ((ScoreCondition)info.CustomProperties["Score"])
            {
                case ScoreCondition.Three:
                    data.Condition = "Score - First to 3";
                    break;
                case ScoreCondition.Five:
                    data.Condition = "Score - First to 5";
                    break;
                case ScoreCondition.Ten:
                    data.Condition = "Score - First to 10";
                    break;
                case ScoreCondition.Fifteen:
                    data.Condition = "Score - First to 15";
                    break;
                case ScoreCondition.Twenty:
                    data.Condition = "Score - First to 20";
                    break;
                case ScoreCondition.Thirty:
                    data.Condition = "Score - First to 30";
                    break;
            }
        }
        else if ((Conditions)info.CustomProperties["Conditions"] == Conditions.Time)
        {
            switch ((TimeCondition)info.CustomProperties["Time"])
            {
                case TimeCondition.Five:
                    data.Condition = "Time - 5 Minutes";
                    break;
                case TimeCondition.Ten:
                    data.Condition = "Time - 10 Minutes";
                    break;
                case TimeCondition.Fifteen:
                    data.Condition = "Time - 15 Minutes";
                    break;
                case TimeCondition.Twenty:
                    data.Condition = "Time - 20 Minutes";
                    break;
            }
        }

        return data;
    }
}
