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
        healthImage.fillAmount = healthSystem.healthRatio;
    }
    private void UpdateArmorImage()
    {
        Debug.Log(healthSystem.GetArmorRatio());
        armorImage.fillAmount = healthSystem.GetArmorRatio();
        if (!healthSystem.HasArmor())
        {
            armorImage.enabled = false;
            //feedback to destroy armor
        }
    }

    public void Hide()
    {
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

}
