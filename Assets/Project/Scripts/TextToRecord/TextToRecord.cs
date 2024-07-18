using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class TextToRecord : MonoBehaviour
{
    [SerializeField] private TextDecoder title;
    [SerializeField] private TextDecoder subtitle;

    [SerializeField] private Volume _volume;
    [SerializeField] private VolumeProfile _regularProfile;
    [SerializeField] private VolumeProfile _glitchProfile;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            StartCoroutine(GlitchSequence());
        }
    }

    private IEnumerator GlitchSequence()
    {
        title.Activate();
        yield return GlitchScreen(1.5f, 0.25f);
        yield return new WaitForSeconds(0.5f);
        title.Activate();
        yield return GlitchScreen(0.0f, 0.15f);
    }

    private IEnumerator GlitchScreen(float delay, float glitchTime, int soundIndex = -2)
    {
        if (delay > 0.0f) { yield return new WaitForSeconds(delay); }
        if (soundIndex == -1) { GameAudioManager.GetInstance().PlayRandomGlitchSound(); }
        else if (soundIndex >= 0) { GameAudioManager.GetInstance().PlayGlitchSound(soundIndex); }
        _volume.profile = _glitchProfile;
        yield return new WaitForSeconds(glitchTime);
        _volume.profile = _regularProfile;
    }
}
