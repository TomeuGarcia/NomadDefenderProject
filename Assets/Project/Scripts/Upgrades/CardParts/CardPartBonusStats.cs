using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPartBonusStats : CardPart, ICardTooltipSource
{
    [SerializeField] private CardPartBonusStatsItem _damageItem;
    [SerializeField] private CardPartBonusStatsItem _shotsPerSecondItem;
    [SerializeField] private CardPartBonusStatsItem _radiusRangeItem;


    [Header("DESCRIPTION")]
    [SerializeField] private Transform leftDescriptionPosition;
    [SerializeField] private Transform rightDescriptionPosition;

    private TurretStatsUpgradeModel.StatString _damageStatString;
    private TurretStatsUpgradeModel.StatString _shotsPerSecondStatString;
    private TurretStatsUpgradeModel.StatString _radiusRangeStatString;


    private TurretStatsMultiplicationSnapshot _statsSnapshotUpgrade;

    [SerializeField] private DescriptionHelpReferences _descriptionHelper;
    private EditableCardAbilityDescription _statsDescription;
    

    [System.Serializable]
    public class DescriptionHelpReferences
    {
        [SerializeField] private Sprite _upgradeSprite;
        [SerializeField] private CardStatViewConfig _damageViewConfig;
        [SerializeField] private CardStatViewConfig _shotsPerSecondViewConfig;
        [SerializeField] private CardStatViewConfig _radiusRangeViewConfig;

        public Sprite UpgradeSprite => _upgradeSprite;
        public Color SpriteColor => Color.white;
        public string Name => "_bonusStats";

        private string MakeStatString(TurretStatsUpgradeModel.StatString statString, CardStatViewConfig statsViewConfig)
        {            
            if (statString.IsNull)
            {
                return "";
            }

            return
                statString.Value +
                " <color=#" + ColorUtility.ToHtmlStringRGBA(statsViewConfig.IconColor) + ">" + statsViewConfig.Name + "</color>\n";
        }
        
        public string MakeDamageString(TurretStatsUpgradeModel.StatString damageStatString)
        {
            return MakeStatString(damageStatString, _damageViewConfig);
        }        
        public string MakeShotsPerSecondString(TurretStatsUpgradeModel.StatString shotsPerSecondStatString)
        {
            return MakeStatString(shotsPerSecondStatString, _shotsPerSecondViewConfig);
        }        
        public string MakeRadiusRangeString(TurretStatsUpgradeModel.StatString radiusRangeStatString)
        {
            return MakeStatString(radiusRangeStatString, _radiusRangeViewConfig);
        }

        public string MakeStatsString(TurretStatsUpgradeModel.StatString damageStatString,
            TurretStatsUpgradeModel.StatString shotsPerSecondStatString,
            TurretStatsUpgradeModel.StatString radiusRangeStatString)
        {
            return MakeDamageString(damageStatString) +
                   MakeShotsPerSecondString(shotsPerSecondStatString) +
                   MakeRadiusRangeString(radiusRangeStatString);
        }
    }


    public void Configure(TurretStatsUpgradeModel model)
    {
        _statsSnapshotUpgrade = model.MakeStatMultiplicationSnapshot();

        model.MakeStatStrings(
            out _damageStatString,
            out _shotsPerSecondStatString,
            out _radiusRangeStatString
        );
        _damageItem.Init(_damageStatString);
        _shotsPerSecondItem.Init(_shotsPerSecondStatString);
        _radiusRangeItem.Init(_radiusRangeStatString);
        
        _statsDescription = new EditableCardAbilityDescription(
                _descriptionHelper.Name, 
                _descriptionHelper.MakeStatsString(_damageStatString, _shotsPerSecondStatString, _radiusRangeStatString),
                Array.Empty<CardAbilityKeyword>()
            );
    }

    public override void Init()
    {
        //attackImage.sprite = turretPartAttack.abilitySprite;
        //attackImage.color = turretPartAttack.materialColor;
    }

    public void ApplyStatsModification(ITurretStatsBonusController turretCardStatsController)
    {
        turretCardStatsController.AddBonusBaseStatsMultiplication(_statsSnapshotUpgrade);
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

    public ICardTooltipSource.SetupData[] GetAbilityDescriptionSetupData()
    {
        ICardTooltipSource.SetupData[] setupData = new ICardTooltipSource.SetupData[2];

        setupData[0] = new ICardTooltipSource.SetupData(
            "bonusStats",
            _descriptionHelper.MakeStatsString(_damageStatString, _shotsPerSecondStatString, _radiusRangeStatString),
            _descriptionHelper.UpgradeSprite,
            Color.white
        );

        setupData[1] = null;

        return setupData;
    }

    public Vector3 GetCenterPosition()
    {
        return CardTransform.position + CardTransform.TransformDirection(Vector3.down * 0.2f);
    }

    public ICardTooltipSource.DescriptionCornerPositions GetCornerPositions()
    {
        return new ICardTooltipSource.DescriptionCornerPositions(leftDescriptionPosition.position, rightDescriptionPosition.position);
    }
    
    public CardTooltipDisplayData MakeTooltipDisplayData()
    {
        return new CardTooltipDisplayData(_descriptionTooltipPositioning, _descriptionHelper, _statsDescription);
    }
}
