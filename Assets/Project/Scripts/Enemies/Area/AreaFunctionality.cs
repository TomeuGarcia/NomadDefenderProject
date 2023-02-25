using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaFunctionality : MonoBehaviour
{
    // Start is called before the first frame update
    Transform enemy;
    ParticleSystem ps;
    int armorToAdd;

    protected List<Enemy> enemies = new List<Enemy>();
    public enum AreaType { ARMOR, HP, STUNT }
    private void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }
    private void OnEnable()
    {
        enemies.Clear();
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

    private void OnTriggerEnter(Collider other)
    {
        if ((other.tag == "Enemy") && !(enemies.Contains(other.GetComponent<Enemy>())))
        {
            if (enemy)
            {
                if ((other.gameObject == enemy.gameObject))
                {
                    return;
                }
            }
           
            other.GetComponent<Enemy>().AddArmor(armorToAdd);
            enemies.Add(other.GetComponent<Enemy>());
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Enemy" && enemies.Contains(other.GetComponent<Enemy>()))
        {
            enemies.Remove(other.GetComponent<Enemy>());
        }
    }

    public void SetArmorToAdd(int armorAmount)
    {
        armorToAdd = armorAmount;
    }
}
