using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class CreateRoomSettingsHandler : MonoBehaviour
{
    [SerializeField]
    TMP_InputField roomName;

    [SerializeField]
    Image mapImage;

    [SerializeField]
    TMP_Dropdown maps;

    [SerializeField]
    TMP_Dropdown gameType, conditions, score, time;


    List<Sprite> images;
    //List<string> gameTypes = new List<string>() { }
    void Start()
    {
        List<string> mapList = RoomManager.Instance.Maps.Where(x => x.mapNiceName != "").Select(x => x.mapNiceName).ToList();
        images = RoomManager.Instance.Maps.Where(x => x.mapNiceName != "").Select(x => x.image).ToList();

        maps.ClearOptions();
        maps.AddOptions(mapList);

        mapImage.sprite = images[0];

        OnConditionSelected(0);
    }

    public void UpdateImage(int index)
    {
        mapImage.sprite = images[index];
    }

    public void OnConditionSelected(int c)
    {
        if (c == 0)
        {
            score.gameObject.SetActive(false);
            time.gameObject.SetActive(true);
        }
        else
        {
            score.gameObject.SetActive(true);
            time.gameObject.SetActive(false);
        }
    }

    public void CreateRoom()
    {
        var instance = RoomManager.Instance;
        var name = string.IsNullOrEmpty(roomName.text) || string.IsNullOrWhiteSpace(roomName.text) ? "Custom Game" : roomName.text;
        Debug.Log(roomName.text.Count() == 0);
        instance.SetRoomName(name);
        instance.SetMap(maps.value);
        instance.SetGameType(gameType.value);
        instance.SetConditions(conditions.value);
        instance.SetTime(conditions.value == 0 ? time.value + 1 : 0);
        instance.SetScore(conditions.value == 0 ? 0 : score.value + 1);
        roomName.text = "";
        instance.CreateRoom();
    }
}
