using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggable : Targetable
{
    #region TargettingLogic

    public float DragForce = 10.0f;
    public float AngularRotationFactor = 15.0f;

    public override void UpdateTargetting()
    {
        if (!HasTargettingSource())
            return;

        var source = GetTargettingSource();
        var sourceTransform = source.GetTargettingSource();

        var destPosition = sourceTransform.position + sourceTransform.forward * source.GetTargettingDistance();
        var destLocalPosition = transform.position + source.GetTargettingOffset();
        var deltaDistance = destPosition - destLocalPosition;

        var targetRotation = source.GetTargettingRotation();
        var currentRotation = transform.rotation;
        var destRotation = Quaternion.Slerp(currentRotation, targetRotation, AngularRotationFactor * Time.deltaTime);

        var rigidbody = GetComponent<Rigidbody>();
        if (null != rigidbody)
        {
            rigidbody.velocity = DragForce * deltaDistance;

            rigidbody.MoveRotation(destRotation);
            rigidbody.angularVelocity = new Vector3();
        }
    }

    #endregion //TargettingLogic

    #region Utilities

    public static Vector3 NormalizeAnglesVector(Vector3 anglesInput)
    {
        anglesInput.x = NormalizeAngle(anglesInput.x);
        anglesInput.y = NormalizeAngle(anglesInput.y);
        anglesInput.z = NormalizeAngle(anglesInput.z);

        return anglesInput;
    }

    public static float NormalizeAngle(float angleInput)
    {
        while (angleInput > 180.0f)
            angleInput -= 360.0f;

        while(angleInput < -180.0f)
            angleInput += 360.0f;

        return angleInput;
    }

    #endregion //Utilities
}
