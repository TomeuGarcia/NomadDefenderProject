using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class OptionalTutorial_UpgradeRoom_CardLevelMeaning : MonoBehaviour
{
    [SerializeField] private TutorialCardOverviewAddOn _cardOverview;
    [SerializeField] private CardOverviewPositioner _cardOverviewPositioner;
    [SerializeField] private SpriteRenderer _blackBackgroundSprite;
    private float _blackBackgroundSpriteAlpha;

    private void Awake()
    {
        _blackBackgroundSpriteAlpha = _blackBackgroundSprite.color.a;
        _blackBackgroundSprite.DOFade(0, 0);
    }

    public IEnumerator Play(BuildingCard card)
    {
        _cardOverviewPositioner.Init(card);
        yield return StartCoroutine(_cardOverviewPositioner.PositionToSpot());
        _blackBackgroundSprite.DOFade(_blackBackgroundSpriteAlpha, 0.3f);

        Transform cardOverviewTransform = _cardOverview.transform;
        cardOverviewTransform.SetParent(card.MotionEffectsController.RotationEffectsTransform);
        cardOverviewTransform.localPosition = Vector3.zero;
        cardOverviewTransform.localRotation = Quaternion.identity;
        cardOverviewTransform.localScale = Vector3.one;
        
        yield return StartCoroutine(_cardOverview.Play());
        
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Mouse0));
        _cardOverview.Finish();
        
        _blackBackgroundSprite.DOFade(0, 0.3f);
        yield return StartCoroutine(_cardOverviewPositioner.UndoPositioning());
        
        Destroy(_cardOverview.gameObject);
    }
}