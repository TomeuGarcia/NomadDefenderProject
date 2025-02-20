using UnityEngine;
using UnityEngine.Serialization;

public class DisableableBuilding_Support : MonoBehaviour, IDisableableBuilding
{
    [SerializeField] private DisableableBuildingView _view;
    private SupportBuilding _support;
    private InBattleBuildingUpgrader _inBattleBuildingUpgrader;
    private bool _activeForDisable;


    public void Init(SupportBuilding support, InBattleBuildingUpgrader inBattleBuildingUpgrader)
    {
        _support = support;
        _inBattleBuildingUpgrader = inBattleBuildingUpgrader;
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
        _inBattleBuildingUpgrader.SetCanUpgradeVisibility(false);
    }

    public void RestartDisabled()
    {
        
    }

    public void FinishDisabled()
    {
        _support.GetBasePart().DoOnBuildingDisableFinish();
        _view.Hide();
        _inBattleBuildingUpgrader.SetCanUpgradeVisibility(true);
    }

    public void UpdateDisabled(float disabledRatio01)
    {
        _view.UpdateView(disabledRatio01);
    }
}