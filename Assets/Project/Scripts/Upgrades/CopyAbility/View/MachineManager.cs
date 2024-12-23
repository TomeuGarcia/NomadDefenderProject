using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineManager : MonoBehaviour
{
    [SerializeField] private CardSlotTube _rightTube;
    [SerializeField] private CardSlotTube _leftTube;
    [SerializeField] private MachineDisplay _machineDisplay;

    [Header("TWEENS")]
    [SerializeField] private float _startDelay;
    [SerializeField] private float _tubeDelay;

    void Start()
    {
        StartCoroutine(EnterAnimation());
    }

    private IEnumerator EnterAnimation()
    {
        yield return new WaitForSeconds(_startDelay);

        StartCoroutine(_machineDisplay.EnterAnimation());
        yield return new WaitForSeconds(_tubeDelay);

        StartCoroutine(_rightTube.EnterAnimation());
    }
}
