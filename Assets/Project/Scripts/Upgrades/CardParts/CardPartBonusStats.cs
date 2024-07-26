using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPartBonusStats : CardPart, ICardDescriptionProvider
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

    [System.Serializable]
    public class DescriptionHelpReferences
    {
        [SerializeField] private Sprite _upgradeSprite;
        [SerializeField] private CardStatViewConfig _damageViewConfig;
        [SerializeField] private CardStatViewConfig _shotsPerSecondViewConfig;
        [SerializeField] private CardStatViewConfig _radiusRangeViewConfig;

        public Sprite UpgradeSprite => _upgradeSprite;

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
        CardDescriptionDisplayer.GetInstance().ShowCardDescription(this);
    }

    public override void HideInfo()
    {
        base.HideInfo();
        CardDescriptionDisplayer.GetInstance().HideCardDescription();
    }

    public ICardDescriptionProvider.SetupData[] GetAbilityDescriptionSetupData()
    {
        ICardDescriptionProvider.SetupData[] setupData = new ICardDescriptionProvider.SetupData[2];

        string damageDescription = _descriptionHelper.MakeDamageString(_damageStatString);
        string shotsPerSecondDescription = _descriptionHelper.MakeShotsPerSecondString(_shotsPerSecondStatString);
        string radiusRangeDescription = _descriptionHelper.MakeRadiusRangeString(_radiusRangeStatString);

        setupData[0] = new ICardDescriptionProvider.SetupData(
            "bonusStats",
            damageDescription + shotsPerSecondDescription + radiusRangeDescription,
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

    public ICardDescriptionProvider.DescriptionCornerPositions GetCornerPositions()
    {
        return new ICardDescriptionProvider.DescriptionCornerPositions(leftDescriptionPosition.position, rightDescriptionPosition.position);
    }
}
