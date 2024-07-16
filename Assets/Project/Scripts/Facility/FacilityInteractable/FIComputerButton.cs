using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FIComputerButton : AFacilityInteractable
{
    [SerializeField] private Transform _screenParent;

    private bool _on = false;

    protected override IEnumerator DoInteract()
    {
        if (_on) yield return TurnOff();
        else yield return TurnOn();
    }
    private IEnumerator TurnOn()
    {
        _screenParent.gameObject.SetActive(true);

        yield return _duration;
    }

    private IEnumerator TurnOff()
    {
        _screenParent.gameObject.SetActive(false);

        yield return _duration;
    }
}
