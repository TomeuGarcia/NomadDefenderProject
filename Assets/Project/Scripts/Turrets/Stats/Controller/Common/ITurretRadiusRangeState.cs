
public interface ITurretRadiusRangeState
{
    float BaseRadiusRange { get; }
    string BaseRadiusRangeText { get; }

    float GetRadiusRangeByLevel(int upgradeLevel);
    string GetRadiusRangeByLevelText(int upgradeLevel);
}
