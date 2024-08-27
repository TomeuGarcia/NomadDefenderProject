using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ICardTooltipSource;
using static TurretBuildingCard;

public class CardPartAttack : CardPart, ICardTooltipSource
{
    [Header("CARD INFO")]
    //[SerializeField] protected CanvasGroup[] cgsInfoHide;

    [Header("Attack card info")]
    [SerializeField] private RectTransform defaultAttackIcon; // used as Hidden info



    [Header("PART")]
    [SerializeField] private TurretPartProjectileDataModel turretPartAttack;
    public TurretPartProjectileDataModel TurretPartAttack => turretPartAttack;


    [Header("VISUALS")]
    //[SerializeField] private MeshRenderer attackMeshRenderer;
    [SerializeField] private Image attackImage;
    [SerializeField] private TMP_Text _attackNameText;


    [Header("DESCRIPTION")]
    [SerializeField] private Transform leftDescriptionPosition;
    [SerializeField] private Transform rightDescriptionPosition;


    private EditableCardAbilityDescription _projectileDescription;

    public void Configure(TurretPartProjectileDataModel turretPartAttack)
    {
        this.turretPartAttack = turretPartAttack;
    }

    public override void Init()
    {
        _projectileDescription = turretPartAttack.MakeAbilityDescription();
        
        attackImage.sprite = turretPartAttack.abilitySprite;
        attackImage.color = turretPartAttack.materialColor;
        _attackNameText.text = _projectileDescription.NameForDisplay;
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


    // ICardTooltipSource OVERLOADS
    public CardTooltipDisplayData MakeTooltipDisplayData()
    {
        return CardTooltipDisplayData.MakeForProjectileCardPart(_descriptionTooltipPositioning, turretPartAttack, _projectileDescription);
    }
}
