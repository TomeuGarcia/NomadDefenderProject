using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ICardDescriptionProvider;
using static TurretBuildingCard;

public class CardPartAttack : CardPart, ICardDescriptionProvider
{
    [Header("CARD INFO")]
    //[SerializeField] protected CanvasGroup[] cgsInfoHide;

    [Header("Attack card info")]
    [SerializeField] private RectTransform infoShownAttackIcon;
    [SerializeField] private RectTransform defaultAttackIcon; // used as Hidden info
    private Vector3 infoShownAttackIconPos;
    private Vector3 infoHiddenAttackIconPos;
    [SerializeField] private TextMeshProUGUI attackNameText;
    [SerializeField] private TextMeshProUGUI attackDescriptionText;



    [Header("PART")]
    [SerializeField] public TurretPartAttack turretPartAttack;


    [Header("VISUALS")]
    //[SerializeField] private MeshRenderer attackMeshRenderer;
    [SerializeField] private Image attackImage;


    [Header("DESCRIPTION")]
    [SerializeField] private Transform leftDescriptionPosition;
    [SerializeField] private Transform rightDescriptionPosition;


    protected override void AwakeInit()
    {
        base.AwakeInit();

        SetupCardInfo();
    }

    public override void Init()
    {
        attackImage.sprite = turretPartAttack.abilitySprite;
        attackImage.color = turretPartAttack.materialColor;

        InitInfoVisuals();
    }


    protected override void InitInfoVisuals()
    {
        attackNameText.text = '/' + turretPartAttack.abilityName;
        attackDescriptionText.text = turretPartAttack.abilityDescription;
    }
    protected override void SetupCardInfo()
    {
        // general
        infoInterface.SetActive(true);
        isShowInfoAnimationPlaying = false;

        // attack
        infoShownAttackIconPos = infoShownAttackIcon.localPosition;
        infoHiddenAttackIconPos = defaultAttackIcon.localPosition;
        attackNameText.alpha = 0;
        attackDescriptionText.alpha = 0;
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


        float t2 = 0.1f;

        // show attack icon
        defaultAttackIcon.DOLocalMove(infoShownAttackIconPos, t2);
        GameAudioManager.GetInstance().PlayCardInfoMoveShown();
        yield return new WaitForSeconds(t2);

        // show attack text
        attackNameText.DOFade(1f, t2);
        GameAudioManager.GetInstance().PlayCardInfoShown();
        yield return new WaitForSeconds(t2);
        attackDescriptionText.DOFade(1f, t2);
        GameAudioManager.GetInstance().PlayCardInfoShown();
        yield return new WaitForSeconds(t2);


        canInfoInteract = true;
        isShowInfoAnimationPlaying = false;
    }

    private IEnumerator HideInfoAnimation()
    {
        canInfoInteract = false;


        float t2 = 0.1f;

        // hide attack text
        attackDescriptionText.DOFade(0f, t2);
        GameAudioManager.GetInstance().PlayCardInfoHidden();
        yield return new WaitForSeconds(t2);
        attackNameText.DOFade(0f, t2);
        GameAudioManager.GetInstance().PlayCardInfoHidden();
        yield return new WaitForSeconds(t2);

        // hide attack icon
        defaultAttackIcon.DOLocalMove(infoHiddenAttackIconPos, t2);
        GameAudioManager.GetInstance().PlayCardInfoMoveHidden();
        yield return new WaitForSeconds(t2);


        canInfoInteract = true;
    }



    // ICardDescriptionProvider OVERLOADS
    public ICardDescriptionProvider.SetupData[] GetAbilityDescriptionSetupData()
    {
        ICardDescriptionProvider.SetupData[] setupData = new ICardDescriptionProvider.SetupData[2];

        setupData[0] = new ICardDescriptionProvider.SetupData(
            turretPartAttack.abilityName,
            turretPartAttack.abilityDescription,
            turretPartAttack.abilitySprite,
            turretPartAttack.materialColor
        );

        setupData[1] = null;

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
