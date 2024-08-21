using System;
using UnityEngine;

public class TurretAbilityFactoryInstaller : MonoBehaviour
{
    private void Start()
    {
        ServiceLocator.GetInstance().TurretAbilityFactory = new TurretAbilityFactory();
    }
}