using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using static UnityEngine.ParticleSystem;

public class LastEnemyKIllAnimation : MonoBehaviour
{
    [HideInInspector] public static LastEnemyKIllAnimation instance { get; private set; }

    [SerializeField] private bool _isBossFight = false;

    [Header("SLOW MOTION")]
    [SerializeField] float animationTime;
    [SerializeField] AnimationCurve animationCurve;

    [Header("SCREEN SPLITTER")]
    [SerializeField] private Material lostMaterial;
    [SerializeField] private GameObject p_Particles;
    [SerializeField] private GameObject p_screenFlash;
    [SerializeField] private GameObject p_flashingLight;
    private GameObject flashingLight;
    private GameObject particles;

    private Vector3[] _tileCheckOffsets;

    private PathTile[] _pathTiles;

    public delegate void LastEnemyKIllAnimationAction();
    public static event LastEnemyKIllAnimationAction OnQueryResumeTimescale;


    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }


        //TOTEST
        //_isBoss = true;

        if (_isBossFight)
        {
            PathTile[] _pathTiles = Resources.FindObjectsOfTypeAll<PathTile>();

            for(int i = 0; i < _pathTiles.Length; i++)
            {
                _pathTiles[i].InitBoss();
            }
        }
    }

    public void DeathAnimation(Vector3 lastEnemyPos, bool deactivateTiles)
    {
        StartCoroutine(StartAnimation(lastEnemyPos, true, deactivateTiles));
    }

    public void FinalKillAnimation(Vector3 lastEnemyPos)
    {
        if (_isBossFight)
        {
            StartCoroutine(StartBossAnimation(lastEnemyPos));
        }
        else
        {
            StartCoroutine(StartAnimation(lastEnemyPos));
        }
    }

    public IEnumerator StartAnimation(Vector3 lastEnemyPos, bool lost = false, bool deactivateTiles = true)
    {

        bool doAnimation = false;
        Vector3 tilePos = Vector3.zero;
        RaycastHit[] allHits;
        allHits = Physics.RaycastAll(lastEnemyPos + Vector3.up * 10.0f, Vector3.down, 100.0f);
        foreach(RaycastHit hit in allHits)
        {
            if (hit.transform.gameObject.GetComponent<PathTile>() != null)
            {
                tilePos = new Vector3(hit.transform.position.x, 0.4f, hit.transform.position.z);
                StartCoroutine(hit.transform.gameObject.GetComponent<PathTile>().Deactivate());
                StartCoroutine(hit.transform.gameObject.GetComponent<PathTile>().Animation());
                doAnimation = true;

                break;
            }
            else if(hit.transform.gameObject.GetComponent<PathLocation>() != null)
            {
                tilePos = new Vector3(hit.transform.GetChild(0).position.x, 0.4f, hit.transform.GetChild(0).position.z);
                if(deactivateTiles)
                {
                    hit.transform.gameObject.GetComponent<PathLocation>().Deactivate();
                    StartCoroutine(hit.transform.gameObject.GetComponent<PathLocation>().Animation(lost));
                }
                doAnimation = true;
                break;
            }
        }
        
        if(doAnimation)
        {
            GameAudioManager.GetInstance().PlayEnemyLastDeathHit();
            //Debug.Log("Last Death Tile Pos - " + tilePos);
            StartCoroutine(Particles(tilePos, lost));

            yield return null;

            StartCoroutine(FlashingLight(tilePos, lost));
            StartCoroutine(CameraShake());
            StartCoroutine(ScreenFlash());
            yield return StartCoroutine(PlayHitStop());
        }
        if (OnQueryResumeTimescale != null) OnQueryResumeTimescale();
    }

    public IEnumerator PlayHitStop()
    {
        Time.timeScale = 0.0f;
        float currentTime = 0.0f;
        yield return new WaitForSecondsRealtime(0.0f);


        while (currentTime < animationTime)
        {
            currentTime += Time.unscaledDeltaTime;
            Time.timeScale = animationCurve.Evaluate(currentTime / animationTime);

            yield return null;
        }
    }

    public IEnumerator PlayHitStopBoss()
    {
        Time.timeScale = 0.0f;
        float currentTime = 0.0f;
        yield return new WaitForSecondsRealtime(0.0f);

        //float bossAnimationTime = 0.1f;
        while (currentTime < animationTime)
        {
            currentTime += Time.unscaledDeltaTime;
            Time.timeScale = animationCurve.Evaluate(currentTime / animationTime);

            yield return null;
        }
    }

    private IEnumerator FlashingLight(Vector3 tilePos, bool lost = false)
    {
        flashingLight = Instantiate(p_flashingLight, transform);
        flashingLight.transform.localPosition = tilePos + Vector3.up * 10;
        if(lost)
        {
            flashingLight.gameObject.GetComponent<Light>().color = new Color32(191, 0, 0, 255);
        }
        flashingLight.gameObject.GetComponent<Light>().DOIntensity(0.0f, 0.25f);
        //yield return new WaitForSeconds(0.25f);
        yield return new WaitForSeconds(0.1f);
        flashingLight.gameObject.GetComponent<Light>().DOIntensity(80.0f, 0.5f);
        yield return new WaitForSeconds(0.75f);
        flashingLight.gameObject.GetComponent<Light>().DOIntensity(0.0f, 1.0f);
    }

    private IEnumerator FlashingLightBoss(Vector3 tilePos, bool lost = false)
    {
        flashingLight = Instantiate(p_flashingLight, transform);


        GameObject flashingLight0 = Instantiate(p_flashingLight, transform);
        GameObject flashingLight1 = Instantiate(p_flashingLight, transform);
        GameObject flashingLight2 = Instantiate(p_flashingLight, transform);
        GameObject flashingLight3 = Instantiate(p_flashingLight, transform);
        flashingLight0.SetActive(false);
        flashingLight1.SetActive(false);
        flashingLight2.SetActive(false);
        flashingLight3.SetActive(false);

        flashingLight.transform.localPosition = tilePos + Vector3.up * 10;

        if (lost)
        {
            flashingLight.gameObject.GetComponent<Light>().color = new Color32(191, 0, 0, 255);
        }
        flashingLight.gameObject.GetComponent<Light>().DOIntensity(0.0f, 0.25f);
        //yield return new WaitForSeconds(0.25f);
        yield return new WaitForSeconds(0.1f);
        flashingLight.gameObject.GetComponent<Light>().DOIntensity(80.0f, 0.5f);
        yield return new WaitForSeconds(1.65f);

        flashingLight0.transform.localPosition = tilePos + Vector3.up * 20 + Vector3.right * 20;
        flashingLight1.transform.localPosition = tilePos + Vector3.up * 20 + Vector3.left * 20;
        flashingLight2.transform.localPosition = tilePos + Vector3.up * 20 + Vector3.forward * 20;
        flashingLight3.transform.localPosition = tilePos + Vector3.up * 20 + Vector3.back * 20;

        flashingLight0.gameObject.GetComponent<Light>().intensity = 0.0f;
        flashingLight1.gameObject.GetComponent<Light>().intensity = 0.0f;
        flashingLight2.gameObject.GetComponent<Light>().intensity = 0.0f;
        flashingLight3.gameObject.GetComponent<Light>().intensity = 0.0f;
        flashingLight0.SetActive(true);
        flashingLight1.SetActive(true);
        flashingLight2.SetActive(true);
        flashingLight3.SetActive(true);

        flashingLight.gameObject.GetComponent<Light>().DOIntensity(10000.0f, 2.5f).SetEase(Ease.Linear);

        flashingLight0.gameObject.GetComponent<Light>().DOIntensity(10000.0f, 2.5f).SetEase(Ease.Linear);
        flashingLight1.gameObject.GetComponent<Light>().DOIntensity(10000.0f, 2.5f).SetEase(Ease.Linear);
        flashingLight2.gameObject.GetComponent<Light>().DOIntensity(10000.0f, 2.5f).SetEase(Ease.Linear);
        flashingLight3.gameObject.GetComponent<Light>().DOIntensity(10000.0f, 2.5f).SetEase(Ease.Linear);
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

    private IEnumerator CameraShakeBoss()
    {
        Camera.main.gameObject.GetComponent<CameraMovement>().CameraShake(0.1f, 30);
        yield return new WaitForSeconds(0.5f);
        Camera.main.gameObject.GetComponent<CameraMovement>().CameraShake(10.0f, 125);
    }

    private IEnumerator Particles(Vector3 spawnPos, bool lost = false)
    {
        particles = Instantiate(p_Particles, transform);
        particles.transform.position = spawnPos;
        if(lost)
        {
            particles.gameObject.GetComponent<ParticleSystemRenderer>().material = lostMaterial;
            particles.transform.GetChild(0).gameObject.GetComponent<LineRenderer>().material = lostMaterial;
            particles.transform.GetChild(1).gameObject.GetComponent<ParticleSystemRenderer>().material = lostMaterial;
        }
        yield return new WaitForSeconds(0.25f);
        //yield return new WaitForSeconds(0);
        particles.transform.GetChild(0).gameObject.GetComponent<BlastWave>().Activate();
    }

    public IEnumerator StartBossAnimation(Vector3 lastEnemyPos, bool lost = false, bool deactivateTiles = true)
    {
        bool doAnimation = false;
        Vector3 tilePos = Vector3.zero;
        RaycastHit[] allHits;
        allHits = Physics.RaycastAll(lastEnemyPos + Vector3.up * 10.0f, Vector3.down, 100.0f);
        foreach (RaycastHit hit in allHits)
        {
            if (hit.transform.gameObject.GetComponent<PathTile>() != null)
            {
                tilePos = new Vector3(hit.transform.position.x, 0.4f, hit.transform.position.z);
                StartCoroutine(hit.transform.gameObject.GetComponent<PathTile>().Deactivate());
                StartCoroutine(hit.transform.gameObject.GetComponent<PathTile>().BossAnimation());
                doAnimation = true;

                break;
            }
            else if (hit.transform.gameObject.GetComponent<PathLocation>() != null)
            {
                tilePos = new Vector3(hit.transform.GetChild(0).position.x, 0.4f, hit.transform.GetChild(0).position.z);
                if (deactivateTiles)
                {
                    hit.transform.gameObject.GetComponent<PathLocation>().Deactivate();
                    StartCoroutine(hit.transform.gameObject.GetComponent<PathLocation>().Animation(lost));
                }
                doAnimation = true;
                break;
            }
        }

        if (doAnimation)
        {
            GameAudioManager.GetInstance().PlayEnemyLastDeathHit();
            //Debug.Log("Last Death Tile Pos - " + tilePos);
            StartCoroutine(Particles(tilePos, lost));

            yield return null;

            StartCoroutine(FlashingLightBoss(tilePos, lost));
            StartCoroutine(CameraShakeBoss());
            StartCoroutine(ScreenFlash());
            yield return StartCoroutine(PlayHitStopBoss());
        }
        if (OnQueryResumeTimescale != null) OnQueryResumeTimescale();
    }

    private void OnDestroy()
    {
        Camera.main.gameObject.GetComponent<CameraMovement>().CameraShake(0.1f, 10);
    }
}
