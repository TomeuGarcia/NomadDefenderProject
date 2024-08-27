using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ICardTooltipSource;
using static TurretBuildingCard;

public class CardPartBase : CardPart, ICardTooltipSource
{
    [Header("CARD INFO")]
    [SerializeField] protected CanvasGroup[] cgsInfoHide;

    [Header("Base card info")]
    [SerializeField] private RectTransform defaultBaseIcon; // used as Hidden info


    [Header("CANVAS COMPONENTS")]
    [SerializeField] private Image basePassiveImage;
    [SerializeField] private CanvasGroup cgPassive;
    [SerializeField] private TMP_Text _basePassiveNameText;


    private ATurretPassiveAbility turretPassive;
    public ATurretPassiveAbilityDataModel TurretPassiveModel => turretPassive.OriginalModel;

    

    [Header("DESCRIPTION")]
    [SerializeField] private Transform leftDescriptionPosition;
    [SerializeField] private Transform rightDescriptionPosition;


    public void SetTurretPassive(ATurretPassiveAbilityDataModel turretPassiveModel)
    {
        turretPassive = turretPassiveModel.MakePassiveAbility();
    }


    public override void Init()
    {
        basePassiveImage.transform.parent.gameObject.SetActive(true);

        basePassiveImage.sprite = TurretPassiveModel.View.Sprite;
        basePassiveImage.color = TurretPassiveModel.View.Color;

        _basePassiveNameText.text = "/" + TurretPassiveModel.Name;
    }

    

    protected override void DoShowInfo()
    {
        CardTooltipDisplayManager.GetInstance().StartDisplayingTooltip(this);
    }

    public override void HideInfo()
    {
        base.HideInfo();
        CardTooltipDisplayManager.GetInstance().StopDisplayingTooltip();
    }

    public void PlayTutorialBlinkAnimation(float delayBeforeAbility)
    {
        StartCoroutine(TutorialBlinkAnimation(delayBeforeAbility));
    }
    private IEnumerator TutorialBlinkAnimation(float delayBeforeAbility)
    {
        float t1 = 0.1f;

        for (int i = 0; i < 8; ++i)
        {
            foreach (CanvasGroup cg in cgsInfoHide)
            {
                cg.DOFade(0f, t1);
            }            
            yield return new WaitForSeconds(t1);


            foreach (CanvasGroup cg in cgsInfoHide)
            {
                cg.DOFade(1f, t1);
            }
            GameAudioManager.GetInstance().PlayCardInfoShown();
            yield return new WaitForSeconds(t1);
        }

        yield return new WaitForSeconds(delayBeforeAbility);
        

        for (int i = 0; i < 8; ++i)
        {
            cgPassive.DOFade(0f, t1);
            yield return new WaitForSeconds(t1);

            cgPassive.DOFade(1f, t1);
            GameAudioManager.GetInstance().PlayCardInfoShown();
            yield return new WaitForSeconds(t1);
        }

    }



    // ICardTooltipSource OVERLOADS
    public CardTooltipDisplayData MakeTooltipDisplayData()
    {
        return CardTooltipDisplayData.MakeForCardPartPassive(_descriptionTooltipPositioning, TurretPassiveModel, 
            turretPassive.GetAbilityDescription());
    }
}
