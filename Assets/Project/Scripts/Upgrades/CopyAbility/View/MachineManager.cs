using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineManager : MonoBehaviour
{
    [SerializeField] private CardSlotTube _rightTube;
    [SerializeField] private CardSlotTube _leftTube;

    [Header("TWEENS")]
    [SerializeField] private float _startDelay;

    void Start()
    {
        StartCoroutine(EnterAnimation());
    }

    private IEnumerator EnterAnimation()
    {
        yield return new WaitForSeconds(_startDelay);
        StartCoroutine(_rightTube.EnterAnimation());
    }
}
