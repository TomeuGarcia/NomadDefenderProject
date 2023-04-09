using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using static UnityEngine.ParticleSystem;

public class LastEnemyKIllAnimation : MonoBehaviour
{
    [Header("SLOW MOTION")]
    [SerializeField] float animationTime;
    [SerializeField] AnimationCurve animationCurve;

    [Header("SCREEN SPLITTER")]
    [SerializeField] private GameObject p_Particles;
    [SerializeField] private GameObject p_screenFlash;
    [SerializeField] private GameObject p_flashingLight;
    private GameObject flashingLight;
    private GameObject particles;

    public IEnumerator StartAnimation(Vector3 lastEnemyPos)
    {
        Vector3 tilePos;
        RaycastHit hit;
        Physics.Raycast(lastEnemyPos, Vector3.down, out hit, 10.0f);
        tilePos = new Vector3(hit.transform.position.x, lastEnemyPos.y, hit.transform.position.z);
        StartCoroutine(hit.transform.gameObject.GetComponent<PathTile>().Deactivate());
        StartCoroutine(hit.transform.gameObject.GetComponent<PathTile>().Animation());

        GameAudioManager.GetInstance().PlayEnemyLastDeathHit();
        StartCoroutine(Particles(tilePos));

        yield return null;

        StartCoroutine(FlashingLight(tilePos));
        StartCoroutine(CameraShake());
        StartCoroutine(ScreenFlash());
        Time.timeScale = 0.0f;
        float currentTime = 0.0f;
        yield return new WaitForSecondsRealtime(0.0f);

        
        while (currentTime < animationTime)
        {
            currentTime += Time.unscaledDeltaTime;
            Time.timeScale = animationCurve.Evaluate(currentTime / animationTime);

            yield return null;
        }

        Time.timeScale = 1.0f;
    }

    private IEnumerator FlashingLight(Vector3 tilePos)
    {
        flashingLight = Instantiate(p_flashingLight, transform);
        flashingLight.transform.localPosition = tilePos + Vector3.up * 10;
        flashingLight.gameObject.GetComponent<Light>().DOIntensity(0.0f, 0.25f);
        //yield return new WaitForSeconds(0.25f);
        yield return new WaitForSeconds(0.1f);
        flashingLight.gameObject.GetComponent<Light>().DOIntensity(80.0f, 0.5f);
        yield return new WaitForSeconds(0.75f);
        flashingLight.gameObject.GetComponent<Light>().DOIntensity(0.0f, 1.0f);
    }

    private IEnumerator ScreenFlash()
    {
        GameObject screenFlash = Instantiate(p_screenFlash, transform);
        screenFlash.gameObject.GetComponent<ScreenFlash>().FadeIn(0.1f);
        yield return new WaitForSeconds(0.1f);
        screenFlash.gameObject.GetComponent<ScreenFlash>().FadeOut(0.75f);
    }

    private IEnumerator CameraShake()
    {
        Camera.main.gameObject.GetComponent<CameraMovement>().CameraShake(0.1f, 30);
        yield return new WaitForSeconds(0.5f);
        Camera.main.gameObject.GetComponent<CameraMovement>().CameraShake(0.5f, 50);
        yield return new WaitForSeconds(0.5f);
        Camera.main.gameObject.GetComponent<CameraMovement>().CameraShake(0.5f, 10);
    }

    private IEnumerator Particles(Vector3 spawnPos)
    {
        particles = Instantiate(p_Particles, transform);
        particles.transform.position = spawnPos;
        yield return new WaitForSeconds(0.25f);
        //yield return new WaitForSeconds(0);
        particles.transform.GetChild(0).gameObject.GetComponent<BlastWave>().Activate();
    }
}
