using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(InputReactor))]

public class ClickWidget : MonoBehaviour
{
    #region Signals

    void Start()
    {
        RegisterInputEvents();
    }

    #endregion //Signals

    #region EventsHandling

    protected virtual void RegisterInputEvents()
    {
        GetComponent<InputReactor>().RegisterEvent(InputController.EInputEventState.TapStart, OnTapStart);
        GetComponent<InputReactor>().RegisterEvent(InputController.EInputEventState.TapCancel, OnTapCancel);
        GetComponent<InputReactor>().RegisterEvent(InputController.EInputEventState.TapEnd, OnTapEnd);
    }

    public UnityEvent OnClick;

    protected virtual void OnTapStart(InputController inputController)
    {
        Debug.Log("Start");
    }

    protected virtual void OnTapCancel(InputController inputController)
    {
        Debug.Log("Cancel");
    }

    protected virtual void OnTapEnd(InputController inputController)
    {
        Debug.Log("End");
        OnClick.Invoke();
    }

    #endregion //EventsHandling
}
