using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocationHealthItem : MonoBehaviour
{
    private static Color holderImageColor = new Color(0.2f, 0.2f, 0.2f);
    [SerializeField] private Image holderImage;
    [SerializeField] private Image frontImage;


    public void TakeDamage()
    {
        StartCoroutine(TakeDamageAnimation());
    }

    private IEnumerator TakeDamageAnimation()
    {
        holderImage.color = Color.red;
        float t1 = 0.1f;
        float t2 = 0.2f;

        for (int i = 0; i < 2; ++i)
        {
            frontImage.DOFade(0f, t1).OnComplete(() => { frontImage.DOFade(1f, t1); });
            yield return new WaitForSeconds(t2);
        }
        frontImage.DOFade(0f, t1);
        yield return new WaitForSeconds(t1);

        holderImage.DOBlendableColor(holderImageColor, t1);
    }


    public void GainHealth()
    {
        StartCoroutine(GainHealthAnimation());
    }

    private IEnumerator GainHealthAnimation()
    {
        holderImage.color = Color.green;
        float t1 = 0.1f;
        float t2 = 0.2f;

        for (int i = 0; i < 2; ++i)
        {
            frontImage.DOFade(1f, t1).OnComplete(() => { frontImage.DOFade(0f, t1); });
            yield return new WaitForSeconds(t2);
        }
        frontImage.DOFade(1f, t1);
        yield return new WaitForSeconds(t1);

        holderImage.DOBlendableColor(holderImageColor, t1);
    }

}
