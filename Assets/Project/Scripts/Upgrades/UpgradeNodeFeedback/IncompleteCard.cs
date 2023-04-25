using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncompleteCard : MonoBehaviour
{
    [SerializeField] private List<TextDecoder> decoders = new List<TextDecoder>();

    private void OnEnable()
    {
        foreach(TextDecoder d in decoders)
        {
            d.Activate();
        }
    }
}
