using UnityEngine;

public class DisableableBuilding_Turret : MonoBehaviour, IDisableableBuilding
{
    [SerializeField] private TurretBuilding _turret;
    [SerializeField] private DisableableBuildingView _view;
    
    
    public bool CanBeDisabled()
    {
        return gameObject.activeInHierarchy;
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