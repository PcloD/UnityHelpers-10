using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]

public class PlayerController : MonoBehaviour
{
    /* Movement */
    public float ForwardForce = 15.0f;
    public float MaxForwardSpeed = 10.0f;

    public float ConstantFriction = 2.0f;

    /* Jumping */
    public float JumpForce = 5.0f;
    public float JumpStopperTime = 0.1f;
    public float JumpVelocityTreshold = 0.01f;
    protected float m_JumpTimer = 0.0f;
    
    /* Rotation */
    public Camera PlayerCamera;

    #region Setup

    void SetupRotation()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Start()
    {
        SetupRotation();
    }

    #endregion //Setup

    #region Utilities

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

    #endregion //Utilities

    #region Movement

    void MoveForward()
    {
        var rigidbody = GetComponent<Rigidbody>();

        if (!IsTouchingGround())
            return;

        Vector3 playerForce = new Vector3();

        if (Input.GetKey(KeyCode.W))
            playerForce += Vector3.forward * ForwardForce;
        if (Input.GetKey(KeyCode.S))
            playerForce -= Vector3.forward * ForwardForce;
        if (Input.GetKey(KeyCode.A))
            playerForce += Vector3.left * ForwardForce;
        if (Input.GetKey(KeyCode.D))
            playerForce -= Vector3.left * ForwardForce;

        Transform forwardTransform = transform;
        if (null != PlayerCamera && null != PlayerCamera.transform)
            forwardTransform = PlayerCamera.transform;

        // Rotate to camera
        playerForce = forwardTransform.rotation * playerForce;
        // Ground force
        var forceLength = playerForce.magnitude;
        playerForce.y = 0.0f;
        playerForce = playerForce.normalized * forceLength;

        rigidbody.AddForce(playerForce);

        var v = rigidbody.velocity;
        v.y = 0.0f;
        if (v.magnitude > MaxForwardSpeed)
        {
            v = v.normalized * MaxForwardSpeed;
            rigidbody.velocity = new Vector3(v.x, rigidbody.velocity.y, v.z);
        }
        
    }

    void ApplyFriction()
    {
        var rigidbody = GetComponent<Rigidbody>();

        if (!IsTouchingGround())
            return;

        var v = rigidbody.velocity;
        v.y = 0.0f;
        rigidbody.AddForce(-v * ConstantFriction);
    }

    #endregion //Movement

    #region Jumping

    void Jump()
    {
        if (!IsTouchingGround())
            return;

        if (!Input.GetKeyDown(KeyCode.Space))
            return;

        var rigidbody = GetComponent<Rigidbody>();
        rigidbody.AddForce(transform.up * JumpForce, ForceMode.Impulse);
    }

    #endregion //Jumping

    #region Rotation

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

    #endregion //Rotation

    void Update()
    {
        UpdateCursorLock();

        UpdateJumpingTest();

        MoveForward();
        ApplyFriction();
        Jump();
        RotateCamera();
    }
}
