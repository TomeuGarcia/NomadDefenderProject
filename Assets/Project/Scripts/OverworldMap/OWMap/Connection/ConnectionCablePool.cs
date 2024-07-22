using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ConnectionCableLibrary", menuName = "Map/ConnectionCableLibrary")]
public class ConnectionCablePool : ScriptableObject
{
    [System.Serializable]
    public class CablePool
    {
        public string name;
        public List<GameObject> pool = new List<GameObject>();
    }

    [SerializeField] private List<CablePool> pool = new List<CablePool>();

    public GameObject GetConnectionCable(int typeIndex)
    {
        return Instantiate(pool[typeIndex].pool[Random.Range(0, pool[typeIndex].pool.Count - 1)]);
    }
}
