using UnityEngine;

public class EnemyDashView : MonoBehaviour
{
    [SerializeField] private ParticleSystem _dashParticles;
    [SerializeField] private ProgressiveRotator _progressiveRotator;
    [SerializeField] private float _dashRotation;
    
    public void StartDashing()
    {
        _dashParticles.Play();

        _progressiveRotator.enabled = false;
        _progressiveRotator.transform.localRotation = Quaternion.identity;
    }   
    
    public void StopDashing()
    {
        _dashParticles.Stop();

        _progressiveRotator.enabled = true;
    }

    private void OnDisable()
    {
        _progressiveRotator.enabled = true;
        _progressiveRotator.transform.localRotation = Quaternion.identity;
    }
}