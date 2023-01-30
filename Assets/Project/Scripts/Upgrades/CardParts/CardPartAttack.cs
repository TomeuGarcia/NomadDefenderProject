using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static TurretBuildingCard;

public class CardPartAttack : CardPart
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
    private Material attackMaterial;



    private void Awake()
    {
        //attackMaterial = attackMeshRenderer.material;
        attackMaterial = new Material(attackImage.material);
        attackImage.material = attackMaterial;

        SetupCardInfo();
    }

    public override void Init()
    {
        attackMaterial.SetTexture("_Texture", turretPartAttack.materialTexture);
        attackMaterial.SetColor("_Color", turretPartAttack.materialColor);

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

        showInfoCoroutine = StartCoroutine(ShowInfoAnimation());
    }

    public override void HideInfo()
    {
        if (isHideInfoAnimationPlaying) return;

        base.HideInfo();

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
        yield return new WaitForSeconds(t2);

        // show attack text
        attackNameText.DOFade(1f, t2);
        yield return new WaitForSeconds(t2);
        attackDescriptionText.DOFade(1f, t2);
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
        yield return new WaitForSeconds(t2);
        attackNameText.DOFade(0f, t2);
        yield return new WaitForSeconds(t2);

        // hide attack icon
        defaultAttackIcon.DOLocalMove(infoHiddenAttackIconPos, t2);
        yield return new WaitForSeconds(t2);


        canInfoInteract = true;
    }

}
