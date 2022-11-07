using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathLocation : MonoBehaviour
{
    [SerializeField] private int health = 3;
    private HealthSystem healthSystem;

    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material deadMaterial;
    [SerializeField] private HealthHUD healthHUD;


    public bool IsDead => healthSystem.IsDead();


    private void Awake()
    {
        healthSystem = new HealthSystem(health);
        healthHUD.Init(healthSystem);
    }

    public void TakeDamage(int damageAmount)
    {
        healthSystem.TakeDamage(damageAmount);
        if (healthSystem.IsDead())
            meshRenderer.material = deadMaterial;
    }


}
