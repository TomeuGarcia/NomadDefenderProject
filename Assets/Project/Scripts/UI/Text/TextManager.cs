using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TextManager : MonoBehaviour
{
    [SerializeField] private List<TextDecoder> texts = new List<TextDecoder>();
    [SerializeField] private float startDelay;
    [SerializeField] private float delay;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DecodeTexts());
    }

    IEnumerator DecodeTexts()
    {
        yield return new WaitForSeconds(startDelay);

        StartCoroutine(LinearDecode());
        //StartCoroutine(SimultaneousDecode(delay));
    }

    IEnumerator SimultaneousDecode(float delay)
    {
        foreach (TextDecoder text in texts)
        {
            text.Activate();
            yield return new WaitForSeconds(delay);
        }
    }

    IEnumerator LinearDecode()
    {
        foreach (TextDecoder text in texts)
        {
            text.Activate();
            yield return new WaitUntil(() => text.IsDoneDecoding() == true);
            yield return new WaitForSeconds(0.6f);
        }
    }

    public void SetTextDecoders(List<TextDecoder> newTextDecoders)
    {
        texts = newTextDecoders;
    }
}
