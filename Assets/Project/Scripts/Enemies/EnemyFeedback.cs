using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFeedback : MonoBehaviour
{
    [Header("ARMOR")]
    [SerializeField] private MeshRenderer armorCover;
    [SerializeField] private ParticleSystem armorShatterParticles;
    [SerializeField] private List<MaterialLerp.FloatData> matLerpsData = new List<MaterialLerp.FloatData>();
    private string armorDisappearCoef = "_DisappearCoef";
    private IEnumerator armorGainLerp;
    private IEnumerator armorBrightnesLerp;

    public void ResetEnemy(bool hasArmor)
    {
        armorCover.enabled = hasArmor;

        foreach (Material material in armorCover.materials)
        {
            material.SetFloat(armorDisappearCoef, Convert.ToInt32(hasArmor));
        }

        if (hasArmor)
        {
            armorGainLerp = MaterialLerp.FloatLerp(matLerpsData[0], armorCover.materials);
            StartCoroutine(armorGainLerp);
        }
    }

    //ARMOR
    public void ArmorUpdate(HealthSystem.UpdateType updateType)
    {
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


        armorBrightnesLerp = MaterialLerp.FloatLerp(matLerpsData[1], armorCover.materials);
        StartCoroutine(armorBrightnesLerp);
    }
    private void DecreaseArmor()
    {
        //play hit sound


        armorBrightnesLerp = MaterialLerp.FloatLerp(matLerpsData[1], armorCover.materials);
        StartCoroutine(armorBrightnesLerp);
    }
    private void GainArmor()
    {
        //play gain sound


        //ACTIVATE SHIELD
        armorCover.enabled = true;

        armorGainLerp = MaterialLerp.FloatLerp(matLerpsData[0], armorCover.materials);
        StartCoroutine(armorGainLerp);
    }
    private void LoseArmor()
    {
        //play break sound


        //SHATTER SHIELD
        armorShatterParticles.Play();

        armorCover.enabled = false;
        foreach (Material material in armorCover.materials)
        {
            material.SetFloat(armorDisappearCoef, 1.0f);
        }
    }
}
