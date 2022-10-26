using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathLocation : MonoBehaviour
{
    private int lives = 2;
    public bool isDead => lives <= 0;


    public void TakeDamage()
    {
        --lives;
    }
}
