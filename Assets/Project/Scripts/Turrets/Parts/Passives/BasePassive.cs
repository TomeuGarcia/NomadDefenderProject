using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Compilation;
using UnityEngine;


public abstract class BasePassive : ScriptableObject
{
    public abstract void ApplyEffects(TurretBuilding owner);
}
