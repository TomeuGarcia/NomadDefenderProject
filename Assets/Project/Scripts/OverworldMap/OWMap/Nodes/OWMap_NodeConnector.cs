using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public class OWMap_NodeConnector : MonoBehaviour
{
    [SerializeField] private List<GameObject> sockets = new List<GameObject>();
    [SerializeField] private List<GameObject> cableConnectionPrefabs = new List<GameObject>();

    public void SetConnectionTypes(int[] connectionTypes)
    {
        foreach(int type in connectionTypes)
        {
            GameObject cable = Instantiate(cableConnectionPrefabs[type]); //FINISH
            //POSITION IT ACCORDINGLY

            if(type < 0)
            {
                //INVERT
                //ALSO INVERT POS
            }
        }
    }
}
