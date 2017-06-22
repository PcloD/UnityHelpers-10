using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingSource : TargettingSource
{
    #region RotatingFields

    public KeyCode[] RotatingKeys;
    public int InputExpropriationLevel = 1;

    #endregion //RotatingFields

    #region InputAnalysis

    protected InputExpropriation m_InputExpropriation = null;

    protected void AssumeInputClaimed()
    {
        if (null != m_InputExpropriation)
            return;

        m_InputExpropriation = InputExpropriator.CreateInputExpropriation(InputExpropriator.InputType.MouseMovement, InputExpropriationLevel);
    }

    protected void AssumeInputReleased()
    {
        if (null == m_InputExpropriation)
            return;

        InputExpropriator.ReleaseInputExpropriation(m_InputExpropriation);
        m_InputExpropriation = null;
    }

    protected bool IsRotatingActive()
    {
        bool result = false;

        for(int i = 0; i < RotatingKeys.Length; ++i)
        {
            if (!Input.GetKey(RotatingKeys[i]))
                continue;

            result = true;
            break;
        }

        return result;
    }

    #endregion //InputAnalysis

    #region Logic

    protected void RotateTargetMarkerAccordingToMouseMovement()
    {
        if (null == m_InputExpropriation || null == m_TargettingHandle)
            return;

        var handleRB = m_TargettingHandle.GetComponent<Rigidbody>();
        if (!m_InputExpropriation.IsActive() || null == handleRB || null == SourcePhysicsHandle)
            return;

        Vector3 mouseOffset = new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0f);

        SourcePhysicsHandle.connectedBody = null;
        m_TargettingHandle.transform.Rotate(mouseOffset, Space.Self);
        SourcePhysicsHandle.connectedBody = handleRB;
    }

    protected override void UpdateTarget()
    {
        base.UpdateTarget();

        if (IsRotatingActive())
        {
            AssumeInputClaimed();
            RotateTargetMarkerAccordingToMouseMovement();
        }
        else
        {
            AssumeInputReleased();
        }
    }

    #endregion //Logic
}
