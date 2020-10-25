using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour, IPunObservable
{
    [SerializeField]
    float movementSpeed = 10f, sprintingMultiplier = 1.2f, gravity = 20f;

    Vector3 moveDirection = Vector3.zero;
    CharacterController controller;

    PhotonView pv;
    Animator anim;

    // Network vectors
    Vector3 networkPosition = Vector3.zero;
    Quaternion networkRotation = Quaternion.identity;

    void Start()
    {
        pv = GetComponent<PhotonView>();
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (!pv.IsMine)
        {
            transform.position = Vector3.MoveTowards(transform.position, networkPosition, Time.deltaTime);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, networkRotation, Time.deltaTime * 100.0f);
        }
        else
        {
            Move();
        }        
    }

    void Move()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");

        pv.RPC("MoveAnimations", RpcTarget.All, inputY, inputX, IsSprinting(inputY));

        if (controller.isGrounded)
        {
            moveDirection = new Vector3(inputX, 0, inputY).normalized;
            moveDirection *= movementSpeed;
            moveDirection *= IsSprinting(inputY) ? sprintingMultiplier : 1f;
            moveDirection = transform.TransformDirection(moveDirection);
        }

        Jump();        

        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);
    }

    [PunRPC]
    void MoveAnimations(float vertical, float horizontal, bool isSprinting)
    {
        anim.SetFloat("Moving", Mathf.Abs(vertical));
        anim.SetBool("Sideways", Mathf.Abs(vertical) == 0 && Mathf.Abs(horizontal) > 0);
        anim.SetBool("Sprinting", isSprinting);
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && controller.isGrounded)
            moveDirection.y = 10f;
    }

    bool IsSprinting(float inputY)
    {
        return Input.GetKey(KeyCode.LeftShift) && inputY > 0;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
        }
    }
}
