using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]

public class PlayerController : MonoBehaviour
{
    /* Movement */
    public float ForwardSpeed = 15.0f;
    public float MaxForwardSpeedChange = 10.0f;

    public float JumpForce = 5.0f;
    public float JumpStopperTime = 0.1f;
    public float JumpVelocityTreshold = 0.01f;
    protected float m_JumpTimer = 0.0f;
    /* Rotation */
    public Camera PlayerCamera;

    void SetupMovement()
    {
        var rigidbody = GetComponent<Rigidbody>();
        rigidbody.freezeRotation = true;
        rigidbody.useGravity = false;
    }

    void SetupRotation()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Start()
    {
        SetupMovement();
        SetupRotation();
    }

    void UpdateCursorLock()
    {
        if (Input.GetMouseButtonDown(0))
            Cursor.lockState = CursorLockMode.Locked;
    }
    
    void UpdateJumpingTest()
    {
        var rigidbody = GetComponent<Rigidbody>();

        if (Mathf.Abs(rigidbody.velocity.y) > JumpVelocityTreshold)
            m_JumpTimer = JumpStopperTime;
        else
            m_JumpTimer -= Time.deltaTime;

        if (m_JumpTimer < 0.0f)
            m_JumpTimer = 0.0f;
    }

    bool IsTouchingGround()
    {
        var rb = GetComponent<Rigidbody>();

        if (Mathf.Abs(rb.velocity.y) > JumpVelocityTreshold)
            return false;

        if (m_JumpTimer > 0.0f)
            return false;

        return true;
    }

    void MoveForward()
    {
        var rigidbody = GetComponent<Rigidbody>();

        if (!IsTouchingGround())
            return;
        
        Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        targetVelocity = transform.TransformDirection(targetVelocity);
        targetVelocity *= ForwardSpeed;

        // Apply a force that attempts to reach our target velocity
        Vector3 velocity = rigidbody.velocity;
        Vector3 velocityChange = (targetVelocity - velocity);
        velocityChange.x = Mathf.Clamp(velocityChange.x, -MaxForwardSpeedChange, MaxForwardSpeedChange);
        velocityChange.z = Mathf.Clamp(velocityChange.z, -MaxForwardSpeedChange, MaxForwardSpeedChange);
        velocityChange.y = 0;


        Transform forwardTransform = transform;
        if (null != PlayerCamera && null != PlayerCamera.transform)
            forwardTransform = PlayerCamera.transform;

        rigidbody.AddForce(forwardTransform.rotation * velocityChange, ForceMode.VelocityChange);

        var v = rigidbody.velocity;
        v.y = 0.0f;
        if (v.magnitude > MaxForwardSpeed)
        {
            v = v.normalized * MaxForwardSpeed;
            rigidbody.velocity = new Vector3(v.x, rigidbody.velocity.y, v.z);
        }
    }

    void Jump()
    {
        if (!IsTouchingGround())
            return;

        if (!Input.GetKeyDown(KeyCode.Space))
            return;

        var rigidbody = GetComponent<Rigidbody>();
        rigidbody.AddForce(transform.up * JumpForce, ForceMode.Impulse);
    }

    void RotateCamera()
    {
        if (Cursor.lockState != CursorLockMode.Locked)
            return;

        if (null == PlayerCamera)
            return;

        // From the marvelous internet
        Vector3 mouseOffset = new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0f);
        PlayerCamera.transform.Rotate(mouseOffset, Space.Self);
        PlayerCamera.transform.LookAt(PlayerCamera.transform.position + PlayerCamera.transform.forward, Vector3.up);

        // Stop rb rotation
        GetComponent<Rigidbody>().angularVelocity = new Vector3();
    }

    void Update()
    {
        UpdateCursorLock();

        UpdateJumpingTest();

        MoveForward();
        Jump();
        RotateCamera();
    }
}
