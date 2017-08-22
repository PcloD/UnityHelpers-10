using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputExpropriation
{
    public InputExpropriator.InputType GetInputType()
    {
        return m_InputType;
    }

    public int GetLevel()
    {
        return m_Level;
    }

    internal InputExpropriation(InputExpropriator.InputType inputType, int level)
    {
        m_InputType = inputType;
        m_Level = level;
    }

    public bool IsActive()
    {
        return m_Active;
    }

    internal void SetActive(bool active)
    {
        m_Active = active;
    }

    private InputExpropriator.InputType m_InputType;
    private int m_Level;
    private bool m_Active = false;
}

public class InputExpropriator
{
    public enum InputType
    {
        MouseMovement,
        MouseButtons,
        MovementKeys,
        JumpKeys
    }

    public static InputExpropriation CreateInputExpropriation(InputType inputType, int level)
    {
        InputExpropriation result = new InputExpropriation(inputType, level);

        if (!s_InputExpropriations.ContainsKey(inputType))
            s_InputExpropriations.Add(inputType, new List<InputExpropriation>());

        List<InputExpropriation> inputs = s_InputExpropriations[inputType];
        inputs.Add(result);

        RefreshInputExpropriations();

        return result;
    }

    public static void ReleaseInputExpropriation(InputExpropriation inputExpropriation)
    {
        if (null == inputExpropriation)
            return;

        var inputType = inputExpropriation.GetInputType();
        if (!s_InputExpropriations.ContainsKey(inputType))
            return;

        var inputsList = s_InputExpropriations[inputType];
        inputsList.Remove(inputExpropriation);

        if (inputsList.Count == 0)
            s_InputExpropriations.Remove(inputType);

        RefreshInputExpropriations();
    }

    private static void RefreshInputExpropriations()
    {
        foreach(InputType inputType in Enum.GetValues(typeof(InputType)))
        {
            if (!s_InputExpropriations.ContainsKey(inputType))
                continue;

            var inputsList = s_InputExpropriations[inputType];

            int maxLevel = 0;
            foreach (var inputExpropriation in inputsList)
                if (inputExpropriation.GetLevel() > maxLevel)
                    maxLevel = inputExpropriation.GetLevel();

            foreach (var inputExpropriation in inputsList)
                inputExpropriation.SetActive(inputExpropriation.GetLevel() == maxLevel);
        }
    }

    private static Dictionary<InputType, List<InputExpropriation>> s_InputExpropriations = new Dictionary<InputType, List<InputExpropriation>>();
}
