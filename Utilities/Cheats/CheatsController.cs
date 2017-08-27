using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CheatsController : MonoBehaviour
{
    #region Singleton

    private static CheatsController s_Instance;

    public static CheatsController Instance
    {
        get { return s_Instance; }
    }

    private void RegisterInstance()
    {
        s_Instance = this;
    }

    #endregion //Singleton

    #region Signals

    private void Start()
    {
        RegisterInstance();
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        HandleCheatsInput();
    }

    #endregion //Signals

    #region CheatsInput

    public GameObject CheatsInputPrefab;
    private GameObject m_CheatsInput;
    private float m_CheatTapTimer = 0.0f;
    private int m_CheatTapCounter = 0;
    private bool m_PreviousInput = false;
    private bool m_NeedToCloseInput = false;

    private void HandleCheatsInput()
    {
        HandleCheatsInputOpening();
        HandleInputClosing();
    }

    private void HandleInputClosing()
    {
        if (!m_NeedToCloseInput)
            return;

        m_NeedToCloseInput = false;
        
        if(null != m_CheatsInput)
            Destroy(m_CheatsInput.gameObject);
    }

    private void HandleCheatsInputOpening()
    {
        m_CheatTapTimer -= (Time.deltaTime / Time.timeScale);
        if (m_CheatTapTimer < 0.0f)
            m_CheatTapTimer = 0.0f;

        var currentInput = 0 != Input.touchCount || Input.GetMouseButton(0);
        var change = !currentInput && m_PreviousInput;
        m_PreviousInput = currentInput;

        if (!change)
            return;

        if (m_CheatTapTimer <= 0.0f)
            m_CheatTapCounter = 0;

        m_CheatTapTimer = 0.25f;
        ++m_CheatTapCounter;

        if (m_CheatTapCounter >= 3)
            OpenCheats();
    }

    private void OpenCheats()
    {
        if (null != m_CheatsInput)
            return;

        m_CheatTapTimer = 0.0f;
        m_CheatTapCounter = 0;

        var canvas = FindObjectOfType<Canvas>();
        if (null == canvas)
            return;

        m_CheatsInput = Instantiate(CheatsInputPrefab, canvas.transform);
        m_CheatsInput.GetComponent<InputField>().onEndEdit.AddListener(InvokeCheat);

        // Set focus
        EventSystem.current.SetSelectedGameObject(m_CheatsInput, null);
        m_CheatsInput.GetComponent<InputField>().OnPointerClick(new PointerEventData(EventSystem.current));
    }

    private void InvokeCheat(string cheatText)
    {
        char[] space = new char[1];
        space[0] = ' ';

        m_NeedToCloseInput = true;

        var argsIndex = cheatText.IndexOf(" ");
        string args = "";
        if(argsIndex >= 0)
        {
            args = cheatText.Substring(argsIndex);
            args = args.Trim(space);
        }
        var cheatName = cheatText.Substring(0, cheatText.Length - args.Length);
        cheatName = cheatName.Trim(space);

        Type type = typeof(CheatsLogic);
        MethodInfo info = type.GetMethod(cheatName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
        if (null == info)
        {
            Debug.LogError("No such a cheat");
            return;
        }

        var parameters = new object[1];
        parameters[0] = args;
        info.Invoke(null, parameters);
    }

    #endregion //CheatsInput
}
