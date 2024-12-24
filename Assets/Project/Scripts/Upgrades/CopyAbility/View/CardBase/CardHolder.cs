using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardHolder : MachineMovablePart
{
    [Header("REFERENCES")]
    [SerializeField] private Transform _cardCover;
    [SerializeField] private Transform _cardHolder;

    [Header("PARAMETERS")]
    [SerializeField] private TweenConfig _coverHideA;
    [SerializeField] private float _delay0;
    [SerializeField] private TweenConfig _coverHideB;
    [SerializeField] private float _delay1;
    [SerializeField] private TweenConfig _holderShow;

    private TweenConfig _coverHideAUndo;
    private TweenConfig _coverHideBUndo;
    private TweenConfig _holderShowUndo;

    public override void Init()
    {
        _coverHideAUndo = _coverHideA.Undo();
        _coverHideBUndo = _coverHideB.Undo();
        _holderShowUndo = _holderShow.Undo();
    }

    public override IEnumerator EnterAnimation()
    {
        _cardCover.DOBlendableLocalMoveBy(_coverHideA.Value, _coverHideA.Duration).SetEase(_coverHideA.Ease);
        yield return new WaitForSeconds(_coverHideA.Duration + _delay0);

        _cardCover.DOBlendableLocalMoveBy(_coverHideB.Value, _coverHideB.Duration).SetEase(_coverHideB.Ease);
        yield return new WaitForSeconds(_coverHideB.Duration + _delay1);

        _cardHolder.DOBlendableLocalMoveBy(_holderShow.Value, _holderShow.Duration).SetEase(_holderShow.Ease);
        yield return new WaitForSeconds(_holderShow.Duration);
    }

    public override IEnumerator ExitAnimation()
    {
        _cardHolder.DOBlendableLocalMoveBy(_holderShowUndo.Value, _holderShowUndo.Duration).SetEase(_holderShowUndo.Ease);
        yield return new WaitForSeconds(_holderShowUndo.Duration + _delay1);

        _cardCover.DOBlendableLocalMoveBy(_coverHideBUndo.Value, _coverHideBUndo.Duration).SetEase(_coverHideBUndo.Ease);
        yield return new WaitForSeconds(_coverHideBUndo.Duration + _delay0);

        _cardCover.DOBlendableLocalMoveBy(_coverHideAUndo.Value, _coverHideAUndo.Duration).SetEase(_coverHideAUndo.Ease);
        yield return new WaitForSeconds(_coverHideAUndo.Duration);
    }
}
