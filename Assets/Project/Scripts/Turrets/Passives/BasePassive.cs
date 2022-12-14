using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PassiveType { TARGETING, DAMAGE }

public class BasePassive : MonoBehaviour
{
    [SerializeField] protected PassiveType passiveType;

    protected virtual void ApllyEffects() { }
}
