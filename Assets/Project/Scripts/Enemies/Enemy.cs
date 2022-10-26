using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Enemy : MonoBehaviour
{
    [SerializeField] public PathFollower pathFollower;
    [SerializeField] public Transform transformToMove;
    [SerializeField] public Rigidbody rb;

    public Vector3 Position => transformToMove.position;
    public Vector3 Right => transformToMove.right;


    private void OnEnable()
    {
        pathFollower.OnPathEndReached += Attack;
    }

    private void OnDisable()
    {
        pathFollower.OnPathEndReached -= Attack;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PathLocation"))
        {
            other.gameObject.GetComponent<PathLocation>().TakeDamage();
            Destroy(gameObject); ////
        }
    }



    private void Attack()
    {
        Debug.Log("Attack");

        Vector3 launchDirection = (transformToMove.forward + (transformToMove.up * 0.2f)).normalized;
        rb.AddForce(launchDirection * 20f, ForceMode.Impulse);
    }

    public void RotateTowardsDirection(Vector3 direction)
    {
        pathFollower.paused = true;

        StartCoroutine(DoRotateTowardsDirection(direction));
    }

    private IEnumerator DoRotateTowardsDirection(Vector3 direction)
    {
        while (Vector3.Dot(transformToMove.forward, direction) < 1f)
        {
            transformToMove.rotation = Quaternion.RotateTowards(transformToMove.rotation, Quaternion.LookRotation(direction, transform.up), 300f * Time.deltaTime);
            yield return null;
        }

        transformToMove.rotation = Quaternion.LookRotation(direction, transform.up);
        Attack();
    }

}
