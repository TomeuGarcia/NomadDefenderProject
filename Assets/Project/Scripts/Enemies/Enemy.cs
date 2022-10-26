using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] public PathFollower pathFollower;
    [SerializeField] public Transform transformToMove;


    private void OnEnable()
    {
        pathFollower.OnPathEndReached += Attack;
    }

    private void OnDisable()
    {
        pathFollower.OnPathEndReached -= Attack;
    }


    private void Attack()
    {
        Debug.Log("Attack");
    }



}
