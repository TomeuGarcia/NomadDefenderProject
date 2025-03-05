using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OWMapVisualEventManager : MonoBehaviour
{
    [SerializeField] private BrokenKeepOutWall[] brokenKeepOutWalls;
    [SerializeField] private DarknessFog darknessFog;

    private void Start()
    {
        StartCoroutine(IntroSequence());
    }

    public IEnumerator IntroSequence()
    {
        yield return new WaitForSeconds(1.5f);

        yield return darknessFog.Dissipate();
        for (int i = 0; i < brokenKeepOutWalls.Length; ++i)
        {
            if (i == 0)
            {
                yield return StartCoroutine(brokenKeepOutWalls[i].InitialBlink());
            }
            else
            {
                StartCoroutine(brokenKeepOutWalls[i].InitialBlink());
            }
        }
    }

    public void TurnOffKeepOutWall()
    {
        for (int i = 0; i < brokenKeepOutWalls.Length; ++i)
        {
            StartCoroutine(brokenKeepOutWalls[i].InitialBlink());
        }
    }

    public void DissipateDarkFog()
    {
        StartCoroutine(darknessFog.Dissipate());
    }
}
