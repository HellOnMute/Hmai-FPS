using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // REmove later
public class PlayerCondition : MonoBehaviourPun
{
    int health = 100;

    TextMeshProUGUI hptxt;
    TextMeshProUGUI ammotxt;

    PlayerState state;
    void Start()
    {
        if (!photonView.IsMine)
            return;

        state = GetComponent<PlayerState>();
        hptxt = GameObject.FindGameObjectWithTag("HPTxt").GetComponent<TextMeshProUGUI>();
        ammotxt = GameObject.FindGameObjectWithTag("AmmoTxt").GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (!photonView.IsMine)
            return;

        hptxt.text = "Health: " + health;
        ammotxt.text = $"{state.CurrentAmmo}/{state.CurrentAmmoReserve}";

        SetPlayerStates();
    }

    void SetPlayerStates()
    {
        state.IsAlive = health > 0;
    }

    [PunRPC]
    public void TakeDamage(int damage)
    {
        health -= damage;
    }

}
