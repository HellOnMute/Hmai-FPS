using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OnMenuLoaded
{
    None,
    JoinLobby,
    LeaveLobby,
    SetTitleToRoomName
}

public enum GameType
{
    Deathmatch,
    TeamDeathmatch,
    CaptureTheFlag
}

public enum Conditions
{
    Time,
    Score
}

public enum TimeCondition
{
    NotSelected,
    Five,
    Ten,
    Fifteen,
    Twenty
}

public enum ScoreCondition
{
    NotSelected,
    Three,
    Five,
    Ten,
    Fifteen,
    Twenty,
    Thirty
}
