using System.Collections;
using System.Collections.Generic;
using UnityEditor.Compilation;
using UnityEngine;

public enum PassiveType { TARGETING, DAMAGE }

public abstract class BasePassive : MonoBehaviour
{
    public abstract void Init(RangeBuilding owner);

    protected PassiveType passiveType;
    protected virtual void ApllyEffects() { }
}
