using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ICardDescriptionProvider;
using static TurretBuildingCard;

public class CardPartBase : CardPart, ICardDescriptionProvider
{
    [Header("CARD INFO")]
    [SerializeField] protected CanvasGroup[] cgsInfoHide;

    [Header("Base card info")]
    [SerializeField] private RectTransform defaultBaseIcon; // used as Hidden info


    [Header("CANVAS COMPONENTS")]
    [SerializeField] private Image basePassiveImage;
    [SerializeField] private CanvasGroup cgPassive;
    [SerializeField] private TMP_Text _basePassiveNameText;


    [Header("PART")]
    [SerializeField] public TurretPassiveBase turretPassiveBase;



    private bool hasBasePassiveAbility;


    [Header("DESCRIPTION")]
    [SerializeField] private Transform leftDescriptionPosition;
    [SerializeField] private Transform rightDescriptionPosition;





    public override void Init()
    {
        hasBasePassiveAbility = turretPassiveBase.passive.GetType() != typeof(BaseNullPassive);
        if (hasBasePassiveAbility)
        {
            basePassiveImage.transform.parent.gameObject.SetActive(true);

            basePassiveImage.sprite = turretPassiveBase.visualInformation.sprite;
            basePassiveImage.color = turretPassiveBase.visualInformation.color;

            _basePassiveNameText.text = "/" + turretPassiveBase.passive.abilityName;
        }
        else
        {
            basePassiveImage.transform.parent.gameObject.SetActive(false);
        }        
    }


    protected override void DoShowInfo()
    {
        CardDescriptionDisplayer.GetInstance().ShowCardDescription(this);
    }

    public override void HideInfo()
    {
        base.HideInfo();
        CardDescriptionDisplayer.GetInstance().HideCardDescription();
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



    // ICardDescriptionProvider OVERLOADS
    public ICardDescriptionProvider.SetupData[] GetAbilityDescriptionSetupData()
    {
        ICardDescriptionProvider.SetupData[] setupData = new ICardDescriptionProvider.SetupData[2];
        
        setupData[0] = null;

        if (hasBasePassiveAbility)
        {
            setupData[1] = new ICardDescriptionProvider.SetupData(
                turretPassiveBase.passive.abilityName,
                turretPassiveBase.passive.abilityDescription,
                turretPassiveBase.visualInformation.sprite,
                turretPassiveBase.visualInformation.color
            );
        }
        else
        {
            setupData[1] = new ICardDescriptionProvider.SetupData();
        }
                    

        return setupData;
    }

    public Vector3 GetCenterPosition()
    {
        return CardTransform.position + CardTransform.TransformDirection(Vector3.down * 0.2f);
    }

    public DescriptionCornerPositions GetCornerPositions()
    {
        return new DescriptionCornerPositions(leftDescriptionPosition.position, rightDescriptionPosition.position);
    }

}
