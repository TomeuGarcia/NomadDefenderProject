using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseCollider : MonoBehaviour
{
    [SerializeField] public TriggerNotifier triggerNotifier;

    [SerializeField] protected MeshRenderer rangePlaneMesh;
    protected Material rangePlaneMaterial;

    private void Awake()
    {
        rangePlaneMaterial = rangePlaneMesh.material;        
    }


    public abstract void UpdateRange(float statsRange);
    public abstract void EnableCollisions();
    public abstract void DisableCollisions();

    public abstract bool IsPointWithinRange(Vector3 point);
    public abstract bool IsBoundsWithinRange(Bounds bounds);

    public void HideRange()
    {
        rangePlaneMesh.gameObject.SetActive(false);
    }

    public void ShowRange()
    {
        rangePlaneMesh.gameObject.SetActive(true);
        //rangePlaneMaterial.SetFloat("_StartTimeFadeIn", Time.time);
    }

    public void SetRangeColor(Color color)
    {
        rangePlaneMaterial.color = color;
    }

    

}
