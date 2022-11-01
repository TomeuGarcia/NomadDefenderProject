using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class TileMap : MonoBehaviour
{
    const int max = 30;

    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Transform tilesTransform;
    [SerializeField, Range(2, max)] private int resolution;
    private int resolutionSquared;

    private GameObject[] tileBuffer = null;


    private void OnValidate()
    {
        GenerateMap();
    }

    private void Awake()
    {
        GenerateMap();
    }


    private void GenerateMap()
    {
        if (tileBuffer == null || tileBuffer[0] == null)
        {
            tileBuffer = new GameObject[max * max];
            for (int i = 0; i < max * max; ++i)
            {
                tileBuffer[i] = Instantiate(tilePrefab, tilesTransform);
                tileBuffer[i].SetActive(false);
            }
        }


        resolutionSquared = resolution * resolution;

        float halfResolution = (float)resolution / 2.0f;
        float offset = (-halfResolution + 0.5f);
        Vector3 positionXZOffset = (offset * Vector3.right) + (offset * Vector3.forward) + tilesTransform.position;

        GameObject currentTile;
        for (int i = 0; i < resolution; ++i)
        {
            for (int j = 0; j < resolution; j++)
            {
                currentTile = tileBuffer[(i * resolution) + j];
                
                currentTile.SetActive(true);

                float y = Mathf.PerlinNoise((float)i / resolution, (float)j / resolution);
                y += Random.Range(-0.2f, 0.2f);
                currentTile.transform.position = new Vector3(i, y, j) + positionXZOffset;
            }
        }

        int count = resolutionSquared;
        while (tileBuffer[count].activeInHierarchy)
        {
            tileBuffer[count].SetActive(false);
            ++count;
        }

    }

}
