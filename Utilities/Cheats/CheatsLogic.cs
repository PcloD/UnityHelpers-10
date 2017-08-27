using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatsLogic
{
    public static void pause(string args)
    {
        Time.timeScale = 0.01f;
    }

    public static void normal_time(string args)
    {
        Time.timeScale = 1.0f;
    }

    public static void fast_time(string args)
    {
        Time.timeScale = 5.0f;
    }
}
