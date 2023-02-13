using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathLocation : MonoBehaviour
{
    [SerializeField] private int health = 3;
    public HealthSystem healthSystem { get; private set; }

    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material deadMaterial;
    [SerializeField] private HealthHUD healthHUD;


    public bool IsDead => healthSystem.IsDead();


    public delegate void PathLocationAction();
    public event PathLocationAction OnDeath;


    private void Awake()
    {
        healthSystem = new HealthSystem(health);
        healthHUD.Init(healthSystem);
    }

    public void TakeDamage(int damageAmount)
    {
        healthSystem.TakeDamage(damageAmount);
        
        if (healthSystem.IsDead())
        {
            Die();
        }   
    }

    private void Die()
    {
        meshRenderer.material = deadMaterial;

        if (OnDeath != null) OnDeath();
    }

}
