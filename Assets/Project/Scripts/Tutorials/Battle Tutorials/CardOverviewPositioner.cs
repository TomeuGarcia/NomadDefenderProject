using System.Collections;
using DG.Tweening;
using UnityEngine;



public class CardOverviewPositioner : MonoBehaviour
{
    [SerializeField] private Transform _overviewSpot;
    [SerializeField] private float _positionScale = 1.2f;
    [SerializeField] private float _positionInDuration = 0.5f;
    [SerializeField] private float _positionOutDuration = 0.75f;
    
    private BuildingCard _card;
    private Vector3 _originPosition;

    public void Init(BuildingCard card)
    {
        _card = card;
        _originPosition = _overviewSpot.position;
    }
    
    public IEnumerator PositionToSpot()
    {
        _card.transform.DOComplete();
        _card.transform.DOMove(_overviewSpot.position, _positionInDuration).SetEase(Ease.InOutSine);
        _card.transform.DOScale( Vector3.one * _positionScale, _positionInDuration).SetEase(Ease.InOutSine);
        _card.transform.DOLocalRotateQuaternion(Quaternion.identity, _positionInDuration).SetEase(Ease.InOutSine);

        _card.DisableMouseInteraction();
        _card.MotionEffectsController.DisableMotion();
        _card.isInteractable = false;

        yield return new WaitForSeconds(_positionInDuration);
    }
    
    public IEnumerator UndoPositioning()
    {
        _card.transform.DOComplete();
        _card.transform.DOMove(_originPosition, _positionOutDuration)
            .SetEase(Ease.InOutSine);
        _card.transform.DOScale(Vector3.one, _positionOutDuration)
            .SetEase(Ease.InOutSine);

        /*
        _card.MotionEffectsController.RotationEffectsTransform.DOBlendableLocalRotateBy(new Vector3(0, 360f, 0), _positionOutDuration,
            RotateMode.LocalAxisAdd);
        */

        yield return new WaitForSeconds(_positionOutDuration + 0.25f);

        _card.EnableMouseInteraction();
        _card.MotionEffectsController.EnableMotion();
        _card.isInteractable = true;
    }
}
