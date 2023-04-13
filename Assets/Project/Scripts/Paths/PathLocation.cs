using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;

public class PathLocation : MonoBehaviour
{
    [Header("HEALTH")]
    [SerializeField] private int health = 3;
    public HealthSystem healthSystem { get; private set; }

    [Header("HUD")]
    [SerializeField] private LocationHealthHUD healthHUD;

    [Header("NODE VISUALS")]
    [SerializeField] private MeshRenderer nodeMesh;
    private Material nodeMeshMaterial;

    [Header("MESH HOLDER")]
    [SerializeField] private Transform locationMeshHolder;

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


    public delegate void PathLocationAction();
    public event PathLocationAction OnDeath;
    public static event PathLocationAction OnTakeDamage;


    private void Awake()
    {
        healthSystem = new HealthSystem(health);
        healthHUD.Init(healthSystem);
        InitParticles();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D) && !IsDead) TakeDamage(1);
    }

    public void TakeDamage(int damageAmount)
    {
        healthSystem.TakeDamage(damageAmount);

        SetDamagedVisuals();
        locationMeshHolder.DOComplete();

        if (healthSystem.IsDead())
        {
            Die();
            locationMeshHolder.DOPunchScale(new Vector3(1f, 0f, 1f) * 0.3f, 0.6f, 8);
            GameAudioManager.GetInstance().PlayLocationDestroyed();
        }
        else
        {
            locationMeshHolder.DOPunchScale(new Vector3(1f, 0f, 1f) * 0.4f, 0.9f, 8);
            GameAudioManager.GetInstance().PlayLocationTakeDamage();
        }

        if (OnTakeDamage != null) OnTakeDamage();
    }

    private void Die()
    {
        if (OnDeath != null) OnDeath();
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

}
