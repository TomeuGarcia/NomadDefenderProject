
public interface ITurretDamageState
{
    int BaseDamage { get; }
    string BaseDamageText { get; }

    int GetDamageByLevel(int upgradeLevel);
    string GetDamageByLevelText(int upgradeLevel);
}
