using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerNotifier : MonoBehaviour
{
    public delegate void TriggerNotifierAction(Collider other);
    public event TriggerNotifierAction OnEnter;
    public event TriggerNotifierAction OnStay;
    public event TriggerNotifierAction OnExit;


    private void OnTriggerEnter(Collider other)
    {
        if (OnEnter != null) OnEnter(other);
    }

    private void OnTriggerStay(Collider other)
    {
        if (OnStay != null) OnStay(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (OnExit != null) OnExit(other);
    }
}
