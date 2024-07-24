using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.ObjectPooling;


[CreateAssetMenu(fileName = "GeneralParticleFactoryConfig",
    menuName = SOAssetPaths.VFX + "GeneralParticleFactoryConfig")]
public class GeneralParticleFactoryConfig : ScriptableObject
{
    [Header("Building Utils")]
    [SerializeField] private BuildingsUtils _buildingsUtils;
    public BuildingsUtils BuildingsUtils => _buildingsUtils;


    [Header("BUILDING UPGRADES")]
    [SerializeField] private ObjectPoolInitData<RecyclableParticles> _buildingUpgrade;
    public ObjectPoolInitData<RecyclableParticles> BuildingUpgrade => _buildingUpgrade;

}
