using System.Collections.Generic;
using UnityEngine;

public class CardTooltipDisplayData
{
    public class Element
    {
        public readonly EditableCardAbilityDescription AbilityDescription;
        public readonly Sprite AbilitySprite;
        public readonly Color AbilityColor;
        public readonly bool FixedTooltipWidth;

        public Element(EditableCardAbilityDescription abilityDescription, Sprite abilitySprite, Color abilityColor, bool 
            fixedTooltipWidth = true)
        {
            AbilityDescription = abilityDescription;
            AbilitySprite = abilitySprite;
            AbilityColor = abilityColor;
            FixedTooltipWidth = fixedTooltipWidth;
        }
    }

    [System.Serializable]
    public class Positioning
    {
        [SerializeField] private Transform _leftDisplaySpot;
        [SerializeField] private Transform _rightDisplaySpot;
        [SerializeField] private Transform _centerSpot;

        private Vector3 LeftSpotPosition => _leftDisplaySpot.position;
        private Vector3 RightSpotPosition => _rightDisplaySpot.position;
        private Vector3 CenterPosition => _centerSpot.position;

        private const float START_DISPLAYING_LEFT_SCREEN_PER1 = 0.55f;
        
        
        public void TODO_GetCanvasDisplayPosition(Camera displayCamera, 
            out bool displayRightSide, out Vector3 displayPosition)
        {
            Vector3 centerPositionScreen = displayCamera.WorldToScreenPoint(CenterPosition);

            displayRightSide = ShouldDisplayRight(displayCamera, centerPositionScreen);
            Vector3 displayPositionWorld = displayRightSide
                ? RightSpotPosition
                : LeftSpotPosition;

            displayPosition = displayCamera.WorldToScreenPoint(displayPositionWorld);
        }

        private bool ShouldDisplayRight(Camera displayCamera, Vector3 centerPositionScreen)
        {
            return centerPositionScreen.x < displayCamera.pixelWidth * START_DISPLAYING_LEFT_SCREEN_PER1;
        }
    }


    public readonly Positioning DisplayPositioning;
    public readonly Element[] Elements;


    private CardTooltipDisplayData(Positioning displayPositioning, Element[] elements)
    {
        DisplayPositioning = displayPositioning;
        Elements = elements;
    }
    
    public CardTooltipDisplayData()
    {
        // Placeholder constructor
    }
    

    public static CardTooltipDisplayData MakeForTurretCard(Positioning displayPositioning, TurretCardData turretCardData)
    {
        var projectileModel = turretCardData.SharedPartsGroup.Projectile;
        List<ATurretPassiveAbility> passiveAbilities = turretCardData.PassiveAbilitiesController.PassiveAbilities;

        List<Element> elements = new List<Element>(1 + passiveAbilities.Count);
        HashSet<TurretPartProjectileDataModel> alreadyAddedProjectiles = new(1);
        HashSet<ATurretPassiveAbilityDataModel> alreadyAddedPassiveAbilities = new(passiveAbilities.Count);
        
        alreadyAddedProjectiles.Add(projectileModel);
        elements.Add(ElementFromProjectile(projectileModel, turretCardData.CurrentProjectileDescription));
        
        foreach (ATurretPassiveAbility passiveAbility in passiveAbilities)
        {
            ATurretPassiveAbilityDataModel passiveAbilityOriginalModel = passiveAbility.OriginalModel;
            alreadyAddedPassiveAbilities.Add(passiveAbilityOriginalModel);
            elements.Add(ElementFromPassive(passiveAbilityOriginalModel, passiveAbility.GetAbilityDescription()));
            AddReferencedProjectiles(passiveAbilityOriginalModel, elements, alreadyAddedProjectiles);
            AddReferencedPassiveAbilities(passiveAbilityOriginalModel, elements, alreadyAddedPassiveAbilities);
        }
        
        return new CardTooltipDisplayData(displayPositioning, elements.ToArray());
    }

    private static void AddReferencedProjectiles(ATurretPassiveAbilityDataModel passiveAbility, 
        List<Element> elementsList, HashSet<TurretPartProjectileDataModel> alreadyAddedProjectiles)
    {
        foreach (TurretPartProjectileDataModel referencedProjectileDataModel in passiveAbility.GetReferencedProjectiles())
        {
            if (alreadyAddedProjectiles.Contains(referencedProjectileDataModel))
            {
                continue;
            }
            elementsList.Add(ElementFromProjectile(referencedProjectileDataModel, referencedProjectileDataModel.MakeAbilityDescription()));
            alreadyAddedProjectiles.Add(referencedProjectileDataModel);
        }
    }
    
    private static void AddReferencedPassiveAbilities(ATurretPassiveAbilityDataModel passiveAbility, 
        List<Element> elementsList, HashSet<ATurretPassiveAbilityDataModel> alreadyAddedPassiveAbilities)
    {
        foreach (ATurretPassiveAbilityDataModel referencedPassiveAbility in passiveAbility.GetReferencedAbilities())
        {
            ATurretPassiveAbilityDataModel referencedPassiveAbilityDataModel = referencedPassiveAbility;
            if (alreadyAddedPassiveAbilities.Contains(referencedPassiveAbilityDataModel))
            {
                continue;
            }
            
            elementsList.Add(ElementFromPassive(referencedPassiveAbilityDataModel, referencedPassiveAbility.MakePassiveAbility().GetAbilityDescription()));
            alreadyAddedPassiveAbilities.Add(referencedPassiveAbilityDataModel);
        }
    }
    

    public static CardTooltipDisplayData MakeForSupportCard(Positioning displayPositioning,
        SupportPartBase supportPartBase,
        EditableCardAbilityDescription[] defaultAndUpgradesDescriptions)
    {
        Element[] elements = new[]
        {
            ElementFromSupport(defaultAndUpgradesDescriptions[0], supportPartBase),
            //ElementFromSupport(defaultAndUpgradesDescriptions[1], supportPartBase),
            //ElementFromSupport(defaultAndUpgradesDescriptions[2], supportPartBase),
            //ElementFromSupport(defaultAndUpgradesDescriptions[3], supportPartBase)
        };
        
        return new CardTooltipDisplayData(displayPositioning, elements);
    }


    public static CardTooltipDisplayData MakeForProjectileCardPart(Positioning displayPositioning,
        TurretPartProjectileDataModel projectileModel, EditableCardAbilityDescription projectileDescription)
    {
        Element[] elements = new[]
        {
            ElementFromProjectile(projectileModel, projectileDescription)
        };
        
        return new CardTooltipDisplayData(displayPositioning, elements);
    }

    
    public static CardTooltipDisplayData MakeForCardPartPassive(Positioning displayPositioning,
        ATurretPassiveAbilityDataModel passiveModel,
        EditableCardAbilityDescription passiveDescription)
    {
        Element[] elements = new[]
        {
            ElementFromPassive(passiveModel, passiveDescription)
        };
        
        return new CardTooltipDisplayData(displayPositioning, elements);
    }

    
    public static CardTooltipDisplayData MakeForCardPartStatsUpgrade(Positioning displayPositioning,
        CardPartBonusStats.DescriptionHelpReferences descriptionHelper,
        EditableCardAbilityDescription statsDescription)
    {
        Element[] elements = new[]
        {
            new Element(statsDescription, descriptionHelper.UpgradeSprite, descriptionHelper.SpriteColor, false)
        };

        return new CardTooltipDisplayData(displayPositioning, elements);
    } 
    
    
    
    private static Element ElementFromProjectile(TurretPartProjectileDataModel projectileModel, 
        EditableCardAbilityDescription projectileDescription)
    {
        return new Element(projectileDescription, projectileModel.abilitySprite, projectileModel.materialColor);
    }
    private static Element ElementFromPassive(ATurretPassiveAbilityDataModel passiveModel, 
        EditableCardAbilityDescription passiveDescription)
    {
        return new Element(passiveDescription, passiveModel.View.Sprite, passiveModel.View.Color);
    }

    private static Element ElementFromSupport(EditableCardAbilityDescription abilityDescription, SupportPartBase supportPartBase)
    {
        return new Element(abilityDescription, supportPartBase.abilitySprite, supportPartBase.spriteColor);
    }
}