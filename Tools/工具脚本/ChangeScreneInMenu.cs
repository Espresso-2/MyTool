using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class ChangeSceneInMenu : EditorWindow
{
    private Vector2 scrollPosition;

    [MenuItem("Scene/ChangeScene")]
    public static void ShowWindow()
    {
        ChangeSceneInMenu window = GetWindow<ChangeSceneInMenu>("Change Scene");
        window.Show();
    }

    public void OnGUI()
    {
        GUILayout.Label("在BuildSetting中的场景", EditorStyles.boldLabel);
        var scenes = EditorBuildSettings.scenes;

        if (scenes.Length == 0)
        {
            GUILayout.Label("当前BuildSetting 中没有场景", EditorStyles.wordWrappedLabel);
            return;
        }


        if (scenes.Length > 10)
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(300));
        }
        foreach (EditorBuildSettingsScene scene in scenes)
        {
            if (!scene.enabled) continue; 
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scene.path);

            if (GUILayout.Button(sceneName, GUILayout.Height(30)))
            {
                OpenScene(scene.path);
            }
        }

        if (scenes.Length > 10)
        {
            GUILayout.EndScrollView(); 
        }
    }

    private void OpenScene(string scenePath)
    {
       
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene(scenePath);
        }
    }
}