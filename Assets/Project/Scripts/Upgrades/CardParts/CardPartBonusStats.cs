using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPartBonusStats : CardPart, ICardTooltipSource
{
    [SerializeField] private CardPartBonusStatsItem _damageItem;
    [SerializeField] private CardPartBonusStatsItem _shotsPerSecondItem;
    [SerializeField] private CardPartBonusStatsItem _radiusRangeItem;
    [SerializeField] private CardPartBonusStatsItem _extraLevelsItem;
    [SerializeField] private CardPartBonusStatsItem _extraPlayCostItem;


    [Header("DESCRIPTION")]
    [SerializeField] private Transform leftDescriptionPosition;
    [SerializeField] private Transform rightDescriptionPosition;

    private TurretStatsUpgradeModel.StatString _damageStatString;
    private TurretStatsUpgradeModel.StatString _shotsPerSecondStatString;
    private TurretStatsUpgradeModel.StatString _radiusRangeStatString;
    private TurretStatsUpgradeModel.StatString _extraLevelsStatString;
    private TurretStatsUpgradeModel.StatString _extraPlayCostStatString;


    public TurretStatsMultiplicationSnapshot StatsSnapshotUpgrade { get; private set; }

    [SerializeField] private DescriptionHelpReferences _descriptionHelper;
    private EditableCardAbilityDescription _statsDescription;
    
    public int ExtraLevelsForCard { get; private set; }
    public int ExtraCardPlayCost { get; private set; }
    

    [System.Serializable]
    public class DescriptionHelpReferences
    {
        [SerializeField] private Sprite _upgradeSprite;
        [SerializeField] private CardStatViewConfig _damageViewConfig;
        [SerializeField] private CardStatViewConfig _shotsPerSecondViewConfig;
        [SerializeField] private CardStatViewConfig _radiusRangeViewConfig;
        [SerializeField] private CardStatViewConfig _extraLevelsViewConfig;
        [SerializeField] private CardStatViewConfig _extraPlayCostViewConfig;

        public Sprite UpgradeSprite => _upgradeSprite;
        public Color SpriteColor => Color.white;
        public string Name => "bonusStats";

        private string MakeStatString(TurretStatsUpgradeModel.StatString statString, CardStatViewConfig statsViewConfig,
            bool withSuffix = true)
        {            
            if (statString.IsNull)
            {
                return "";
            }

            string value = statString.Value;
            if (withSuffix) value += statsViewConfig.ValueSuffix;
            value += " <color=#" + ColorUtility.ToHtmlStringRGBA(statsViewConfig.IconColor) + ">" + statsViewConfig.Name + "</color>\n";
            
            return value;
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
        public string MakeExtraLevelsStatString(TurretStatsUpgradeModel.StatString extraLevelsStatString, bool withSuffix)
        {
            return MakeStatString(extraLevelsStatString, _extraLevelsViewConfig, withSuffix);
        }
        public string MakeExtraPlayCostStatString(TurretStatsUpgradeModel.StatString extraPlayCostStatString)
        {
            return MakeStatString(extraPlayCostStatString, _extraPlayCostViewConfig);
        }

        public string MakeStatsString(
            TurretStatsUpgradeModel.StatString damageStatString,
            TurretStatsUpgradeModel.StatString shotsPerSecondStatString,
            TurretStatsUpgradeModel.StatString radiusRangeStatString,
            TurretStatsUpgradeModel.StatString extraLevelsStatString,
            bool extraLevelNeedsSuffix,
            TurretStatsUpgradeModel.StatString extraPlayCostStatString
            )
        {
            return MakeDamageString(damageStatString) +
                   MakeShotsPerSecondString(shotsPerSecondStatString) +
                   MakeRadiusRangeString(radiusRangeStatString) +
                   MakeExtraPlayCostStatString(extraPlayCostStatString) +
                   MakeExtraLevelsStatString(extraLevelsStatString, extraLevelNeedsSuffix);
        }
    }


    public void Configure(TurretStatsUpgradeModel model)
    {
        StatsSnapshotUpgrade = model.MakeStatMultiplicationSnapshot();

        model.MakeStatStrings(
            out _damageStatString,
            out _shotsPerSecondStatString,
            out _radiusRangeStatString,
            out _extraLevelsStatString,
            out _extraPlayCostStatString
        );
        
        _damageItem.Init(_damageStatString, isDebuff: model.DamageMultiplier < 0);
        _shotsPerSecondItem.Init(_shotsPerSecondStatString, isDebuff: model.ShotsPerSecondMultiplier < 0);
        _radiusRangeItem.Init(_radiusRangeStatString, isDebuff: model.RadiusRangeMultiplier < 0);

        bool extraLevelNeedsSuffix = model.ExtraLevels < 0;
        if (extraLevelNeedsSuffix)
        {
            _extraLevelsItem.Init(_extraLevelsStatString, textSuffix:" Lvl UPG", isDebuff: true);
        }
        else
        {
            _extraLevelsItem.Init(_extraLevelsStatString, isDebuff: true);
        }
        _extraPlayCostItem.Init(_extraPlayCostStatString, textSuffix:"<size=150%>♦", isDebuff: true);
        
        _statsDescription = new EditableCardAbilityDescription(
                _descriptionHelper.Name, 
                _descriptionHelper.MakeStatsString(_damageStatString, _shotsPerSecondStatString, _radiusRangeStatString,
                    _extraLevelsStatString, extraLevelNeedsSuffix, _extraPlayCostStatString),
                Array.Empty<CardAbilityKeyword>()
            );

        ExtraLevelsForCard = model.ExtraLevels;
        ExtraCardPlayCost = model.ExtraPlayCost;
    }

    public override void Init()
    {
        //attackImage.sprite = turretPartAttack.abilitySprite;
        //attackImage.color = turretPartAttack.materialColor;
    }

    public void ApplyStatsModification(ITurretStatsBonusController turretCardStatsController)
    {
        turretCardStatsController.AddBonusBaseStatsMultiplication(StatsSnapshotUpgrade);
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
        return CardTooltipDisplayData.MakeForCardPartStatsUpgrade(_descriptionTooltipPositioning, _descriptionHelper, 
            _statsDescription);
    }

    public bool WithKeywords()
    {
        return true;
    }
}
