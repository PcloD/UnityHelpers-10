using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InputReactor))]

public class DragZone : MonoBehaviour
{
    #region Settings

    public bool DragHorizontal = true;
    public float MaxDragScreenPercent = 40.0f;
    public float DragScreenPercentRequiredToSwitch = 15.0f;
    protected float m_MaxDragWorldOffset;
    protected float m_DragWorldToSwitchOffset;

    public GameObject DragZoneCenterPositionMarker;

    public List<DragZoneElement> DragZoneElements;
    public int CurrentDragZoneElementIndex = 0;

    public float DragZoneElementPositionCorrectionSpeed = 10.0f;

    #endregion //Settings

    #region Signals

    private void Start()
    {
        RegisterInputEvents();
        RefreshMaxDragOffset();
        RefreshDragToSwitchOffset();
        CalculateDragZoneElementIndex();
    }

    private void Update()
    {
        UpdateDragZoneElementPositionCorrection();
    }

    #endregion //Signals

    #region EventsHandling

    protected virtual void RegisterInputEvents()
    {
        GetComponent<InputReactor>().RegisterEvent(InputController.EInputEventState.DragStart, OnDragStart);
        GetComponent<InputReactor>().RegisterEvent(InputController.EInputEventState.DragUpdate, OnDragUpdate);
        GetComponent<InputReactor>().RegisterEvent(InputController.EInputEventState.DragEnd, OnDragEnd);
    }

    protected Vector3 m_BeginningDragWorldPos;
    protected Vector3 m_TransformBeginningPos;

    protected float GetTotalWorldDragDiff()
    { 
        var currentDragScreenPos = InputController.GetInputPosition();
        var currentDragWorldPos = Utilities.ScreenToWorldPos(currentDragScreenPos);

        var totalDragWorldDiff = currentDragWorldPos - m_BeginningDragWorldPos;
        return DragHorizontal ? totalDragWorldDiff.x : totalDragWorldDiff.y;
    }

    protected virtual void OnDragStart(InputController inputController)
    {
        //TODO:SZ - refactor
        m_IsDragging = true;

        var dragStartScreenPos = inputController.GetInputEventStartPos();
        m_BeginningDragWorldPos = Utilities.ScreenToWorldPos(dragStartScreenPos);

        m_TransformBeginningPos = transform.position;
    }

    protected virtual void OnDragUpdate(InputController inputController)
    {
        //TODO:SZ - refactor
        var totalDragWorldDiff = Mathf.Clamp(GetTotalWorldDragDiff(), -m_MaxDragWorldOffset, m_MaxDragWorldOffset);

        Vector3 worldDragOffset = new Vector3();
        if (DragHorizontal)
            worldDragOffset.x = totalDragWorldDiff;
        else
            worldDragOffset.y = totalDragWorldDiff;

        transform.position = m_TransformBeginningPos + worldDragOffset;
    }

    protected virtual void OnDragEnd(InputController inputController)
    {
        //TODO:SZ - refactor
        m_IsDragging = false;

        var totalDragWorld = GetTotalWorldDragDiff();
        var absTotalDragWorld = Mathf.Abs(totalDragWorld);
        if (absTotalDragWorld < m_DragWorldToSwitchOffset)
            return;

        if (totalDragWorld > 0.0f)
            --CurrentDragZoneElementIndex;
        else
            ++CurrentDragZoneElementIndex;
        CalculateDragZoneElementIndex();
    }

    #endregion //EventsHandling

    #region DraggingHandling

    protected bool m_IsDragging = false;

    protected static Vector2 ScreenPercentToWorldSize(float screenPercent)
    {
        var screenWorldSpaceSize = (screenPercent / 100.0f) * Utilities.ScreenToWorldPos(new Vector2(Screen.width, Screen.height));
        return 2 * screenWorldSpaceSize;
    }

    protected virtual void RefreshMaxDragOffset()
    {
        var maxWorldDragOffset = ScreenPercentToWorldSize(MaxDragScreenPercent);
        m_MaxDragWorldOffset = DragHorizontal ? maxWorldDragOffset.x : maxWorldDragOffset.y;
    }

    protected virtual void RefreshDragToSwitchOffset()
    {
        var worldDragToSwitchOffset = ScreenPercentToWorldSize(DragScreenPercentRequiredToSwitch);
        m_DragWorldToSwitchOffset = DragHorizontal ? worldDragToSwitchOffset.x : worldDragToSwitchOffset.y;
    }

    protected virtual void CalculateDragZoneElementIndex()
    {
        if (DragZoneElements.Count <= 1)
            CurrentDragZoneElementIndex = 0;
        else
            CurrentDragZoneElementIndex = CurrentDragZoneElementIndex % DragZoneElements.Count;

        if (CurrentDragZoneElementIndex < 0)
            CurrentDragZoneElementIndex += DragZoneElements.Count;
    }

    protected virtual void UpdateDragZoneElementPositionCorrection()
    {
        if (m_IsDragging)
            return;

        var currentDragZoneElement = DragZoneElements[CurrentDragZoneElementIndex];
        Utilities.Assert(null != currentDragZoneElement);

        var currentDragZoneElementPosition = currentDragZoneElement.transform.position;
        var targetDragZoneElementPosition = DragZoneCenterPositionMarker.transform.position;
        
        var positionDiff = targetDragZoneElementPosition - currentDragZoneElementPosition;

        positionDiff *= DragZoneElementPositionCorrectionSpeed * Time.deltaTime;
        transform.position += positionDiff;
    }

    //TODO:SZ - refactor kodu - tutaj niech idzie domena dragowania, a domena eventow do EventsHandling

    #endregion //DraggingHandling
}
