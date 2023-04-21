using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthHUD : MonoBehaviour
{
    private HealthSystem healthSystem;
    [SerializeField] private Image healthImage;
    [SerializeField] private Image armorImage;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private bool startHidden;
    [SerializeField] private RectTransform canvasRectTransform;

    [Header("FEEDBACK")]
    [SerializeField] private Image armorFlashImage;
    [SerializeField] private ParticleSystem armorShatterParticles;
    private IEnumerator armorBreak;


    public void Init(HealthSystem healthSystem)
    {
        this.healthSystem = healthSystem;

        this.healthSystem.OnHealthUpdated += UpdateHealthImage;
        this.healthSystem.OnArmorUpdated += UpdateArmorImage;
        if (startHidden)
        {
            Hide();
        }
        SetArmorImageVisibility();
    }

    private void UpdateHealthImage(HealthSystem.UpdateType updateType)
    {
        healthImage.fillAmount = healthSystem.HealthRatio;
    }
    private void UpdateArmorImage(HealthSystem.UpdateType updateType)
    {
        //Debug.Log(healthSystem.GetArmorRatio());
        armorImage.fillAmount = healthSystem.GetArmorRatio();
        if (!healthSystem.HasArmor())
        {
            armorImage.enabled = false;
            //feedback to destroy armor

            armorBreak = ArmorBreak();
            StartCoroutine(armorBreak);
        }
    }

    public void Hide()
    {
        if(armorBreak != null)
        {
            StopCoroutine(armorBreak);
            armorFlashImage.enabled = false;
        }

        canvasGroup.alpha = 0.0f;
    }

    public void Show()
    {
        canvasGroup.alpha = 1.0f;
        SetArmorImageVisibility();
    }

    private void SetArmorImageVisibility()
    {
        if (!healthSystem.HasArmor())
        {
            armorImage.enabled = false;
        }
        else
        {
            armorImage.enabled = true;
        }
    }

    private IEnumerator ArmorBreak()
    {
        GameAudioManager.GetInstance().PlayEnemyArmorBreak();
        armorShatterParticles.Play();

        Color32 color = armorFlashImage.color;

        armorFlashImage.enabled = true;
        color.a = 100;
        armorFlashImage.color = color;
        yield return new WaitForSeconds(0.04f);
        color.a = 75;
        armorFlashImage.color = color;
        yield return new WaitForSeconds(0.02f);
        color.a = 50;
        armorFlashImage.color = color;
        yield return new WaitForSeconds(0.02f);
        color.a = 25;
        armorFlashImage.color = color;
        yield return new WaitForSeconds(0.02f);

        color.a = 255;
        armorFlashImage.color = color;
        armorFlashImage.enabled = false;
    }
}
