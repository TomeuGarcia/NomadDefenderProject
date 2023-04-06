using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static TurretBuildingCard;

public class CardPartBase : CardPart, ICardDescriptionProvider
{
    [Header("CARD INFO")]
    [SerializeField] protected CanvasGroup[] cgsInfoHide;

    [Header("Base card info")]
    [SerializeField] private RectTransform infoShownBaseIcon;
    [SerializeField] private RectTransform defaultBaseIcon; // used as Hidden info
    private Vector3 infoShownBaseIconPos;
    private Vector3 infoHiddenBaseIconPos;
    [SerializeField] private TextMeshProUGUI baseNameText;
    [SerializeField] private TextMeshProUGUI baseDescriptionText;


    [Header("CANVAS COMPONENTS")]
    [SerializeField] private Image rangeFillImage;
    [SerializeField] private Image basePassiveImage;
    [SerializeField] private CanvasGroup cgRange;
    [SerializeField] private CanvasGroup cgPassive;

    [Header("PART")]
    [SerializeField] public TurretPartBase turretPartBase;
    [SerializeField] public TurretPassiveBase turretPassiveBase;

    [Header("VISUALS")]
    //[SerializeField] private MeshRenderer baseMeshRenderer;
    [SerializeField] private Image baseImage;
    private Material baseMaterial;


    private bool hasBasePassiveAbility;



    protected override void AwakeInit()
    {
        base.AwakeInit();
        //baseMaterial = baseMeshRenderer.material;
        baseMaterial = new Material(baseImage.material);
        baseImage.material = baseMaterial;

        SetupCardInfo();
    }

    public override void Init()
    {
        baseMaterial.SetTexture("_Texture", turretPartBase.materialTexture);
        baseMaterial.SetColor("_Color", turretPartBase.materialColor);

        rangeFillImage.fillAmount = turretPartBase.GetRangePer1();


        hasBasePassiveAbility = turretPassiveBase.passive.GetType() != typeof(BaseNullPassive);
        if (hasBasePassiveAbility)
        {
            basePassiveImage.transform.parent.gameObject.SetActive(true);

            basePassiveImage.sprite = turretPassiveBase.visualInformation.sprite;
            basePassiveImage.color = turretPassiveBase.visualInformation.color;
        }
        else
        {
            basePassiveImage.transform.parent.gameObject.SetActive(false);
        }

        InitInfoVisuals();
    }


    protected override void InitInfoVisuals()
    {
        baseNameText.text = '/' + turretPassiveBase.passive.abilityName;
        baseDescriptionText.text = turretPassiveBase.passive.abilityDescription;
    }
    protected override void SetupCardInfo()
    {
        // general
        infoInterface.SetActive(true);
        isShowInfoAnimationPlaying = false;

        // base
        infoShownBaseIconPos = infoShownBaseIcon.localPosition;
        infoHiddenBaseIconPos = defaultBaseIcon.localPosition;
        baseNameText.alpha = 0;
        baseDescriptionText.alpha = 0;
    }

    public override void ShowInfo()
    {
        base.ShowInfo();
        CardDescriptionDisplayer.GetInstance().ShowCardDescription(this);
        return;


        showInfoCoroutine = StartCoroutine(ShowInfoAnimation());
    }

    public override void HideInfo()
    {
        base.HideInfo();
        CardDescriptionDisplayer.GetInstance().HideCardDescription();
        return;

        if (isHideInfoAnimationPlaying) return;


        if (isShowInfoAnimationPlaying)
        {
            StopCoroutine(showInfoCoroutine);
        }

        StartCoroutine(HideInfoAnimation());
    }

    private IEnumerator ShowInfoAnimation()
    {
        canInfoInteract = false;
        isShowInfoAnimationPlaying = true;

        // hide generics
        float t = 0.05f;
        for (int i = 0; i < cgsInfoHide.Length; ++i)
        {
            cgsInfoHide[i].DOFade(0f, t);
            GameAudioManager.GetInstance().PlayCardInfoShown();
            yield return new WaitForSeconds(t);
        }


        float t2 = 0.1f;

        // show base icon
        if (hasBasePassiveAbility)
        {
            defaultBaseIcon.DOLocalMove(infoShownBaseIconPos, t2);
            GameAudioManager.GetInstance().PlayCardInfoMoveShown();
            yield return new WaitForSeconds(t2);
        }

        // show base text
        baseNameText.DOFade(1f, t2);
        GameAudioManager.GetInstance().PlayCardInfoShown();
        yield return new WaitForSeconds(t2);
        baseDescriptionText.DOFade(1f, t2);
        GameAudioManager.GetInstance().PlayCardInfoShown();
        yield return new WaitForSeconds(t2);


        canInfoInteract = true;
        isShowInfoAnimationPlaying = false;
    }

    private IEnumerator HideInfoAnimation()
    {
        canInfoInteract = false;


        float t2 = 0.1f;

        // hide base text
        baseDescriptionText.DOFade(0f, t2);
        GameAudioManager.GetInstance().PlayCardInfoHidden();
        yield return new WaitForSeconds(t2);
        baseNameText.DOFade(0f, t2);
        GameAudioManager.GetInstance().PlayCardInfoHidden();
        yield return new WaitForSeconds(t2);


        // hide base icon
        if (hasBasePassiveAbility)
        {
            defaultBaseIcon.DOLocalMove(infoHiddenBaseIconPos, t2);
            GameAudioManager.GetInstance().PlayCardInfoMoveHidden();
            yield return new WaitForSeconds(t2);
        }


        // show generics
        float t = 0.05f;

        for (int i = cgsInfoHide.Length - 1; i >= 0; --i)
        {
            cgsInfoHide[i].DOFade(1f, t);
            GameAudioManager.GetInstance().PlayCardInfoHidden();
            yield return new WaitForSeconds(t);
        }


        canInfoInteract = true;
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
            cgRange.DOFade(0f, t1);
            yield return new WaitForSeconds(t1);

            cgRange.DOFade(1f, t1);
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
        return CardTransform.position;
    }

}
