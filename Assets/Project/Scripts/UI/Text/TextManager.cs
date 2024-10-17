using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TextManager : MonoBehaviour
{
    [SerializeField] private List<TextDecoder> texts = new List<TextDecoder>();
    [SerializeField] private float startDelay;
    [SerializeField] private float delay;
    bool secondTime = false;
    // Start is called before the first frame update
    void Start()
    {
        secondTime = true;
        StartCoroutine(DecodeTextsWithDelay());
    }

    public IEnumerator DecodeTextsWithDelay()
    {
        yield return new WaitForSecondsRealtime(startDelay);
        DecodeTexts();
    }

    public void DecodeTexts()
    {
        foreach (TextDecoder text in texts)
        {
            text.ClearDecoder();
        }
        StartCoroutine(LinearDecode());
    }

    IEnumerator SimultaneousDecode(float delay)
    {
        foreach (TextDecoder text in texts)
        {
            text.Activate();
            yield return new WaitForSeconds(delay);
        }
    }
    public void ResetTexts()
    {
        foreach (TextDecoder text in texts)
        {
            text.ClearDecoder();
        }
    }

    IEnumerator LinearDecode()
    {
        foreach (TextDecoder text in texts)
        {
            if (text.isActiveAndEnabled)
            {
                text.Activate();
                yield return new WaitUntil(() => text.IsDoneDecoding() == true);
                yield return new WaitForSecondsRealtime(delay);
            }
        }
    }

    public void SetTextDecoders(List<TextDecoder> newTextDecoders)
    {
        texts = newTextDecoders;
    }
}
