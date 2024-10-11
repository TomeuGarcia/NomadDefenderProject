public interface IDisableableBuilding
{
    bool CanBeDisabled();
    void StartDisabled();
    void RestartDisabled();
    void FinishDisabled();
    void UpdateDisabled(float disabledRatio01);
}