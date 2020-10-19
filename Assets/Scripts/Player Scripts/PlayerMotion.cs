using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotion : MonoBehaviour
{
    [SerializeField]
    float speed, sprintModifier, jumpForce;
    [SerializeField]
    Camera eyeCam;
    [SerializeField]
    Transform groundCheck;

    Rigidbody rb;
    float baseFOV;
    float sprintFOVModifier = 1.25f;

    bool isGrounded = false;

    void Start()
    {
        Camera.main.enabled = false;
        rb = GetComponent<Rigidbody>();
        baseFOV = eyeCam.fieldOfView;
    }

    private void Update()
    {
        Jump();
        CheckGrounded();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void CheckGrounded()
    {
        isGrounded = Physics.Raycast(groundCheck.position, Vector3.down, 0.2f);
    }

    void Move()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        bool isSprinting = Sprinting(vertical);
        
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
