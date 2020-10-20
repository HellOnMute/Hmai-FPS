using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [SerializeField]
    Transform player, eyeCam, weapon;

    [SerializeField]
    float xSensitivity, ySensitivity, maxAngle;

    Quaternion camCenter;

    void Start()
    {
        camCenter = eyeCam.localRotation;
    }
    
    void Update()
    {
        RotateEyes();
        RotateBody();
        CursorLock();
    }

    void CursorLock()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void RotateEyes()
    {
        float input = Input.GetAxis("Mouse Y") * ySensitivity * Time.deltaTime;
        Quaternion adjustment = Quaternion.AngleAxis(input, -Vector3.right);
        Quaternion delta = eyeCam.localRotation * adjustment;

        if (Quaternion.Angle(camCenter, delta) < maxAngle)
        {
            eyeCam.localRotation = delta;
            weapon.localRotation = delta;
        }
            
    }

    void RotateBody()
    {
        float input = Input.GetAxis("Mouse X") * ySensitivity * Time.deltaTime;
        Quaternion adjustment = Quaternion.AngleAxis(input, Vector3.up);
        Quaternion delta = player.localRotation * adjustment;
        player.localRotation = delta;
    }
}
