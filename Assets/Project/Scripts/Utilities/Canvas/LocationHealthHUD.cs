using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocationHealthHUD : MonoBehaviour
{
    private HealthSystem healthSystem;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private bool startHidden;


    [SerializeField] private Transform healthPointImagesHolder;
    [SerializeField] private GameObject prefabHealthItem;
    private List<LocationHealthItem> healthPointItems;
    private int currentHealth;


    public void Init(HealthSystem healthSystem)
    {
        this.healthSystem = healthSystem;

        this.healthSystem.OnHealthUpdated += UpdateHealthImage;
        if (startHidden)
        {
            Hide();
        }

        healthPointItems = new List<LocationHealthItem>();
        for (int i = 0; i < healthSystem.GetMaxHealth(); ++i)
        {
            LocationHealthItem healthPointItem = Instantiate(prefabHealthItem, healthPointImagesHolder).GetComponent<LocationHealthItem>();
            healthPointItem.gameObject.SetActive(true);
            this.healthPointItems.Add(healthPointItem);
        }
        currentHealth = healthSystem.GetMaxHealth();
    }

    private void UpdateHealthImage(HealthSystem.UpdateType updateType)
    {
        int updateStartHealthI = currentHealth - 1;
        int amount = currentHealth - healthSystem.health;
        currentHealth -= amount;


        if (amount > 0)
        {
            for (int i = updateStartHealthI; i > updateStartHealthI - amount; --i)
            {
                healthPointItems[i].TakeDamage();                
            }
        }
        else if (amount < 0)
        {            
            for (int i = updateStartHealthI +1 ; i < updateStartHealthI+1 - amount; ++i)
            {
                healthPointItems[i].GainHealth();
            }
        }
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
