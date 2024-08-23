using UnityEngine;

public interface ICardDescriptionProvider
{
    public class SetupData
    {
        public readonly string AbilityName;
        public readonly string AbilityDescription;
        public readonly Sprite Icon;
        public readonly Color IconColor;

        public SetupData()
        {
            AbilityName = "none";
            AbilityDescription = "No ability.";
            Icon = null;
            IconColor = Color.black;
        }
        public SetupData(string abilityName, string abilityDescription, Sprite icon, Color iconColor)
        {
            AbilityName = abilityName;
            AbilityDescription = abilityDescription;
            Icon = icon;
            IconColor = iconColor;
        }

        private TurretPartProjectileDataModel _projectileDataModel;
        private ATurretPassiveAbility[] _passiveAbilities;
        
        public void WithProjectile(TurretPartProjectileDataModel projectileDataModel)
        {
            
        }
        
        public void WithPassiveAbilities(ATurretPassiveAbility[] passiveAbilities)
        {
            
        }
        
    }

    public class DescriptionCornerPositions
    {
        public DescriptionCornerPositions(Vector3 leftPosition, Vector3 rightPosition)
        {
            this.leftPosition = leftPosition;
            this.rightPosition = rightPosition;
        }
        public Vector3 leftPosition;
        public Vector3 rightPosition;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <returns>Element 0 = Projectile Attack, Element 1 = Base Passive</returns>
    public SetupData[] GetAbilityDescriptionSetupData();

    public Vector3 GetCenterPosition();


    public DescriptionCornerPositions GetCornerPositions();
}
