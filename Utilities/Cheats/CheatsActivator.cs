using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatsActivator : MonoBehaviour
{
    public GameObject CheatsControllerPrefab;

    void Start()
    {
        if (null == CheatsController.Instance)
            Instantiate(CheatsControllerPrefab);
    }
}
