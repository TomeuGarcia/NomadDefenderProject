using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool : MonoBehaviour
{
    [SerializeField]
    private GameObject pooledObject;
    private bool missingObjects = true;

    private List<GameObject> objects;


    void Start()
    {
        objects = new List<GameObject>();
    }

    public GameObject GetObject()
    {
        if(objects.Count > 0)
        {
            for(int i = 0; i < objects.Count; i++)
            {
                if(!objects[i].activeInHierarchy)
                {
                    return objects[i];
                }
            }
        }

        if(missingObjects)
        {
            GameObject obj = Instantiate(pooledObject);
            obj.SetActive(false);
            objects.Add(obj);
            return obj;
        }

        return null;
    }

    public GameObject GetObject(Vector3 position, Quaternion rotation)
    {
        if(objects.Count > 0)
        {
            for(int i = 0; i < objects.Count; i++)
            {
                if(!objects[i].activeInHierarchy)
                {
                    objects[i].gameObject.transform.position = position;
                    objects[i].gameObject.transform.rotation = rotation;
                    return objects[i];
                }
            }
        }

        if(missingObjects)
        {
            GameObject obj = Instantiate(pooledObject);
            obj.SetActive(false);

            obj.gameObject.transform.position = position;
            obj.gameObject.transform.rotation = rotation;

            objects.Add(obj);
            return obj;
        }

        return null;
    }
}
