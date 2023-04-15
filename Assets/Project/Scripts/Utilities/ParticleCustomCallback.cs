using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ParticleCustomCallback : MonoBehaviour
{
    [SerializeField] private bool deactivate;

    [Header("CUSTOM CALLS")]
    [SerializeField] UnityEvent m_MyEvent;

    public void OnParticleSystemStopped()
    {
        m_MyEvent.Invoke();

        if (deactivate) gameObject.SetActive(false);
    }
}
