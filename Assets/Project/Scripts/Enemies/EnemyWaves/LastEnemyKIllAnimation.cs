using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class LastEnemyKIllAnimation : MonoBehaviour
{
    [Header("SLOW MOTION")]
    [SerializeField] float animationTime;
    [SerializeField] AnimationCurve animationCurve;

    [Header("SCREEN SPLITTER")]
    [SerializeField] private GameObject p_ScreenSpliter;
    [SerializeField] private GameObject p_Explosion;
    [SerializeField] private GameObject p_Particles;
    [SerializeField] private MaterialLerp.FloatData screenSpliterMatLerp;
    [SerializeField] private MaterialLerp.FloatData explosionMatLerp;
    private GameObject screenSpliter;
    private GameObject explosion;
    private GameObject particles;

    public IEnumerator StartAnimation(Vector3 lastEnemyPos)
    {
        GameAudioManager.GetInstance().PlayEnemyLastDeathHit();
        StartCoroutine(Particles(lastEnemyPos));

        RaycastHit hit;
        if (Physics.Raycast(lastEnemyPos, Vector3.down, out hit, 10.0f))
        {
            StartCoroutine(hit.transform.gameObject.GetComponent<PathTile>().Deactivate());
            StartCoroutine(hit.transform.gameObject.GetComponent<PathTile>().Animation());
            //StartCoroutine(hit.transform.gameObject.GetComponent<PathTile>().FallDown());
        }

        yield return null;

        Time.timeScale = 0.0f;
        float currentTime = 0.0f;

        //StartCoroutine(Particles(lastEnemyPos));
        //StartCoroutine(Explosion(lastEnemyPos));
        yield return new WaitForSecondsRealtime(0.2f);
        
        while (currentTime < animationTime)
        {
            currentTime += Time.unscaledDeltaTime;

            yield return null;
        }

        Time.timeScale = 1.0f;
    }

    private IEnumerator Explosion(Vector3 lastEnemyPos)
    {

        explosion = Instantiate(p_Explosion, transform);
        explosion.transform.position = lastEnemyPos;

        yield return new WaitForSeconds(0.5f);
        Camera.main.gameObject.GetComponent<CameraMovement>().CameraShake();

        StartCoroutine(MaterialLerp.FloatLerp(explosionMatLerp, explosion.gameObject.GetComponent<MeshRenderer>().materials));
    }

    private IEnumerator ScreenSplitter(Vector3 lastEnemyPos)
    {

        screenSpliter = Instantiate(p_Particles, transform);
        screenSpliter.transform.position = lastEnemyPos;
        Vector3 centerPos = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width / 2, Screen.height / 2));

        yield return new WaitForSeconds(0.5f);

        StartCoroutine(MaterialLerp.FloatLerp(screenSpliterMatLerp, screenSpliter.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().materials));
    }

    private IEnumerator Particles(Vector3 lastEnemyPos)
    {
        particles = Instantiate(p_Particles, transform);
        particles.transform.position = lastEnemyPos;
        yield return new WaitForSeconds(0.5f);

    }
}
