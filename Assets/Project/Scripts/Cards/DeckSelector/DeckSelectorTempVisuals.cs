using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckSelectorTempVisuals : MonoBehaviour
{
    [SerializeField] private GameObject lighting;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ActivateLighting());
    }

    private IEnumerator ActivateLighting()
    {
        yield return new WaitForSeconds(0.1f);
        lighting.SetActive(true);
    }
}
