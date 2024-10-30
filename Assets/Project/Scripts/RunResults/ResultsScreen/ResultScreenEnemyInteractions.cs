using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ResultScreenEnemyInteractions : MonoBehaviour
{
    private bool _hasBeenInitialized = false;
    private Enemy _enemy;
    private int _damageToDeal;
    
    public void Init(Enemy enemy, int damageToDeal)
    {
        _hasBeenInitialized = true;
        _enemy = enemy;
        _damageToDeal = damageToDeal;
    }

    private void OnMouseDown()
    {
        if (!_hasBeenInitialized)
        {
            return;
        }

        if (_enemy.IsDead() || !_enemy.gameObject.activeInHierarchy)
        {
            return;
        }
        
        _enemy.TakeDamage(new TurretDamageAttack(null, _enemy, _damageToDeal), OnDamageDealtToEnemy);
        GameAudioManager.GetInstance().PlayCardInfoHidden();
    }

    private void OnDamageDealtToEnemy(TurretDamageAttackResult result)
    {
        if (_enemy.IsDead())
        {
            StartCoroutine(ReviveEnemy());
        }
    }

    private IEnumerator ReviveEnemy()
    {
        yield return new WaitForSeconds(2.0f);
        
        _enemy.gameObject.SetActive(true);
        _enemy.InitWithoutFunctionality();

        yield return null;
        _enemy.MeshTransform.DOPunchScale(Vector3.one * 0.1f, 0.5f, 2);
        
        GameAudioManager.GetInstance().PlayCardInfoShown();
    }
}
