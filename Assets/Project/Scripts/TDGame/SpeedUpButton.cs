using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SpeedUpButton : MonoBehaviour
{
    [SerializeField] private Image image;

    [SerializeField] private List<Sprite> sprites = new List<Sprite>();
    [SerializeField] private List<float> timeScales = new List<float>();

    private int current = 0;

    public void ChangeTimeSpeed()
    {
        current = (current + 1) % sprites.Count;

        Time.timeScale = timeScales[current];
        image.sprite = sprites[current];
    }

    private void OnDestroy()
    {
        Time.timeScale = 1.0f;
    }
}
