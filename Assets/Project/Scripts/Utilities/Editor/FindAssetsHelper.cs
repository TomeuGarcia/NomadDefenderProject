using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

public static class FindAssetsHelper
{
    public static T[] GetAllAssetsInProject<T>(Action<T> elementFindCallback = null) where T : UnityEngine.Object
    {
        List<string> paths = AssetDatabase.FindAssets($"t:{typeof(T).Name}").ToList()
            .Select(AssetDatabase.GUIDToAssetPath).ToList();

        T[] assets = new T[paths.Count];

        for (int i = 0; i < paths.Count; ++i)
        {
            T asset = (T)AssetDatabase.LoadAssetAtPath(paths[i], typeof(T));
            assets[i] = asset;

            if (elementFindCallback != null)
            {
                elementFindCallback(asset);
            }
        }

        return assets;
    }
}