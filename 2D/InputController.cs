using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    #region Properties

    public float ScreenSpaceDragTreshold = 10.0f;

    #endregion //Properties

    #region InputWrapper

    public static bool IsPressed()
    {
        if (Input.touchCount > 0)
            return true;
        else
            return Input.GetMouseButton(0);
    }

    public static Vector2 GetInputPosition()
    {
        if (Input.touchCount > 0)
        {
            Vector2 result = new Vector2();
            for (int i = 0; i < Input.touchCount; ++i)
                result += Input.touches[i].position;

            result /= Input.touchCount;
            return result;
        }
        else
        {
            return Input.mousePosition;
        }
    }

    public static List<InputReactor> GetInputObjects()
    {
        const int MAX_OBJECTS_TO_CHECK = 10;

        if (!IsPressed())
            return null;

        List<InputReactor> result = new List<InputReactor>();

        var inputPos = GetInputPosition();
        var worldPos = Utilities.ScreenToWorldPos(inputPos);
        RaycastHit2D[] raycastResult = new RaycastHit2D[MAX_OBJECTS_TO_CHECK];

        var foundObjectsCount = Physics2D.Raycast(worldPos, Camera.main.transform.forward, new ContactFilter2D(), raycastResult);
        for(int i = 0; i < foundObjectsCount; ++i)
        {
            if (raycastResult[i].collider == null)
                continue;

            var inputReactor = raycastResult[i].collider.gameObject.GetComponent<InputReactor>();
            if (null == inputReactor)
                continue;

            result.Add(inputReactor);
        }
        
        return result;
    }

    #endregion //InputWrapper

    #region InputHandling

    protected enum EInputState
    {
        Released,
        JustPressed,
        Pressed,
        JustReleased
    }

    protected EInputState m_InputState = EInputState.Released;

    protected void UpdateInputState()
    {
        var isPressed = IsPressed();

        switch (m_InputState)
        {
            case EInputState.Released:
                if (isPressed)
                    m_InputState = EInputState.JustPressed;
                break;
            case EInputState.JustPressed:
                m_InputState = EInputState.Pressed;
                break;
            case EInputState.Pressed:
                if (!isPressed)
                    m_InputState = EInputState.JustReleased;
                break;
            case EInputState.JustReleased:
                m_InputState = EInputState.Released;
                break;
        }
    }

    #endregion //InputHandling

    #region EventsHandling

    public enum EInputEventState
    {
        /*
                                                                ___
                                                               /   \
                                                              |     v
            NoInput -> TapStart -> TapCancel -> DragStart -> DragUpdate -> DragEnd
                                -> TapEnd
             
        */

        NoInput,
        TapStart,
        TapCancel,
        TapEnd,
        DragStart,
        DragUpdate,
        DragEnd
    }

    protected List<InputReactor> m_InputEventObjects;
    protected Vector2 m_InputEventStartPosition;
    protected EInputEventState m_InputEventState;

    public List<InputReactor> GetInputEventObjects()
    {
        return m_InputEventObjects;
    }

    public Vector2 GetInputEventStartPos()
    {
        return m_InputEventStartPosition;
    }

    public EInputEventState GetInputEventState()
    {
        return m_InputEventState;
    }

    protected void UpdateEvents()
    {
        switch(m_InputEventState)
        {
            case EInputEventState.NoInput:
                Utilities.Assert(null == m_InputEventObjects);

                if(m_InputState == EInputState.JustPressed || m_InputState == EInputState.Pressed)
                {
                    m_InputEventObjects = GetInputObjects();
                    if (null == m_InputEventObjects)
                        break;

                    m_InputEventStartPosition = GetInputPosition();
                    m_InputEventState = EInputEventState.TapStart;
                }
                break;
            case EInputEventState.TapStart:
                Utilities.Assert(null != m_InputEventObjects);

                var currentPos = GetInputPosition();
                var distance = (currentPos - m_InputEventStartPosition).magnitude;
                if(distance > ScreenSpaceDragTreshold)
                {
                    m_InputEventState = EInputEventState.TapCancel; // start drag. Machine state can cancel a tap only by starting a drag
                    break;
                }

                if(m_InputState == EInputState.JustReleased || m_InputState == EInputState.Released)
                {
                    m_InputEventState = EInputEventState.TapEnd;
                    break;
                }

                break;
            case EInputEventState.TapCancel:
                Utilities.Assert(null != m_InputEventObjects);
                m_InputEventState = EInputEventState.DragStart;
                break;
            case EInputEventState.TapEnd:
                Utilities.Assert(null != m_InputEventObjects);
                m_InputEventState = EInputEventState.NoInput;
                m_InputEventObjects = null;
                break;
            case EInputEventState.DragStart:
                Utilities.Assert(null != m_InputEventObjects);
                m_InputEventState = EInputEventState.DragUpdate;
                break;
            case EInputEventState.DragUpdate:
                Utilities.Assert(null != m_InputEventObjects);

                if (m_InputState == EInputState.JustReleased || m_InputState == EInputState.Released)
                {
                    m_InputEventState = EInputEventState.DragEnd;
                    break;
                }

                break;
            case EInputEventState.DragEnd:
                Utilities.Assert(null != m_InputEventObjects);
                m_InputEventState = EInputEventState.NoInput;
                m_InputEventObjects = null;
                break;
        }

        if (null != m_InputEventObjects)
        {
            foreach(var inputEventObject in m_InputEventObjects)
                inputEventObject.UpdateEventState(this, m_InputEventState);
        }
    }

    #endregion //EventsHandling

    #region Signals

    private void Update()
    {
        UpdateInputState();
        UpdateEvents();
    }

    #endregion //Signals
}
