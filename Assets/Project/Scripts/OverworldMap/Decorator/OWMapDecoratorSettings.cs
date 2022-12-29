using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewOWMapDecoratorSettings", menuName = "Map/OWMapDecoratorSettings")]
public class OWMapDecoratorSettings : ScriptableObject
{
    [SerializeField] public bool firstNodeIsEmpty = true;
    [SerializeField, Min(0)] public int numStartUpgradesLevels = 2;
    [SerializeField, Min(0)] public int battleAfterNLevels = 3;
    [SerializeField] public bool lastNodeIsBattle = true;

}
