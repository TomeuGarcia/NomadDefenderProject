public interface IDisableableBuilding
{
    void StartDisabled();
    void RestartDisabled();
    void FinishDisabled();
    void UpdateDisabled(float disabledRatio01);
}