using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseCollider : MonoBehaviour
{
    [SerializeField] public TriggerNotifier triggerNotifier;

    [SerializeField] protected MeshRenderer rangePlaneMesh;
    [SerializeField] protected MeshRenderer upgradePreviewRangePlaneMesh;
    protected Material rangePlaneMaterial;
    protected Material upgradePreviewRangePlaneMaterial;

    
    private void Awake()
    {
        rangePlaneMaterial = rangePlaneMesh.material;        
        upgradePreviewRangePlaneMaterial = upgradePreviewRangePlaneMesh.material;        
    }


    public abstract void UpdateRange(float statsRange);
    public abstract void EnableCollisions();
    public abstract void DisableCollisions();

    public abstract bool IsPointWithinRange(Vector3 point);
    public abstract bool IsBoundsWithinRange(Bounds bounds);

    public void HideRange()
    {
        rangePlaneMesh.gameObject.SetActive(false);
        HidePreviewRange();
    }

    public void ShowRange()
    {
        rangePlaneMesh.gameObject.SetActive(true);
    }
    
    public void ShowPreviewRange(float currentRangeStat, float nextRangeStat)
    {
        upgradePreviewRangePlaneMesh.gameObject.SetActive(true);
        UpdatePreviewRange(currentRangeStat, nextRangeStat);
    }
    public void HidePreviewRange()
    {
        upgradePreviewRangePlaneMesh.gameObject.SetActive(false);
    }

    protected abstract void UpdatePreviewRange(float currentRangeStat, float nextRangeStat);
    
    public void SetRangeColor(Color color)
    {
        rangePlaneMaterial.color = color;
    }

    public abstract Collider GetCollider();
    public abstract bool ColliderIsWithinRange(SphereCollider otherCollider);
    public abstract bool ColliderWillBeWithinRange(SphereCollider otherCollider);


}
