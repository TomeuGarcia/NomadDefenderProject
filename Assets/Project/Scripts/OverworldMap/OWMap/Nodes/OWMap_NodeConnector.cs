using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public class OWMap_NodeConnector : MonoBehaviour
{
    [SerializeField] private List<GameObject> cableConnectionPrefabs = new List<GameObject>();

    public void SetConnectionTypes(int[] connectionTypes)
    {
        foreach(int type in connectionTypes)
        {
            GameObject cable = Instantiate(cableConnectionPrefabs[Mathf.Abs(type)]);
            cable.transform.SetParent(gameObject.transform, false);

            if (type > 0)
            {
                cable.transform.localPosition = new Vector3(-cable.transform.localPosition.x, cable.transform.localPosition.y, cable.transform.localPosition.z);
                cable.transform.rotation = Quaternion.Euler(cable.transform.rotation.x, cable.transform.rotation.y, cable.transform.rotation.z + 180f);
            }
        }
    }
}
