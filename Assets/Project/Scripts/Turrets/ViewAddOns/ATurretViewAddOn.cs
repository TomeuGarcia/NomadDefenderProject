using System.Threading.Tasks;
using Scripts.ObjectPooling;
using UnityEngine;

public abstract class ATurretViewAddOn : RecyclableObject
{
    private Transform _originalParent;

    private void Awake()
    {
        _originalParent = transform.parent;
    }

    public void StartViewing(Transform parent)
    {
        transform.SetParent(parent);
        transform.localPosition = Vector3.zero;

        StartPlayingEffects();
    }

    public void StopViewing()
    {
        StopPlayingEffects();
        
        transform.SetParent(_originalParent);
        Recycle();
    }


    protected abstract void StartPlayingEffects();
    protected abstract void StopPlayingEffects();
}