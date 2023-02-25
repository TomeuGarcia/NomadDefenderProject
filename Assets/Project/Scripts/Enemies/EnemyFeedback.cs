using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFeedback : MonoBehaviour
{
    [Header("ARMOR")]
    [SerializeField] private MeshRenderer armorCover;
    [SerializeField] private ParticleSystem armorShatterParticles;
    [SerializeField] private float armorLerpCoefMax;
    [SerializeField] private float armorLerpCoefMin;
    [SerializeField] private float armorLerpTime;
    [SerializeField] private AnimationCurve armorLerpCurve;
    private List<Material> armorMaterials = new List<Material>();
    private string armorDisappearCoef = "_DisappearCoef";
    private IEnumerator armorMatLerp;

    private void Awake()
    {
        foreach(Material material in armorCover.materials)
        {
            armorMaterials.Add(material);
        }
    }

    public void ResetEnemy(bool hasArmor)
    {
        armorCover.enabled = hasArmor;

        foreach (Material material in armorCover.materials)
        {
            material.SetFloat(armorDisappearCoef, Convert.ToInt32(hasArmor));
        }

        if (hasArmor)
        {
            armorMatLerp = ArmorMaterialLerp();
            StartCoroutine(armorMatLerp);
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

    }
    private void DecreaseArmor()
    {

    }
    private void GainArmor()
    {
        //ACTIVATE SHIELD
        armorCover.enabled = true;

        //LERP "_DisappearAmount" to 0
        armorMatLerp = ArmorMaterialLerp();
        StartCoroutine(armorMatLerp);
    }
    private void LoseArmor()
    {
        //SHATTER SHIELD
        armorShatterParticles.Play();

        foreach (Material material in armorCover.materials)
        {
            material.SetFloat(armorDisappearCoef, 1.0f);
        }
    }

    private IEnumerator ArmorMaterialLerp()
    {
        float tParam;
        float currentTime = 0.0f;
        float lerpCoefDif = armorLerpCoefMax - armorLerpCoefMin;

        while(currentTime < armorLerpTime)
        {
            currentTime += Time.deltaTime;

            tParam = armorLerpCurve.Evaluate(currentTime / armorLerpTime);
            tParam = armorLerpCoefMin + lerpCoefDif * tParam;

            foreach (Material material in armorCover.materials)
            {
                material.SetFloat(armorDisappearCoef, 1 - tParam);
            }

            yield return null;
        }

        foreach (Material material in armorCover.materials)
        {
            material.SetFloat(armorDisappearCoef, 0.0f);
        }
    }
}
