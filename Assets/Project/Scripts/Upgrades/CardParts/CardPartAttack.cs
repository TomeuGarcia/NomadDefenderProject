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
    [SerializeField] private RectTransform defaultAttackIcon; // used as Hidden info



    [Header("PART")]
    [SerializeField] private TurretPartAttack turretPartAttack;
    public TurretPartAttack TurretPartAttack => turretPartAttack;


    [Header("VISUALS")]
    //[SerializeField] private MeshRenderer attackMeshRenderer;
    [SerializeField] private Image attackImage;


    [Header("DESCRIPTION")]
    [SerializeField] private Transform leftDescriptionPosition;
    [SerializeField] private Transform rightDescriptionPosition;


    protected override void AwakeInit()
    {
        base.AwakeInit();
    }

    public void Configure(TurretPartAttack turretPartAttack)
    {
        this.turretPartAttack = turretPartAttack;
    }

    public override void Init()
    {
        attackImage.sprite = turretPartAttack.abilitySprite;
        attackImage.color = turretPartAttack.materialColor;
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
