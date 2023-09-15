using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OWMapVisualEventManager : MonoBehaviour
{
    [SerializeField] private BrokenKeepOutWall brokenKeepOutWall;
    [SerializeField] private DarknessFog darknessFog;

    private void Start()
    {
        StartCoroutine(IntroSequence());
    }

    public IEnumerator IntroSequence()
    {
        yield return new WaitForSeconds(1.5f);

        yield return darknessFog.Dissipate();
        yield return brokenKeepOutWall.InitialBlink();
    }

    public void TurnOffKeepOutWall()
    {
        StartCoroutine(brokenKeepOutWall.InitialBlink());
    }

    public void DissipateDarkFog()
    {
        StartCoroutine(darknessFog.Dissipate());
    }
}
