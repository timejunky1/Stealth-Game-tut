using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControler : MonoBehaviour
{
    public event System.Action OnReachedEndOfLevel;

    public float movementSpeed = 5;
    public float smoothMoveTime = 0.1f;
    public float turnSpeed = 8;

    float angle;
    float smoothInputmagnitude;
    float smoothMoveVelocity;
    Vector3 velocity;

    Rigidbody rb;
    bool disabled;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        Guard.OnGaurdHasSpottedPlayer += Disable;
    }

    private void Update()
    {
        Vector3 moveDirection = Vector3.zero;
        if(!disabled)
        {
            moveDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        }
        float inputMagnitude = moveDirection.magnitude;
        smoothInputmagnitude = Mathf.SmoothDamp(smoothInputmagnitude, inputMagnitude, ref smoothMoveVelocity, smoothMoveTime);

        float lookAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
        angle = Mathf.LerpAngle(angle, lookAngle, Time.deltaTime * turnSpeed * inputMagnitude);

        velocity = transform.forward * movementSpeed * smoothInputmagnitude;
        
    }
    private void FixedUpdate()
    {
        rb.MoveRotation(Quaternion.Euler(Vector3.up * angle));
        rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter(Collider colider)
    {
        if(colider.tag == "Finish")
        {
            Disable();
            if(OnReachedEndOfLevel != null)
            {
                OnReachedEndOfLevel();
            }
        }
    }

    void Disable()
    {
        disabled= true;
    }

    private void OnDestroy()
    {
        Guard.OnGaurdHasSpottedPlayer-= Disable;
    }

}
