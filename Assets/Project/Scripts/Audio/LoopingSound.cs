using DG.Tweening;
using UnityEngine;

public class LoopingSound : MonoBehaviour
{
    private enum PauseMode { SILENT, PAUSED, RESET }

    [SerializeField] private PauseMode pauseMode;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private float fadeInDuration;
    [SerializeField] private float fadeOutDuration;
    [SerializeField, Range(0, 1)] private float volume;

    private OptionsMenu _optionsMenu;

    private void Awake()
    {
        audioSource.clip = audioClip;
        audioSource.loop = true;
        audioSource.volume = 0f;

        _optionsMenu = PauseMenu.GetInstance().OptionsMenu;
    }

    public void StartPlaying()
    {
        if (pauseMode == PauseMode.SILENT)
        {
            if (!audioSource.isPlaying) audioSource.Play();
        }
        else if (pauseMode == PauseMode.PAUSED)
        {
            audioSource.Play();
        }
        else if (pauseMode == PauseMode.RESET)
        {
            audioSource.Play();
        }

        audioSource.DOFade(volume, fadeInDuration);
    }

    public void StopPlaying()
    {
        Sequence stopSequence = DOTween.Sequence();
        stopSequence.Append(audioSource.DOFade(0f, fadeOutDuration));

        if (pauseMode == PauseMode.SILENT)
        {
        }
        else if (pauseMode == PauseMode.PAUSED)
        {
            stopSequence.AppendCallback(() => audioSource.Pause());
        }
        else if (pauseMode == PauseMode.RESET)
        {
            stopSequence.AppendCallback(() => audioSource.Stop());
        }
    }

    private void Update()
    {
        audioSource.volume = volume * _optionsMenu.GetCombinedSFXandMasterVolumeCoef();
    }
}
