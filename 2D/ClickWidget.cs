using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(InputReactor))]

public class ClickWidget : MonoBehaviour
{
    public UnityEvent OnClick;

    void Start()
    {
        GetComponent<InputReactor>().RegisterEvent(InputController.EInputEventState.TapStart, OnTapStart);
        GetComponent<InputReactor>().RegisterEvent(InputController.EInputEventState.TapCancel, OnTapCancel);
        GetComponent<InputReactor>().RegisterEvent(InputController.EInputEventState.TapEnd, OnTapEnd);
    }

    void OnTapStart()
    {
        Debug.Log("Start");
    }

    void OnTapCancel()
    {
        Debug.Log("Cancel");
    }

    void OnTapEnd()
    {
        Debug.Log("End");
        OnClick.Invoke();
    }

}
