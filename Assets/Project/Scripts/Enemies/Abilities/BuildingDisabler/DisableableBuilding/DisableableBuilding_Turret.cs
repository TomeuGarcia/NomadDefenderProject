using UnityEngine;

public class DisableableBuilding_Turret : MonoBehaviour, IDisableableBuilding
{
    [SerializeField] private DisableableBuildingView _view;
    private TurretBuilding _turret;
    private bool _activeForDisable;


    public void Init(TurretBuilding turret)
    {
        _turret = turret;
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
        _turret.IsDisabled = true;
        _view.Show();
    }

    public void RestartDisabled()
    {
        
    }

    public void FinishDisabled()
    {
        _turret.IsDisabled = false;
        _view.Hide();
    }

    public void UpdateDisabled(float disabledRatio01)
    {
        _view.UpdateView(disabledRatio01);
    }
}