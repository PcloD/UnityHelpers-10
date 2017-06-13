using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggable : Targetable
{
    #region TargettingLogic

    public float DragForce = 10.0f;
    public float AngularRotationFactor = 0.2f;

    public override void UpdateTargetting()
    {
        if (!HasTargettingSource())
            return;

        var physicsHandleMarker = m_TargettingSource.GetTargettingHandle().transform;
        var currentPosition = transform.position;
        var destPosition = physicsHandleMarker.position;
        var deltaPosition = destPosition - currentPosition;

        var destRotation = physicsHandleMarker.rotation;
        var currentRotation = transform.rotation;
        var deltaRotation = Quaternion.Slerp(currentRotation, destRotation, AngularRotationFactor);

        var rigidbody = GetComponent<Rigidbody>();
        if (null != rigidbody)
        {
            rigidbody.velocity = DragForce * deltaPosition;

            rigidbody.MoveRotation(deltaRotation);
            rigidbody.angularVelocity = new Vector3();
        }
    }

    #endregion //TargettingLogic
}
