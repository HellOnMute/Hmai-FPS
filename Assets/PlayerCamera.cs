using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviourPun
{
    [SerializeField]
    float mouseSensitivity, maxVerticalAngle;

    [SerializeField]
    Camera eyeCam;
    [SerializeField]
    Transform hands;

    float verticalLookRotation;
    float baseFOV;
    float sprintFOVModifier = 1.25f;

    void Start()
    {
        baseFOV = eyeCam.fieldOfView;
        if (!photonView.IsMine)
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
        }
        else
        {
            GetComponentInChildren<Sway>().enabled = photonView.IsMine;
        }
    }

    void Update()
    {
        if (!photonView.IsMine)
            return;

        Look();
        Sprinting();
    }

    void Look()
    {
        transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivity);
        verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -maxVerticalAngle, maxVerticalAngle);

        eyeCam.transform.parent.localEulerAngles = Vector3.left * verticalLookRotation;
        hands.transform.localEulerAngles = Vector3.left * verticalLookRotation;
    }

    void Sprinting()
    {
        bool sprinting = Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.W);

        if (sprinting)
            eyeCam.fieldOfView = Mathf.Lerp(eyeCam.fieldOfView, baseFOV * sprintFOVModifier, Time.deltaTime * 5f);
        else
            eyeCam.fieldOfView = Mathf.Lerp(eyeCam.fieldOfView, baseFOV, Time.deltaTime * 6f);
    }
}
