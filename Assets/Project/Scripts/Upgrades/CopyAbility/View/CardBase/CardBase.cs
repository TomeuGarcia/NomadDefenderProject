using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CardBase : MachineMovablePart
{
    [Header("REFERENCES")]
    [SerializeField] private CardHolder _cardHolder;
    [SerializeField] private CardBaseMisc _cardBaseMisc;

    public override void Init()
    {

    }

    public override IEnumerator EnterAnimation()
    {
        yield return null;
        StartCoroutine(_cardHolder.EnterAnimation());
        StartCoroutine(_cardBaseMisc.EnterAnimation());
    }

    public override IEnumerator ExitAnimation()
    {
        yield return null;
    }
}
