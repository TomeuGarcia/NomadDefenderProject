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

    private void Awake()
    {
        DoAwake();
    }

    protected virtual void DoAwake() { }

    public void Init(FacilityPointAndClickManager manager)
    {
        _manager = manager;
        DoInit();
    }

    protected virtual void DoInit() { }
    public virtual void InteractedStart() { }

    public void Interact()
    {
        if(CanInteract())
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

    public bool CanInteract()
    {
        return ((!_needsPermissionToInteract || _manager.CanInteract) && ExtraInteractionConditions());
    }

    protected virtual bool ExtraInteractionConditions()
    {
        return true;
    }
}
