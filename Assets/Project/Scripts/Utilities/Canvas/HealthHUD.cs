using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthHUD : MonoBehaviour
{
    private HealthSystem healthSystem;
    [SerializeField] private Image healthImage;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private bool startHidden;
    [SerializeField] private RectTransform canvasRectTransform;



    public void Init(HealthSystem healthSystem)
    {
        this.healthSystem = healthSystem;

        this.healthSystem.OnHealthUpdated += UpdateHealthImage;

        if (startHidden)
        {
            Hide();
        }
    }

    private void UpdateHealthImage()
    {
        healthImage.fillAmount = healthSystem.healthRatio;
    }

    public void Hide()
    {
        canvasGroup.alpha = 0.0f;
    }

    public void Show()
    {
        canvasGroup.alpha = 1.0f;
    }

}
