using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FacilityTrophy : MonoBehaviour
{
    [SerializeField] private UnlockableTrophyModel _model;
    
    void Start()
    {
        InitIfUnlocked();
    }

    private void InitIfUnlocked()
    {
        gameObject.SetActive(_model.IsUnlocked());
    }

}
