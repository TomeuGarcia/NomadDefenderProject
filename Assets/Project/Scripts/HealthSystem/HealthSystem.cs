using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class HealthSystem
{
    [SerializeField] private int maxHealth;
    [SerializeField] private int maxArmor;
    [SerializeField] private int spawnArmorAmount;
    public int health { get; private set; }
    public int armor { get; private set; }

    public float HealthRatio => (float)health / (float)maxHealth;

    public delegate void HealthSystemAction();
    public event HealthSystemAction OnHealthUpdated;
    public event HealthSystemAction OnArmorUpdated;


    public HealthSystem(int maxHealth)
    {
        this.maxHealth = maxHealth;
        health = this.maxHealth;
        maxArmor = this.maxHealth;
        armor = 0;
        
    }
    public HealthSystem(int maxHealth , int maxArmour)
    {
        this.maxHealth = maxHealth;
        this.maxArmor = maxArmour;
        health = this.maxHealth;
        armor = spawnArmorAmount;
        
    }
    public void TakeDamage(int damageAmount)
    {
        if (HasArmor())
        {
            Debug.Log("Take damage on armor");
            armor -= damageAmount;
            int remainingDamage = -armor;
            armor = armor > 0 ? armor : 0;
            InvokeOnArmorUpdated();
            //visuallyDestroyArmor();
        }
        else
        {
            health -= damageAmount;
            health = health > 0 ? health : 0;
            InvokeOnHealthUpdated();
        }
    }

    public void Heal(int healAmount)
    {
        health += healAmount;
        health = health < maxHealth ? health : maxHealth;

        InvokeOnHealthUpdated();
    }
    public void AddArmor(int armorAmount)
    {
        armor += armorAmount;
        armor = armor < maxArmor ? armor : maxArmor;
        InvokeOnArmorUpdated();
    }

    public void HealToMax()
    {
        health = maxHealth;
        InvokeOnHealthUpdated();
    }
    public void ArmorToMax()
    {
        armor = maxArmor;
        if (HasArmor()) InvokeOnArmorUpdated();
    }
    public void SetArmor(int armorAmount)
    {
        armor = armorAmount;
        armor = armor < maxArmor ? armor : maxArmor;
    }
    public bool IsDead()
    {
        return health <= 0;
    }

    public bool IsFullHealth()
    {
        return health == maxHealth;
    }

    public bool HasArmor()
    {
        return armor > 0;
    }
    private void InvokeOnHealthUpdated()
    {
        if (OnHealthUpdated != null) OnHealthUpdated();
    }
    private void InvokeOnArmorUpdated()
    {
        if (OnArmorUpdated != null) OnArmorUpdated();
    }

    public void UpdateHealth(int newHealth)
    {
        health = newHealth;
        maxHealth = newHealth;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }
    public int GetSpawnArmor()
    {
        return spawnArmorAmount;
    }
    public float GetArmorRatio()
    {
        if (maxArmor > 0)
        {
            return (float)armor / (float)maxArmor;
        }
        return 0;

    }

}
