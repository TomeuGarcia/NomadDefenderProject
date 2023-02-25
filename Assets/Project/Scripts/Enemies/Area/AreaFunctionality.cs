using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaFunctionality : MonoBehaviour
{
    // Start is called before the first frame update
    Transform enemy;
    ParticleSystem ps;
    public enum AreaType { ARMOR, HP, STUNT }
    private void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }
    private void OnEnable()
    {
        StartCoroutine(Deactivate());
    }

    private void Update()
    {
        if(enemy)
        transform.position = enemy.position + Vector3.up * 0.25f;
    }

    IEnumerator Deactivate()
    {
        yield return new WaitForSeconds(1.1f);
        this.gameObject.SetActive(false);
    }

    public void Follow(Transform _enemy)
    {

        enemy = _enemy;
    }
    public void StopFollowing()
    {
        enemy = null;
    }

    void OnParticleCollision(GameObject other)
    {
        Debug.Log("particlecollision");
        Debug.Log(other);
    }
}
