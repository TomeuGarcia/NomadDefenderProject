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
    [SerializeField] private GameObject p_Particles;
    [SerializeField] private MaterialLerp.FloatData screenSpliterMatLerp;
    [SerializeField] private MaterialLerp.FloatData explosionMatLerp;
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

        Time.timeScale = 0.0f;
        float currentTime = 0.0f;
        yield return new WaitForSecondsRealtime(0.2f);
        
        while (currentTime < animationTime)
        {
            currentTime += Time.unscaledDeltaTime;
            Time.timeScale = animationCurve.Evaluate(currentTime / animationTime);

            yield return null;
        }

        Time.timeScale = 1.0f;
    }

    private IEnumerator Particles(Vector3 spawnPos)
    {
        particles = Instantiate(p_Particles, transform);
        particles.transform.position = spawnPos;
        yield return new WaitForSeconds(0.25f);
        particles.transform.GetChild(0).gameObject.GetComponent<BlastWave>().Activate();
    }
}
