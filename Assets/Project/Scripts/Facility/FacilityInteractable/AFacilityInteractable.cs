using OWmapShowcase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AFacilityInteractable : MonoBehaviour
{
    protected FacilityPointAndClickManager _manager;

    [Header("PARAMETERS")]
    [SerializeField] protected bool _needsPermissionToInteract = true;
    [SerializeField] protected float _duration;

    public void Init(FacilityPointAndClickManager manager)
    {
        _manager = manager;
        DoInit();
    }

    protected virtual void DoInit() { }

    public void Interact()
    {
        if(!_needsPermissionToInteract || _manager.CanInteract)
        {
            _manager.DisableInteractions();
            StartCoroutine(InteractionCoroutine());
        }
    }

    private IEnumerator InteractionCoroutine()
    {
        yield return DoInteract();
        FinishedInteraction();
    }

    protected abstract IEnumerator DoInteract();

    protected void FinishedInteraction()
    {
        _manager.FinishedInteraction();
    }
}
