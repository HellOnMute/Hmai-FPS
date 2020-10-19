using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Headbob : MonoBehaviour
{
    [SerializeField]
    Transform objectToBob;

    [SerializeField]
    float xIntensityIdle = 0.025f, 
        yIntensityIdle = 0.025f, 
        xIntensityMoving = 0.05f, 
        yIntensityMoving = 0.05f, 
        moveMultiplier = 3.5f, 
        sprintMultiplier = 7f;

    float bobCurve = 1, moveCurve = 1;

    Vector3 objectToBobOrigin;
    Vector3 targetBobPosition;
    void Start()
    {
        objectToBobOrigin = objectToBob.localPosition;
    }

    void Update()
    {
        if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0)
        {
            Headbobbing(bobCurve, xIntensityIdle, yIntensityIdle);
            bobCurve += Time.deltaTime;
        }
        else
        {
            Headbobbing(bobCurve, xIntensityMoving, yIntensityMoving);
            if (Input.GetKey(KeyCode.LeftShift))
                bobCurve += Time.deltaTime * sprintMultiplier;
            else
                bobCurve += Time.deltaTime * moveMultiplier;
        }

        objectToBob.localPosition = Vector3.Lerp(objectToBob.localPosition, targetBobPosition, Time.deltaTime * 8f);
    }

    void Headbobbing(float curveCounter, float xIntensity, float yIntensity)
    {
        targetBobPosition = objectToBobOrigin + new Vector3(Mathf.Cos(curveCounter) * xIntensity, Mathf.Sin(curveCounter * 2) * yIntensity, 0f);
    }
}
