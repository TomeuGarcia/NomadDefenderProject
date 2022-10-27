using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static PathFollower;

public class Enemy : MonoBehaviour
{
    private void OnMouseEnter()
    {
        Debug.Log("GOT CLICKED");
    }

    [Header("Mesh")]
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material baseMaterial;
    [SerializeField] private Material selectedMaterial;

    [Header("Components")]
    [SerializeField] public PathFollower pathFollower;
    [SerializeField] public Transform transformToMove;
    [SerializeField] public Rigidbody rb;
    [SerializeField] private RectTransform canvasTransform;
    [SerializeField] private Image healthImage;
    [SerializeField] private BoxCollider boxCollider;

    [Header("Stats")]
    [SerializeField] private int damage = 1;
    [SerializeField] private int health = 2;
    [SerializeField] public int currencyDrop;


    private HealthSystem healthSystem;

    public delegate void EnemyAction(Enemy enemy);
    public static EnemyAction OnEnemyDeathDropCurrency;
    public EnemyAction OnEnemyDeath;

    public Vector3 Position => meshRenderer.transform.position;
    public Vector3 Right => transformToMove.right;

    private void Awake()
    {
        healthSystem = new HealthSystem(health);
    }

    private void OnValidate()
    {
        boxCollider.center = meshRenderer.gameObject.transform.localPosition;
        boxCollider.size = meshRenderer.gameObject.transform.localScale;
    }

    private void OnEnable()
    {
        ResetEnemy();

        pathFollower.OnPathEndReached += Attack;
    }

    private void OnDisable()
    {
        pathFollower.OnPathEndReached -= Attack;
    }

    private void ResetEnemy()
    {
        StopAllCoroutines();

        meshRenderer.material = baseMaterial;
        healthSystem.HealToMax();
        healthImage.fillAmount = healthSystem.healthRatio;
    }

    private void Update()
    {
        Vector3 cameraDirection = Camera.main.transform.forward;
        //cameraDirection.y = 0;

        canvasTransform.rotation = Quaternion.LookRotation(cameraDirection);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PathLocation"))
        {
            other.gameObject.GetComponent<PathLocation>().TakeDamage(damage);
            Suicide();
            //Destroy(gameObject);//////////////////////////
        }
    }



    private void Attack()
    {
        Vector3 launchDirection = (transformToMove.forward + (transformToMove.up * 0.2f)).normalized;
        rb.AddForce(launchDirection * 20f, ForceMode.Impulse);
    }

    public void TakeDamage(int damageAmount)
    {
        healthSystem.TakeDamage(damageAmount);

        healthImage.fillAmount = healthSystem.healthRatio;

        if (healthSystem.IsDead())
        {
            Death();
        }
    }

    private void Suicide()
    {
        Deactivation();
    }

    private void Death()
    {
        if (OnEnemyDeathDropCurrency != null) OnEnemyDeathDropCurrency(this);
        Deactivation();
    }

    private void Deactivation()
    {
        if (OnEnemyDeath != null) OnEnemyDeath(this);

        rb.velocity = Vector3.zero;
        gameObject.SetActive(false);
    }

    public void ChangeMat()
    {
        meshRenderer.material = selectedMaterial;
    }
    public void ChangeToBaseMat()
    {
        meshRenderer.material = baseMaterial;
    }
}
