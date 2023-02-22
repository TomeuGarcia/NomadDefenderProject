using UnityEngine;

public class PathLocation : MonoBehaviour
{
    [SerializeField] private int health = 3;
    public HealthSystem healthSystem { get; private set; }

    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material deadMaterial;
    [SerializeField] private HealthHUD healthHUD;

    [Header("NODE VISUALS")]
    [SerializeField] private MeshRenderer nodeMesh;
    private Material nodeMeshMaterial;



    public bool IsDead => healthSystem.IsDead();


    public delegate void PathLocationAction();
    public event PathLocationAction OnDeath;


    private void Awake()
    {
        healthSystem = new HealthSystem(health);
        healthHUD.Init(healthSystem);
    }

    public void TakeDamage(int damageAmount)
    {
        healthSystem.TakeDamage(damageAmount);
        
        if (healthSystem.IsDead())
        {
            Die();
        }   
    }

    private void Die()
    {
        meshRenderer.material = deadMaterial;

        if (OnDeath != null) OnDeath();
    }


    public void InitNodeVisuals(Texture nodeIconTexture, Color borderColor)
    {
        float materailHDR = 4f;

        nodeMeshMaterial = nodeMesh.material;

        nodeMeshMaterial.SetFloat("_TimeOffset", Random.Range(0f, 1f));
        nodeMeshMaterial.SetColor("_IconColor", OWMap_Node.darkGreyColor * OWMap_Node.multiplierColorHDR);
        nodeMeshMaterial.SetFloat("_IsInteractable", 0f);
        nodeMeshMaterial.SetFloat("_NoiseTwitchingEnabled", 0f);
        nodeMeshMaterial.SetFloat("_IsDestroyed", 0f);

        nodeMeshMaterial.SetFloat("_FadeDuration", OWMap_Node.FADE_DURATION);
        nodeMeshMaterial.SetFloat("_TimeStartFade", 0f);
        nodeMeshMaterial.SetFloat("_IsFadingAway", 0f);

        nodeMeshMaterial.SetFloat("_SelectedDuration", OWMap_Node.SELECTED_DURATION);
        nodeMeshMaterial.SetFloat("_IsSelected", 0f);
        nodeMeshMaterial.SetFloat("_TimeStartSelected", 0f);


        nodeMeshMaterial.SetTexture("_IconTexture", nodeIconTexture);
        nodeMeshMaterial.SetColor("_BorderColor", borderColor * OWMap_Node.multiplierColorHDR);
    }

}
