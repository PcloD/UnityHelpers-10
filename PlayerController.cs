using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    /* Movement */
    public float ForwardForce = 15.0f;
    public float MaxForwardSpeed = 10.0f;

    public float ConstantFriction = 2.0f;
    public float YPositionTreshold = 1.05f;

    public float JumpForce = 5.0f;

    /* Rotation */
    public Camera PlayerCamera;

    void SetupRotation()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Start()
    {
        SetupRotation();
    }

    void UpdateCursorLock()
    {
        if (Input.GetMouseButtonDown(0))
            Cursor.lockState = CursorLockMode.Locked;
    }

    void MoveForward()
    {
        if (transform.position.y > YPositionTreshold)
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

        var rb = GetComponent<Rigidbody>();
        if (null != rb)
        {
            Transform forwardTransform = transform;
            if (null != PlayerCamera && null != PlayerCamera.transform)
                forwardTransform = PlayerCamera.transform;

            rb.AddForce(forwardTransform.rotation * playerForce);

            var v = rb.velocity;
            v.y = 0.0f;
            if (v.magnitude > MaxForwardSpeed)
            {
                v = v.normalized * MaxForwardSpeed;
                rb.velocity = new Vector3(v.x, rb.velocity.y, v.z);
            }
        }
    }

    void ApplyFriction()
    {
        var rb = GetComponent<Rigidbody>();
        if (null == rb)
            return;

        if (transform.position.y > YPositionTreshold)
            return;

        var v = rb.velocity;
        v.y = 0.0f;
        rb.AddForce(-v * ConstantFriction);
    }

    void Jump()
    {
        if (!Input.GetKeyDown(KeyCode.Space))
            return;

        var rb = GetComponent<Rigidbody>();
        if (null == rb)
            return;

        rb.AddForce(transform.up * JumpForce, ForceMode.Impulse);
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
        if (null != GetComponent<Rigidbody>())
            GetComponent<Rigidbody>().angularVelocity = new Vector3();
    }

    void Update()
    {
        UpdateCursorLock();

        MoveForward();
        ApplyFriction();
        Jump();
        RotateCamera();
    }
}
