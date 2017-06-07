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
    protected Vector3 m_TargettingOffset = new Vector3();
    protected Quaternion m_TargetRotation;

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
    }

    protected virtual void AttachTarget(Targetable target, RaycastHit targettingInfo)
    {
        if (HasTarget())
            ReleaseTarget();

        m_OwnTarget = target;
        if (null == target)
            return;

        m_TargetDistance = targettingInfo.distance;
        m_TargettingOffset = targettingInfo.point - target.transform.position;
        m_TargetRotation = target.gameObject.transform.rotation;

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

    public virtual Targetable GetTarget()
    {
        return m_OwnTarget;
    }

    public virtual float GetTargettingDistance()
    {
        return m_TargetDistance;
    }

    public virtual Vector3 GetTargettingOffset()
    {
        return m_TargettingOffset;
    }

    public virtual Quaternion GetTargettingRotation()
    {
        return m_TargetRotation;
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
