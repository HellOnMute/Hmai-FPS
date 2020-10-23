using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IPunObservable
{
    [SerializeField]
    float speed, sprintModifier, jumpForce, mouseSensitivity, maxVerticalAngle;
    [SerializeField]
    Camera eyeCam;
    [SerializeField]
    Transform hands, groundCheck;

    float baseFOV;
    float sprintFOVModifier = 1.25f;

    float verticalLookRotation;
    bool isGrounded = false;
    float horizontal, vertical;
    bool isSprinting = false;
    PhotonView pv;
    Rigidbody rb;

    // Network vectors
    Vector3 networkPosition = Vector3.zero;
    Quaternion networkRotation = Quaternion.identity;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        pv = GetComponent<PhotonView>();
        baseFOV = eyeCam.fieldOfView;

        //Cursor.lockState = CursorLockMode.Locked;
    }

    void Start()
    {
        if (!pv.IsMine)
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
        }
        else
        {
            GetComponentInChildren<Sway>().enabled = pv.IsMine;
        }
    }

    private void Update()
    {
        if (!pv.IsMine)
            return;

        Jump();
        Look();
        CheckGrounded();

        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        isSprinting = Sprinting(vertical);
    }

    private void FixedUpdate()
    {
        if (!pv.IsMine)
        {
            transform.position = Vector3.MoveTowards(rb.position, networkPosition, Time.fixedDeltaTime);
            transform.rotation = Quaternion.RotateTowards(rb.rotation, networkRotation, Time.fixedDeltaTime * 100.0f);
        }
        else
        {
            Move();
        }
    }


    private void CheckGrounded()
    {
        isGrounded = Physics.Raycast(groundCheck.position, Vector3.down, 0.2f);
    }

    void Look()
    {
        transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivity);
        verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -maxVerticalAngle, maxVerticalAngle);

        eyeCam.transform.parent.localEulerAngles = Vector3.left * verticalLookRotation;
        hands.transform.localEulerAngles = Vector3.left * verticalLookRotation;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(rb.position);
            stream.SendNext(rb.rotation);
            stream.SendNext(rb.velocity);
        }
        else
        {
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
            rb.velocity = (Vector3)stream.ReceiveNext();

            float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
            networkPosition += (rb.velocity * lag);
        }
    }

    void Move() // Local move
    {
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
        direction = transform.TransformDirection(direction) * speed * (isSprinting ? sprintModifier : 1) * Time.deltaTime;
        direction.y = rb.velocity.y;
        
        rb.velocity = direction;
    }

    void Jump()
    {
        if (!Input.GetKeyDown(KeyCode.Space) || !isGrounded)
            return;

        rb.velocity = Vector3.zero;
        rb.AddForce(Vector3.up * jumpForce);
    }

    bool Sprinting(float verticalMovement)
    {
        bool sprinting = Input.GetKey(KeyCode.LeftShift) && verticalMovement > 0;

        if (sprinting)
            eyeCam.fieldOfView = Mathf.Lerp(eyeCam.fieldOfView, baseFOV * sprintFOVModifier, Time.deltaTime * 5f);
        else
            eyeCam.fieldOfView = Mathf.Lerp(eyeCam.fieldOfView, baseFOV, Time.deltaTime * 6f);

        return sprinting;
    }
}
