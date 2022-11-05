using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthHUD : MonoBehaviour
{
    private HealthSystem healthSystem;
    [SerializeField] private Image healthImage;

    public void Init(HealthSystem healthSystem)
    {
        this.healthSystem = healthSystem;

        this.healthSystem.OnHealthUpdated += UpdateHealthImage;
    }

    private void UpdateHealthImage()
    {
        healthImage.fillAmount = healthSystem.healthRatio;
    }


}
