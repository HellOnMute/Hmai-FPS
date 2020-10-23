using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    PhotonView PV;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        //Camera.main.enabled = false;
    }

    void Start()
    {
        if (PV.IsMine) // Is true of pv is owned by local player
        {
            CreateController();
        }
    }

    void CreateController()
    {
        // Instantiate our player controller
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player"), Vector3.zero, Quaternion.identity);
    }
}
