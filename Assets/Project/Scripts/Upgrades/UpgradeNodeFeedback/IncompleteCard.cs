using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncompleteCard : MonoBehaviour
{
    [SerializeField] private List<TextDecoder> decoders = new List<TextDecoder>();

    IEnumerator Start()
    {
        DeactivateAllDecoders();
        yield return new WaitForSeconds(0.5f);
        ActivateAllDecoders();
    }

    private void OnEnable()
    {
        ActivateAllDecoders();
    }

    private void DeactivateAllDecoders()
    {
        foreach (TextDecoder d in decoders)
        {
            d.gameObject.SetActive(false);
        }
    }

    private void ActivateAllDecoders()
    {
        foreach (TextDecoder d in decoders)
        {
            d.gameObject.SetActive(true);
            d.Activate();
        }
    }
}
