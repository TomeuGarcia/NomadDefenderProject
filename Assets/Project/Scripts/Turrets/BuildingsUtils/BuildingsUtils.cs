using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingsUtils", menuName = "BuildingsUtils")]
public class BuildingsUtils : ScriptableObject
{    
    public Material AttackUpgradeParticleMat => attackUpgradeParticleMat;
    [SerializeField] private Material attackUpgradeParticleMat;
    public Material CadencyUpgradeParticleMat => cadencyUpgradeParticleMat;
    [SerializeField] private Material cadencyUpgradeParticleMat;
    public Material RangeUpgradeParticleMat => rangeUpgradeParticleMat;
    [SerializeField] private Material rangeUpgradeParticleMat;
    public Material SupportUpgradeParticleMat => supportUpgradeParticleMat;
    [SerializeField] private Material supportUpgradeParticleMat;

}
