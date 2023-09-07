using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.ProBuilder.Shapes;
using Unity.VisualScripting;

public class PathLocation : MonoBehaviour
{
    [Header("HEALTH")]
    [SerializeField] private int health = 3;
    public HealthSystem healthSystem { get; private set; }
    private bool hasGameFinished;

    [Header("HUD")]
    [SerializeField] private LocationHealthHUD healthHUD;

    [Header("NODE VISUALS")]
    [SerializeField] private MeshRenderer nodeMesh;
    [SerializeField] Material lostMaterial;
    [SerializeField] GameObject animationCube;
    private Material nodeMeshMaterial;
    [SerializeField] private MeshRenderer healFlashMesh;
    private Material healFlashMaterial;


    [Header("MESH HOLDER")]
    [SerializeField] private Transform locationMeshHolder;


    public Vector3 Position => transform.position;


    [System.Serializable]
    private struct DamagedVisuals
    {
        public NodeEnums.HealthState healthState;
        public ParticleSystem takeDamageParticles;

        public void Activate()
        {
            takeDamageParticles.gameObject.SetActive(true);
        }
        public void Deactivate()
        {
            takeDamageParticles.gameObject.SetActive(false);
        }
    }
    [Header("DAMAGED VISUALS")]
    [SerializeField] private DamagedVisuals[] damagedVisualsList;
    [SerializeField] private Dictionary<NodeEnums.HealthState, DamagedVisuals> mappedDamagedVisuals;


    public bool IsDead => healthSystem.IsDead();


    public delegate void PathLocationAction(PathLocation thisPathLocation);
    public delegate void PathLocationAction2();
    public event PathLocationAction OnDeath;
    public static event PathLocationAction OnTakeDamage;
    public static event PathLocationAction2 OnHealthChanged;


    private void Awake()
    {
        healthSystem = new HealthSystem(health);
        healthHUD.Init(healthSystem);
        InitParticles();

        hasGameFinished = false;

        healFlashMaterial = healFlashMesh.material;
    }

    private void OnEnable()
    {
        EnemyWaveManager.OnAllWavesFinished += SetHasGameFinished;
    }
    private void OnDisable()
    {
        EnemyWaveManager.OnAllWavesFinished -= SetHasGameFinished;
    }

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.D) && !IsDead) TakeDamage(1);
    //}

    private void SetHasGameFinished()
    {
        hasGameFinished = true;
    }

    public bool CanTakeDamage()
    {
        return !IsDead && !hasGameFinished;
    }

    public void TakeDamage(int damageAmount)
    {
        healthSystem.TakeDamage(damageAmount);

        SetDamagedVisuals();
        locationMeshHolder.DOComplete();

        if (healthSystem.IsDead())
        {
            Die();
            locationMeshHolder.DOComplete();
            locationMeshHolder.DOPunchScale(new Vector3(1f, 0f, 1f) * 0.3f, 0.6f, 8);
            GameAudioManager.GetInstance().PlayLocationDestroyed();
        }
        else
        {
            locationMeshHolder.DOComplete();
            locationMeshHolder.DOPunchScale(new Vector3(1f, 0f, 1f) * 0.4f, 0.9f, 8);
            GameAudioManager.GetInstance().PlayLocationTakeDamage();
        }

        if (OnTakeDamage != null) OnTakeDamage(this);
        if (OnHealthChanged != null) OnHealthChanged();
    }

    public void Heal(int healAmount)
    {        
        healthSystem.Heal(healAmount);

        SetDamagedVisuals();
        locationMeshHolder.DOComplete();
        locationMeshHolder.DOPunchScale(new Vector3(0.3f, 1f, 0.3f) * 0.3f, 0.6f, 8);

        
        GameAudioManager.GetInstance().PlayLocationHealed();
        healFlashMaterial.SetFloat("_StartTimeFlashAnimation", Time.time);

        if (OnHealthChanged != null) OnHealthChanged();
    }

    private void Die()
    {
        if (OnDeath != null) OnDeath(this);
    }


    public void InitNodeVisuals(Texture nodeIconTexture, Color borderColor)
    {
        nodeMeshMaterial = nodeMesh.material;

        nodeMeshMaterial.SetFloat("_TimeOffset", Random.Range(0f, 1f));
        nodeMeshMaterial.SetColor("_IconColor", OWMapDecoratorUtils.s_darkGreyColor);
        nodeMeshMaterial.SetFloat("_IsInteractable", 0f);
        nodeMeshMaterial.SetFloat("_NoiseTwitchingEnabled", 0f);
        nodeMeshMaterial.SetFloat("_IsDamaged", 0f);
        nodeMeshMaterial.SetFloat("_IsDestroyed", 0f);

        nodeMeshMaterial.SetFloat("_FadeDuration", OWMap_Node.FADE_DURATION);
        nodeMeshMaterial.SetFloat("_TimeStartFade", 0f);
        nodeMeshMaterial.SetFloat("_IsFadingAway", 0f);

        nodeMeshMaterial.SetFloat("_SelectedDuration", OWMap_Node.SELECTED_DURATION);
        nodeMeshMaterial.SetFloat("_IsSelected", 0f);
        nodeMeshMaterial.SetFloat("_TimeStartSelected", 0f);


        nodeMeshMaterial.SetTexture("_IconTexture", nodeIconTexture);
        nodeMeshMaterial.SetColor("_BorderColor", borderColor);

        nodeMeshMaterial.SetFloat("_NormalBorderMoveSpeed", OWMap_Node.DEFAULT_BORDER_MOVE_SPEED);
        nodeMeshMaterial.SetFloat("_FastBorderMoveSpeed", OWMap_Node.SELECTED_BORDER_MOVE_SPEED);
        nodeMeshMaterial.SetFloat("_FastBorderDuration", OWMap_Node.SELECTED_BORDER_MOVE_DURATION);
        nodeMeshMaterial.SetFloat("_DoFastBorder", 0f);
    }


    private void InitParticles()
    {
        mappedDamagedVisuals = new Dictionary<NodeEnums.HealthState, DamagedVisuals>();
        foreach (DamagedVisuals damagedVisuals in damagedVisualsList)
        {
            damagedVisuals.Deactivate();
            mappedDamagedVisuals[damagedVisuals.healthState] = damagedVisuals;
        }        
    }

    private void SetDamagedVisuals()
    {
        NodeEnums.HealthState healthState = TDGameManager.ComputeHealthState(healthSystem);


        if (!mappedDamagedVisuals.ContainsKey(healthState)) return;

        DamagedVisuals damagedVisuals = mappedDamagedVisuals[healthState];
        
        damagedVisuals.Activate();
        damagedVisuals.takeDamageParticles.Clear(true);
        damagedVisuals.takeDamageParticles.Play();


        nodeMeshMaterial.SetFloat("_IsDamaged", 1f);
        nodeMeshMaterial.SetFloat("_StartTimeBorderFlash", Time.time);
        if (healthState == NodeEnums.HealthState.DESTROYED)
        {
            nodeMeshMaterial.SetFloat("_IsDestroyed", 1f);
            nodeMeshMaterial.SetColor("_DamagedTwitchColor", OWMapDecoratorUtils.s_redColor);
            nodeMeshMaterial.SetColor("_IconColor", OWMapDecoratorUtils.s_redColor);
        }
        else if (healthState == NodeEnums.HealthState.GREATLY_DAMAGED)
        {
            nodeMeshMaterial.SetColor("_BorderFlashColor", OWMapDecoratorUtils.s_redColor);
            nodeMeshMaterial.SetColor("_DamagedTwitchColor", OWMapDecoratorUtils.s_redColor);
            nodeMeshMaterial.SetColor("_IconColor", OWMapDecoratorUtils.s_blueColor);
        }
        else
        {
            nodeMeshMaterial.SetColor("_BorderFlashColor", OWMapDecoratorUtils.s_redColor);
            nodeMeshMaterial.SetColor("_DamagedTwitchColor", OWMapDecoratorUtils.s_yellowColor);
            nodeMeshMaterial.SetColor("_IconColor", OWMapDecoratorUtils.s_blueColor);
        }
    }


    public void Deactivate()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, 1.5f);
        foreach (Collider col in hits)
        {
            if (col.gameObject.GetComponent<PathTile>() != null)
            {
                StartCoroutine(col.gameObject.GetComponent<PathTile>().Deactivate());
            }
        }
    }

    public IEnumerator Animation(bool lost = false)
    {
        if(animationCube == null)
        {
            yield break;
        }
        GameObject newCube = Instantiate(animationCube, transform.parent);
        newCube.transform.position = transform.GetChild(0).position;
        newCube.transform.SetParent(transform.parent);
        if(lost)
        {
            newCube.GetComponent<MeshRenderer>().material = lostMaterial;
        }
        yield return new WaitForSeconds(0.25f);

        newCube.gameObject.GetComponent<Lerp>().LerpScale(new Vector3(1.0f, 100.0f, 1.0f), 0.1f);
        yield return new WaitForSeconds(0.75f);

        newCube.gameObject.GetComponent<Lerp>().LerpScale(new Vector3(0.0f, 100.0f, 0.0f), 1.25f);
    }
}
