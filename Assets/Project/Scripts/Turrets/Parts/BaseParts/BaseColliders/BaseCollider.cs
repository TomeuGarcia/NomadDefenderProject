using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseCollider : MonoBehaviour
{
    [SerializeField] public TriggerNotifier triggerNotifier;

    [SerializeField] protected GameObject rangePlaneMeshObject;
    protected Material rangePlaneMaterial;

    public abstract void UpdateRange(float statsRange);
    public abstract void EnableCollisions();
    public abstract void DisableCollisions();


    public void HideRange()
    {
        rangePlaneMeshObject.SetActive(false);
    }

    public void ShowRange()
    {
        rangePlaneMeshObject.SetActive(true);
    }

}
