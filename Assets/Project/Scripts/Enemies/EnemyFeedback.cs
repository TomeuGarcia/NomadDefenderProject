using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFeedback : MonoBehaviour
{
    [Header("ARMOR")]
    [SerializeField] private List<MeshRenderer> armorCovers = new List<MeshRenderer>();
    [SerializeField] private ParticleSystem armorShatterParticles;
    [SerializeField] private List<MaterialLerp.FloatData> matLerpsData = new List<MaterialLerp.FloatData>();
    private string armorDisappearCoef = "_DisappearCoef";
    private IEnumerator armorGainLerp;
    private IEnumerator armorBrightnesLerp;

    [Header("AREA ABILITY")]
    [SerializeField] private MeshRenderer areaMeshIndicator;
    [SerializeField] private MaterialLerp.FloatData areaMatLerpData;
    private IEnumerator areaCooldownLerp;


    public void ResetEnemy(bool hasArmor)
    {
        foreach (MeshRenderer mesh in armorCovers)
        {
            mesh.enabled = hasArmor;

            foreach (Material material in mesh.materials)
            {
                material.SetFloat(armorDisappearCoef, Convert.ToInt32(hasArmor));
            }

            if (hasArmor)
            {
                armorGainLerp = MaterialLerp.FloatLerp(matLerpsData[0], mesh.materials);
                StartCoroutine(armorGainLerp);
            }
        }
    }

    //ARMOR
    public void ArmorUpdate(HealthSystem.UpdateType updateType)
    {
        if (!gameObject.activeInHierarchy) return;

        switch (updateType)
        {
            case HealthSystem.UpdateType.INCREASE:
                IncreaseArmor();
                break;
            case HealthSystem.UpdateType.DECREASE:
                DecreaseArmor();
                break;
            case HealthSystem.UpdateType.GAIN:
                GainArmor();
                break;
            case HealthSystem.UpdateType.LOSE:
                LoseArmor();
                break;
            case HealthSystem.UpdateType.IGNORE:
            default:
                break;
        }
    }

    private void IncreaseArmor()
    {
        //play inc sound


        foreach (MeshRenderer mesh in armorCovers)
        {
            armorBrightnesLerp = MaterialLerp.FloatLerp(matLerpsData[1], mesh.materials);
            StartCoroutine(armorBrightnesLerp);
        }
    }
    private void DecreaseArmor()
    {
        //play hit sound

        foreach (MeshRenderer mesh in armorCovers)
        {
            armorBrightnesLerp = MaterialLerp.FloatLerp(matLerpsData[1], mesh.materials);
            StartCoroutine(armorBrightnesLerp);
        }        
    }
    private void GainArmor()
    {
        //play gain sound


        //ACTIVATE SHIELD
        foreach (MeshRenderer mesh in armorCovers)
        {
            mesh.enabled = true;

            armorGainLerp = MaterialLerp.FloatLerp(matLerpsData[0], mesh.materials);
            StartCoroutine(armorGainLerp);
        }
    }
    private void LoseArmor()
    {
        //play break sound


        //SHATTER SHIELD
        foreach (MeshRenderer mesh in armorCovers)
        {
            armorShatterParticles.Play();

            mesh.enabled = false;
            foreach (Material material in mesh.materials)
            {
                material.SetFloat(armorDisappearCoef, 1.0f);
            }
        }
    }


    //AREA
    public void AreaOnCooldown(float lerpTime)
    {
        areaMatLerpData.time = lerpTime;
        areaCooldownLerp = MaterialLerp.FloatLerp(areaMatLerpData, areaMeshIndicator.materials);
        StartCoroutine(areaCooldownLerp);
    }


    public void FinishCoroutines()
    {
        StopAllCoroutines();
    }
}
