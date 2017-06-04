using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggable : Targetable
{
    #region TargettingLogic

    public override void UpdateTargetting()
    {
        if (!HasTargettingSource())
            return;

        var source = GetTargettingSource();
        var sourceTransform = source.GetTargettingSource();

        var destPosition = sourceTransform.position + sourceTransform.forward * source.GetTargettingDistance();
        var destLocalPosition = transform.position + source.GetTargettingOffset();

        transform.position += destPosition - destLocalPosition;
    }

    #endregion //TargettingLogic
}
