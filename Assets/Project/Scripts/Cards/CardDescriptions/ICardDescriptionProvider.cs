using UnityEngine;

public interface ICardDescriptionProvider
{
    public class SetupData
    {
        public string abilityName;
        public string abilityDescription;
        public Sprite icon;
        public Color iconColor;

        public SetupData()
        {
            this.abilityName = "none";
            this.abilityDescription = "No ability.";
            this.icon = null;
            this.iconColor = Color.black;
        }
        public SetupData(string abilityName, string abilityDescription, Sprite icon, Color iconColor)
        {
            this.abilityName = abilityName;
            this.abilityDescription = abilityDescription;
            this.icon = icon;
            this.iconColor = iconColor;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <returns>Element 0 = Projectile Attack, Element 1 = Base Passive</returns>
    public abstract SetupData[] GetAbilityDescriptionSetupData();
    public abstract Vector3 GetCenterPosition();
}
