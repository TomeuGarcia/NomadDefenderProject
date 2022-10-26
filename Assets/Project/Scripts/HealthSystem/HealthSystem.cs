using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class HealthSystem
{
    [SerializeField] private int maxHealth;
    private int health;

    public float healthRatio => (float)health / (float)maxHealth;


    public HealthSystem(int maxHealth)
    {
        this.maxHealth = maxHealth;
        health = this.maxHealth;
    }

    public void TakeDamage(int damageAmount)
    {
        health -= damageAmount;
        health = health > 0 ? health : 0;
    }

    public void Heal(int healAmount)
    {
        health += healAmount;
        health = health < maxHealth ? health : maxHealth;
    }

    public void HealToMax()
    {
        health = maxHealth;
    }

    public bool IsDead()
    {
        return health <= 0;
    }


}
