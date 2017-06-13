using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargettingSource : MonoBehaviour
{
    #region TargettingFields

    public int TargettingMouseButton = 0;
    public Transform TargettingSourceTransform = null; //null means myself
    public float MaxTargettingDistance = 4.0f;

    protected Targetable m_OwnTarget = null;
    protected float m_TargetDistance = 0.0f;
    protected Quaternion m_TargetRotation;
    protected Quaternion m_SourceRotation;

    public Joint SourcePhysicsHandle;
    public GameObject TargettingHandleTemplate;
    protected GameObject m_TargettingHandle;

    #endregion //TargettingFields

    #region TargettingTemplateMethod

    protected virtual void LookForTarget()
    {
        var source = GetTargettingSource();
        if (null == source)
            return;

        RaycastHit hitInfo;
        if (!Physics.Raycast(source.position, source.forward, out hitInfo))
            return;

        if (hitInfo.distance > MaxTargettingDistance)
            return;

        var newTarget = hitInfo.collider.gameObject.GetComponent<Targetable>();
        if (null == newTarget)
            return;

        if(DoesTargetMatch(newTarget))
            AttachTarget(newTarget, hitInfo);
    }

    protected virtual bool DoesTargetMatch(Targetable target)
    {
        return true;
    }

    protected virtual bool HasTarget()
    {
        return null != m_OwnTarget;
    }

    protected virtual void UpdateTarget()
    {
        if (!HasTarget())
            return;

        m_OwnTarget.UpdateTargetting();
    }

    protected virtual void ReleaseTarget()
    {
        if (!HasTarget())
            return;

        m_OwnTarget.DetachFromTargettingSource();
        m_OwnTarget = null;

        if (null != SourcePhysicsHandle)
            SourcePhysicsHandle.connectedBody = null;

        if (null != m_TargettingHandle)
            Destroy(m_TargettingHandle);
        m_TargettingHandle = null;
    }

    protected virtual void AttachTarget(Targetable target, RaycastHit targettingInfo)
    {
        if (HasTarget())
            ReleaseTarget();

        m_OwnTarget = target;
        if (null == target)
            return;

        var sourceTransform = GetTargettingSource();

        m_TargetDistance = (target.transform.position - sourceTransform.position).magnitude;
        m_TargetRotation = target.gameObject.transform.rotation;
        m_SourceRotation = sourceTransform.rotation;

        m_TargettingHandle = (GameObject)Instantiate(TargettingHandleTemplate);
        m_TargettingHandle.transform.position = target.transform.position;
        m_TargettingHandle.transform.rotation = target.transform.rotation;

        Rigidbody handleRB = m_TargettingHandle.GetComponent<Rigidbody>();
        if(null != handleRB && null != SourcePhysicsHandle)
            SourcePhysicsHandle.connectedBody = handleRB;

        target.AttachToTargettingSource(this);
    }

    #endregion //TargettingTemplateMethod

    #region Accessors

    public virtual Transform GetTargettingSource()
    {
        if (null != TargettingSourceTransform)
            return TargettingSourceTransform;

        return transform;
    }

    public virtual GameObject GetTargettingHandle()
    {
        return m_TargettingHandle;
    }

    public virtual Targetable GetTarget()
    {
        return m_OwnTarget;
    }

    public virtual float GetTargettingDistance()
    {
        return m_TargetDistance;
    }

    public virtual Quaternion GetTargettingRotation()
    {
        return m_TargetRotation;
    }

    public virtual Quaternion GetSourceRotation()
    {
        return m_SourceRotation;
    }

    public virtual Quaternion GetSourceCurrentRotation()
    {
        return GetTargettingSource().rotation;
    }

    #endregion //Accessors

    #region Logic

    void HandleTargettingLogic()
    {
        // Template method design pattern
        if (Input.GetMouseButtonDown(TargettingMouseButton))
            LookForTarget();

        if (HasTarget())
            UpdateTarget();

        if (Input.GetMouseButtonUp(TargettingMouseButton))
            ReleaseTarget();
    }

    void Update()
    {
        HandleTargettingLogic();
    }

    #endregion //Logic
}
