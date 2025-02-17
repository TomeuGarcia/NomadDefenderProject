using UnityEngine;
using UnityEngine.Serialization;

public class DisableableBuilding_Support : MonoBehaviour, IDisableableBuilding
{
    [SerializeField] private DisableableBuildingView _view;
    private SupportBuilding _support;
    private bool _activeForDisable;


    public void Init(SupportBuilding support)
    {
        _support = support;
        _activeForDisable = false;
    }

    public void Ready()
    {
        _activeForDisable = true;
    }
    public void Cancel()
    {
        _activeForDisable = false;
    }
    
    
    public bool CanBeDisabled()
    {
        return gameObject.activeInHierarchy && _activeForDisable;
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