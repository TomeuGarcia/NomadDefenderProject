using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class HealthSystem
{
    [SerializeField] private int maxHealth;
    public int health { get; private set; }

    public float HealthRatio => (float)health / (float)maxHealth;

    public delegate void HealthSystemAction();
    public event HealthSystemAction OnHealthUpdated;


    public HealthSystem(int maxHealth)
    {
        this.maxHealth = maxHealth;
        health = this.maxHealth;
    }

    public void TakeDamage(int damageAmount)
    {
        //float damagedArmor = armor - damageAmount;
        //damageAmount = damageAmount - armor;
        //armor = damagedArmor;

        //if(armor <= 0)
        //visuallyDestroyArmor()
        health -= damageAmount;
        health = health > 0 ? health : 0;

        InvokeOnHealthUpdated();
    }

    public void Heal(int healAmount)
    {
        health += healAmount;
        health = health < maxHealth ? health : maxHealth;

        InvokeOnHealthUpdated();
    }

    public void HealToMax()
    {
        health = maxHealth;

        InvokeOnHealthUpdated();
    }

    public bool IsDead()
    {
        return health <= 0;
    }

    public bool IsFullHealth()
    {
        return health == maxHealth;
    }

    private void InvokeOnHealthUpdated()
    {
        if (OnHealthUpdated != null) OnHealthUpdated();
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

}
