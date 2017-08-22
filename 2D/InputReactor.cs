using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]

public class InputReactor : MonoBehaviour
{
    public delegate void InputReactorEvent();

    protected Dictionary<InputController.EInputEventState, InputReactorEvent> m_OnStateChangedEvents;
    protected InputController.EInputEventState m_PreviousState = InputController.EInputEventState.NoInput;

    public InputReactor()
    {
        m_OnStateChangedEvents = new Dictionary<InputController.EInputEventState, InputReactorEvent>();

        foreach (InputController.EInputEventState state in System.Enum.GetValues(typeof(InputController.EInputEventState)))
            m_OnStateChangedEvents.Add(state, null);
    }

    public virtual void UpdateEventState(InputController.EInputEventState inputEventState)
    {
        if (inputEventState == m_PreviousState && inputEventState != InputController.EInputEventState.DragUpdate)
            return;

        m_PreviousState = inputEventState;

        if (null != m_OnStateChangedEvents[inputEventState])
            m_OnStateChangedEvents[inputEventState].Invoke();
    }

    public virtual void RegisterEvent(InputController.EInputEventState inputEventState, InputReactorEvent callback)
    {
        m_OnStateChangedEvents[inputEventState] += callback;
    }
}
