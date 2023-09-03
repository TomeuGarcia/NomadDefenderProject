using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Compilation;
using UnityEngine;


public abstract class BasePassive : ScriptableObject
{
    [Header("ABILITY INFO")]
    [Header("Name")]
    [SerializeField] public string abilityName;
    [Header("Description")]
    [SerializeField, TextArea(3, 5)] public string abilityDescription;


    public abstract void ApplyEffects(TurretBuilding owner);

    public virtual void GotEnabledPlacing()
    {
    }
    public virtual void GotDisabledPlacing()
    {
    }
    public virtual void GotMovedWhenPlacing()
    {
    }


}
