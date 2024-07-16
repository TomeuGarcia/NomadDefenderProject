using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingsUtils", 
    menuName = SOAssetPaths.BUILDINGS + "BuildingsUtils")]
public class BuildingsUtils : ScriptableObject
{
    [Header("Particles Materials")]
    [SerializeField] private Material attackUpgradeParticleMat;
    public Material AttackUpgradeParticleMat => attackUpgradeParticleMat;


    [SerializeField] private Material cadencyUpgradeParticleMat;
    public Material CadencyUpgradeParticleMat => cadencyUpgradeParticleMat;


    [SerializeField] private Material rangeUpgradeParticleMat;
    public Material RangeUpgradeParticleMat => rangeUpgradeParticleMat;


    [SerializeField] private Material supportUpgradeParticleMat;
    public Material SupportUpgradeParticleMat => supportUpgradeParticleMat;




    [Header("Building PREVIEW Colors")]
    [SerializeField] private Color previewCanBePlacedColor = Color.green;
    public Color PreviewCanBePlacedColor => previewCanBePlacedColor;


    [SerializeField] private Color previewCanNOTBePlacedColor = Color.red;
    public Color PreviewCanNOTBePlacedColor => previewCanNOTBePlacedColor;


    [SerializeField] private Color previewPunchCanNOTBePlacedColor = Color.white;
    public Color PreviewPunchCanNOTBePlacedColor => previewPunchCanNOTBePlacedColor;
}
