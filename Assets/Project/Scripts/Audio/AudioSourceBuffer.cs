using UnityEngine;

public class AudioSourceBuffer : MonoBehaviour
{
    [SerializeField] private AudioSource[] _audioSources;
    [SerializeField] private AudioClip[] _clips;
    [SerializeField] private Vector2 _volumeRange = new Vector2(1.0f, 1.0f);
    [SerializeField] private Vector2 _pitchRange = new Vector2(0.9f, 1.1f);
    private int _nextAudioSourceIndex = 0;

    public void Play()
    {
        AudioSource audioSource = _audioSources[_nextAudioSourceIndex];
        _nextAudioSourceIndex = (_nextAudioSourceIndex + 1) % _audioSources.Length;

        audioSource.volume = Random.Range(_volumeRange.x, _volumeRange.y);
        audioSource.pitch = Random.Range(_pitchRange.x, _pitchRange.y);
        audioSource.clip = _clips[Random.Range(0, _clips.Length)];
        audioSource.Play();
    }
}