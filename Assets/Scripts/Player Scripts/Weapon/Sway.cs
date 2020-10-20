using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sway : MonoBehaviour
{
    [SerializeField]
    float swayIntensity, smooth;

    Quaternion origin;

    void Start()
    {
        origin = transform.localRotation;
    }


    void Update()
    {
        UpdateSway();
    }

    void UpdateSway()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        Quaternion xAdj = Quaternion.AngleAxis(-swayIntensity * mouseX, Vector3.up);
        Quaternion yAdj = Quaternion.AngleAxis(swayIntensity * mouseY, Vector3.right);

        Quaternion targetRotation = origin * xAdj * yAdj;

        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * smooth);
    }
}
