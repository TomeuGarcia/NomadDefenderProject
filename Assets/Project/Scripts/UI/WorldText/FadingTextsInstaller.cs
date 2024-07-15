using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadingTextsInstaller : MonoBehaviour
{
    [SerializeField] private FadingTextsFactory _fadingTextFactory;

    void Start()
    {
        ServiceLocator.GetInstance().FadingTextFactory = _fadingTextFactory;
        _fadingTextFactory.transform.parent = ServiceLocator.GetInstance().transform;
    }

}
