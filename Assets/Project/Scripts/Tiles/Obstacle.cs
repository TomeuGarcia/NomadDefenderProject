using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private void Awake()
    {
        RaycastHit hit;

        if(Physics.Raycast(gameObject.transform.position + (Vector3.up * 0.5f), Vector3.down, out hit, Mathf.Infinity))
        {
            hit.transform.gameObject.GetComponent<Tile>().isOccupied = true;
        }
    }
}
