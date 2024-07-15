using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;


[InitializeOnLoad]
public static class StartFromFirstScene
{
    private const string MenuName = "Nomad Defender/Start From First Scene";
    private const string StartFromFistSceneKey = "StartFromFirstScene";

    static StartFromFirstScene()
    {
        EditorApplication.delayCall += () =>
        {
            bool isActive = EditorPrefs.GetBool(StartFromFistSceneKey);

            if (isActive == Menu.GetChecked(MenuName))
            {
                return;
            }

            SetFirstSceneAsStartScene();
            Menu.SetChecked(MenuName, isActive);
        };
    }

    [MenuItem(MenuName)]
    static void StartFromFirstSceneMenuOption()
    {
        bool isActive = !EditorPrefs.GetBool(StartFromFistSceneKey);
        EditorPrefs.SetBool(StartFromFistSceneKey, isActive);

        if (isActive)
        {
            SetFirstSceneAsStartScene();
        }
        else
        {
            EditorSceneManager.playModeStartScene = default;
            Debug.Log("Current opened scene was set as default play mode scene");
        }

        Menu.SetChecked(MenuName, isActive);
    }

    private static void SetFirstSceneAsStartScene()
    {
        var pathOfFirstScene = EditorBuildSettings.scenes[0].path;
        var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(pathOfFirstScene);
        EditorSceneManager.playModeStartScene = sceneAsset;
        Debug.Log(pathOfFirstScene + " was set as default play mode scene");
    }
}
