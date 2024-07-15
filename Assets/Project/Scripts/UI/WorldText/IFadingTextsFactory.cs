using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFadingTextsFactory
{
    public class TextSpawnData
    {
        public Vector3 WorldPosition { get; private set; }
        public string Content { get; private set; }
        public Color TextColor { get; private set; }
        private Color _defaultTextColor;


        public TextSpawnData(Color defaultTextColor)
        {
            _defaultTextColor = defaultTextColor;
        }

        public void Init(Vector3 worldPosition, string content)
        {
            Init(worldPosition, content, _defaultTextColor);
        }
        public void Init(Vector3 worldPosition, string content, Color textColor)
        {
            WorldPosition = worldPosition;
            Content = content;
            TextColor = textColor;
        }
    }

    TextSpawnData GetTextSpawnData();
    void SpawnFadingText(TextSpawnData textSpawnData);
}
