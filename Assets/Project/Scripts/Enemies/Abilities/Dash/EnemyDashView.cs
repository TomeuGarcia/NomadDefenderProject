using UnityEngine;

public class EnemyDashView : MonoBehaviour
{
    [SerializeField] private ParticleSystem _dashParticles;
    [SerializeField] private GameObject _dashSphere;
    
    public void StartDashing()
    {
        _dashParticles.Play();
        _dashSphere.SetActive(true);
    }   
    
    public void StopDashing()
    {
        _dashParticles.Stop();
        _dashSphere.SetActive(false);
    }        
}