
public interface ITurretShotsPerSecondState
{
    float BaseShotsPerSecond { get; }
    float BaseShotsPerSecondInverted { get; }
    string BaseShotsPerSecondText { get; }

    float GetShotsPerSecondByLevel(int upgradeLevel);
    float GetShotsPerSecondInvertedByLevel(int upgradeLevel);
    string GetShotsPerSecondByLevelText(int upgradeLevel);
}
