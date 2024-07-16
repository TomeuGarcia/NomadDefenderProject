using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacilityPointAndClickManager : MonoBehaviour
{
    public bool CanInteract => _canInteract;
    private bool _canInteract = true;

    [SerializeField] private List<AFacilityInteractable> _interactables = new();

    private void Awake()
    {
        foreach (AFacilityInteractable aFacilityInteractable in _interactables)
        {
            aFacilityInteractable.Init(this);
        }
    }

    public void DisableInteractions()
    {
        _canInteract = false;
    }

    public void FinishedInteraction()
    {
        _canInteract = true;
    }
}
