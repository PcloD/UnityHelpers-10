using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targetable : MonoBehaviour
{
    #region TargettingFields

    protected TargettingSource m_TargettingSource;

    #endregion //TargettingFields

    #region TargettingLogic

    public virtual void AttachToTargettingSource(TargettingSource source)
    {
        if (null == source)
            return;

        if (HasTargettingSource())
            DetachFromTargettingSource();

        m_TargettingSource = source;
    }

    public virtual void DetachFromTargettingSource()
    {
        if (!HasTargettingSource())
            return;

        m_TargettingSource = null;
    }

    public virtual void UpdateTargetting()
    {
        // nothign
    }

    #endregion //TargettingLogic

    #region Accessors

    public virtual bool HasTargettingSource()
    {
        return null != GetTargettingSource();
    }

    public virtual TargettingSource GetTargettingSource()
    {
        return m_TargettingSource;
    }

    #endregion //Accessors
}
