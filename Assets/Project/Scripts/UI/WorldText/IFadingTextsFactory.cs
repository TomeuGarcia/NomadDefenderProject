using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFadingTextsFactory
{
    public class TextSpawnData
    {
        public Vector3 WorldPosition { get; private set; }
        public string Content { get; private set; }
        public Color BackgroundImageColor { get; private set; }
        private Color _defaultBackgroundImageColor;


        public TextSpawnData(Color defaultBackgroundImageColor)
        {
            _defaultBackgroundImageColor = defaultBackgroundImageColor;
        }

        public void Init(Vector3 worldPosition, string content)
        {
            Init(worldPosition, content, _defaultBackgroundImageColor);
        }
        public void Init(Vector3 worldPosition, string content, Color backgroundImageColor)
        {
            WorldPosition = worldPosition;
            Content = content;
            BackgroundImageColor = backgroundImageColor;
        }
    }

    TextSpawnData GetTextSpawnData();
    void SpawnFadingText(TextSpawnData textSpawnData);
}
