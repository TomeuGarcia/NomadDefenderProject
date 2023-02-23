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

    private void UpdateHealthImage()
    {
        healthImage.fillAmount = healthSystem.HealthRatio;
    }
    private void UpdateArmorImage()
    {
        Debug.Log(healthSystem.GetArmorRatio());
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
        Color32 color = armorFlashImage.color;

        //yield return new WaitForSeconds(0.1f);

        armorFlashImage.enabled = true;
        color.a = (byte)75;
        armorFlashImage.color = color;
        yield return new WaitForSeconds(0.02f);
        color.a = (byte)50;
        armorFlashImage.color = color;
        yield return new WaitForSeconds(0.02f);
        color.a = (byte)25;
        armorFlashImage.color = color;
        yield return new WaitForSeconds(0.02f);

        color.a = (byte)255;
        armorFlashImage.color = color;
        armorFlashImage.enabled = false;
    }
}
