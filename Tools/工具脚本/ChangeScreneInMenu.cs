using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class ChangeSceneInMenu : EditorWindow
{
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

        // 遍历并显示场景按钮
        foreach (EditorBuildSettingsScene scene in scenes)
        {
            if (!scene.enabled) continue; // 跳过未启用的场景
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scene.path);
            
            if (GUILayout.Button(sceneName, GUILayout.Height(30)))
            {
                OpenScene(scene.path);
            }
        }
    }

    private void OpenScene(string scenePath)
    {
        // 保存当前场景的更改并打开选中的场景
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene(scenePath);
        }
    }
}