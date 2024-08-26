using System.Collections.Generic;
using UnityEngine;

public class CardTooltipDisplayData
{
    public class Element
    {
        public readonly EditableCardAbilityDescription AbilityDescription;
        public readonly Sprite AbilitySprite;
        public readonly Color AbilityColor;

        public Element(EditableCardAbilityDescription abilityDescription, Sprite abilitySprite, Color abilityColor)
        {
            AbilityDescription = abilityDescription;
            AbilitySprite = abilitySprite;
            AbilityColor = abilityColor;
        }
        public Element(EditableCardAbilityDescription abilityDescription)
        {
            AbilityDescription = abilityDescription;
            AbilitySprite = null;
            AbilityColor = Color.black;
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

        private const float START_DISPLAYING_LEFT_SCREEN_PER1 = 0.80f;
        
        
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


    public readonly Element[] Elements;
    public readonly Positioning DisplayPositioning;

    
    
    public CardTooltipDisplayData(Positioning displayPositioning, TurretCardData turretCardData)
    {
        DisplayPositioning = displayPositioning;
        
        var projectileModel = turretCardData.SharedPartsGroup.Projectile;
        List<ATurretPassiveAbility> passiveAbilities = turretCardData.PassiveAbilitiesController.PassiveAbilities;

        List<Element> elements = new List<Element>(1 + passiveAbilities.Count);
        elements.Add(ElementFromProjectile(projectileModel, turretCardData.ProjectileDescription));
        foreach (ATurretPassiveAbility passiveAbility in passiveAbilities)  
        {
            elements.Add(ElementFromPassive(passiveAbility.OriginalModel, passiveAbility.AbilityDescription));
        }

        Elements = elements.ToArray();
    }
    
    
    public CardTooltipDisplayData(Positioning displayPositioning, 
        TurretPartProjectileDataModel projectileModel, 
        EditableCardAbilityDescription projectileDescription)
    {
        DisplayPositioning = displayPositioning;
        Elements = new[]
        {
            ElementFromProjectile(projectileModel, projectileDescription)
        };
    }
    
    
    public CardTooltipDisplayData(Positioning displayPositioning, 
        ATurretPassiveAbilityDataModel passiveModel, 
        EditableCardAbilityDescription passiveDescription)
    {
        DisplayPositioning = displayPositioning;
        Elements = new[]
        {
            ElementFromPassive(passiveModel, passiveDescription)
        };
    }


    public CardTooltipDisplayData(Positioning displayPositioning,
        SupportPartBase supportPartBase,
        EditableCardAbilityDescription[] defaultAndUpgradesDescriptions)
    {
        DisplayPositioning = displayPositioning;
        Elements = new[]
        {
            new Element(defaultAndUpgradesDescriptions[0], supportPartBase.abilitySprite, supportPartBase.spriteColor),
            new Element(defaultAndUpgradesDescriptions[1]),
            new Element(defaultAndUpgradesDescriptions[2]),
            new Element(defaultAndUpgradesDescriptions[3])
        };
    }
    
    public CardTooltipDisplayData(Positioning displayPositioning,
        CardPartBonusStats.DescriptionHelpReferences descriptionHelper,
        EditableCardAbilityDescription statsDescription)
    {
        DisplayPositioning = displayPositioning;
        Elements = new[]
        {
            new Element(statsDescription, descriptionHelper.UpgradeSprite, descriptionHelper.SpriteColor)
        };
    }
    
    public CardTooltipDisplayData(Positioning displayPositioning)
    {
        // Placeholder constructor
    }
    
    
    private Element ElementFromProjectile(TurretPartProjectileDataModel projectileModel, 
        EditableCardAbilityDescription projectileDescription)
    {
        return new Element(projectileDescription, projectileModel.abilitySprite, projectileModel.materialColor);
    }
    private Element ElementFromPassive(ATurretPassiveAbilityDataModel passiveModel, 
        EditableCardAbilityDescription passiveDescription)
    {
        return new Element(passiveDescription, passiveModel.View.Sprite, passiveModel.View.Color);
    }
    
}