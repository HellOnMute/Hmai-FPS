using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Map/New map")]
public class PlayableMap : ScriptableObject
{
    public string mapNiceName;
    public string mapName;
    public int suggestedPlayerCount = 2;
    public string description;
    public Sprite image;
}
