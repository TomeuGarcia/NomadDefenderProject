using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[System.Serializable]
public class HealthSystem
{
    public enum UpdateType { INCREASE, DECREASE, GAIN, LOSE, IGNORE };

    [SerializeField] private int maxHealth;
    [SerializeField] private int maxArmor;
    [SerializeField] private int spawnArmorAmount;
    public int health { get; private set; }
    public int armor { get; private set; }

    public float HealthRatio => (float)health / (float)maxHealth;

    public delegate void HealthSystemAction(UpdateType updateType);
    public event HealthSystemAction OnHealthUpdated;
    public event HealthSystemAction OnArmorUpdated;


    public HealthSystem(int maxHealth)
    {
        this.maxHealth = maxHealth;
        health = this.maxHealth;
        maxArmor = this.maxHealth;
        armor = 0;
        spawnArmorAmount = 0;
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
        UpdateType type;

        if (HasArmor())
        {
            Debug.Log("Take damage on armor");
            armor -= damageAmount;
            int remainingDamage = -armor;
            armor = armor > 0 ? armor : 0;

            if (HasArmor()) { type = UpdateType.DECREASE; }
            else { type = UpdateType.LOSE; }
            InvokeOnArmorUpdated(type);
        }
        else
        {
            health -= damageAmount;
            health = health > 0 ? health : 0;

            if (IsDead()) { type = UpdateType.LOSE; }
            else { type = UpdateType.DECREASE; }
            InvokeOnHealthUpdated(type);
        }
    }

    public void Heal(int healAmount)
    {
        health += healAmount;
        health = health < maxHealth ? health : maxHealth;

        InvokeOnHealthUpdated(UpdateType.INCREASE);
    }
    public void AddArmor(int armorAmount)
    {
        UpdateType type;
        if (armor == 0) { type = UpdateType.GAIN; }
        else { type = UpdateType.INCREASE; }

        armor += armorAmount;
        armor = armor < maxArmor ? armor : maxArmor;
        InvokeOnArmorUpdated(type);
    }

    public void HealToMax()
    {
        health = maxHealth;
        InvokeOnHealthUpdated(UpdateType.IGNORE);
    }
    public void ResetArmor()
    {
        armor = spawnArmorAmount;
        if (HasArmor()) InvokeOnArmorUpdated(UpdateType.IGNORE);
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
    private void InvokeOnHealthUpdated(UpdateType updateType)
    {
        if (OnHealthUpdated != null) OnHealthUpdated(updateType);
    }
    private void InvokeOnArmorUpdated(UpdateType updateType)
    {
        if (OnArmorUpdated != null) OnArmorUpdated(updateType);
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
