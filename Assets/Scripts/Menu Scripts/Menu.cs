using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    [SerializeField] OnMenuLoaded menuLoaded;

    public string menuName;
    public bool open;

    public void Open()
    {
        open = true;
        MenuLoaded();
        gameObject.SetActive(true);

        if (menuLoaded == OnMenuLoaded.SetTitleToRoomName)
            MenuManager.Instance.SetMenuTitle(PhotonNetwork.CurrentRoom.Name);
        else
            MenuManager.Instance.SetMenuTitle(menuName);
    }

    public void Close()
    {
        open = false;
        gameObject.SetActive(false);
    }

    void MenuLoaded()
    {
        switch(menuLoaded)
        {
            case OnMenuLoaded.None:
                return;
        }
    }
}
