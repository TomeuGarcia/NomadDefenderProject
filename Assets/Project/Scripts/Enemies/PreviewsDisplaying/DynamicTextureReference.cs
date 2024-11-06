using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CreateAssetMenu(fileName = "DynamicTextureReference_NAME", menuName = 
    SOAssetPaths.HELPERS + "DynamicTextureReference")]
public class DynamicTextureReference : ScriptableObject
{
    private Texture2D _texture;
    public Texture2D Texture => _texture;

    public void BakeFromRenderTexture(RenderTexture renderTexture)
    {
        _texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        
        RenderTexture oldRenderTexture = RenderTexture.active;
        RenderTexture.active = renderTexture;

        _texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        _texture.Apply();

        RenderTexture.active = oldRenderTexture;
    }
}
