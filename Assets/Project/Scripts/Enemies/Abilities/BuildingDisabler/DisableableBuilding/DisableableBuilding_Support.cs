using UnityEngine;
using UnityEngine.Serialization;

public class DisableableBuilding_Support : MonoBehaviour, IDisableableBuilding
{
    [SerializeField] private SupportBuilding _support;
    [SerializeField] private DisableableBuildingView _view;

    public bool CanBeDisabled()
    {
        return gameObject.activeInHierarchy;
    }

    public void StartDisabled()
    {
        _support.GetBasePart().DoOnBuildingDisableStart();
        _view.Show();
    }

    public void RestartDisabled()
    {
        
    }

    public void FinishDisabled()
    {
        _support.GetBasePart().DoOnBuildingDisableFinish();
        _view.Hide();
    }

    public void UpdateDisabled(float disabledRatio01)
    {
        _view.UpdateView(disabledRatio01);
    }
}