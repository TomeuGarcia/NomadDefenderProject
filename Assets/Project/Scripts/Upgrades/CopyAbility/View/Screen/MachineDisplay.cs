using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class MachineDisplay : MachineMovablePart
{
    [Header("HEADER")]
    [SerializeField] private MachineDisplayMovement _machineMovement;
    [SerializeField] private MachineDisplayScreen _machineScreen;

    [Header("PARAMETERS")]
    [SerializeField] private float _screenActivationDelay;

    public override void Init()
    {

    }

    public override IEnumerator EnterAnimation()
    {
        StartCoroutine(_machineMovement.EnterAnimation());
        yield return new WaitForSeconds(_screenActivationDelay);

        StartCoroutine(_machineScreen.EnterAnimation());
    }

    public override IEnumerator ExitAnimation()
    {
        yield return null;
    }

    public void Warning()
    {
        _machineScreen.Warning();
        GameAudioManager.GetInstance().PlayScreenShut();
    }

    public void Ready()
    {
        _machineScreen.Ready();
        GameAudioManager.GetInstance().PlayScreenOpen();
    }

    public void Replace()
    {
        _machineScreen.Replace();
        GameAudioManager.GetInstance().PlayScreenShut();
    }
}
