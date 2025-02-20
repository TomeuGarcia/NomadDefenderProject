using NaughtyAttributes;
using UnityEngine;

public class DisableCannonsShootingTester : MonoBehaviour
{
    [Header("SHOOT LOGIC")]
    [SerializeField] private DisableCannonsShootLogic _shootLogic;

    [Header("TESTING")] 
    [SerializeField] private DisableCannonsShootLogic.ShootingFunctions _shootingFunction;
    [SerializeField, Min(1)] private int _numberOfShots = 3;


    [Button()]
    private void TestShooting()
    {
        _shootLogic.Shoot(_shootingFunction, _numberOfShots);
    }
}