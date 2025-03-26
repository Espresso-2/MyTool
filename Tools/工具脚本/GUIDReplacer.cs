using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class GUIDReplacer : EditorWindow
{
    /*1. 得到要被替换的资源的GUID与FIleID
     *2. 得到替换后的资源的GUID与FIleID
     *3. 检查当前场景是否在BulidingSetting中，如果存在则创建一个新的场景并跳转到呢个新场景再执行操作（不能在需要被替换的场景中进行操作）
     *4. 得到BulidingSetting中所有的场景文件，通过File.GetAllText得到.Unity文件中的Yaml数据
     *5. 找到要被替换的GUID与FileID ，替换为新的GUID与FileID
     *
     */

    // private static string AssetsPath = Application.dataPath;
    private static string OLD_GUID = "请输入旧的GUID";
    private static string OLD_FileID = "请输入旧的LocalID";
    private static string NEW_GUID = "请输入新的GUID";
    private static string NEW_FileID = "请输入新的LocalID";
    private static string OLDAssetName = string.Empty;

    [MenuItem("Tools/资源替换")]
    private static void Init()
    {
        var window = GetWindow<GUIDReplacer>("精灵资源引用替换工具");
        window.position = new Rect(Screen.width / 4f, Screen.height / 4f, 800, 600);
    }

    private void OnGUI()
    {
        Vertical(() =>
        {
            EditorGUILayout.LabelField("步骤1：请在Project窗口，右键资源->资源替换/得到要替换的GUID与FIleID");
            EditorGUILayout.LabelField("步骤2：点击尝试获取新的资源的GUID与FILEID");
            EditorGUILayout.LabelField("步骤3：请切换到一个空的场景中并且不在BuildingSetting中");
            EditorGUILayout.LabelField("步骤4：点击尝试替换所以预制体中的引用");
            EditorGUILayout.LabelField("步骤5：点击尝试替换所有场景中的引用（只有在BuildingSetting中）");
        });
        Horizontal(() =>
        {
            OLD_GUID = EditorGUILayout.TextField("旧的GUID", OLD_GUID);
            OLD_FileID = EditorGUILayout.TextField("旧的LocalID", OLD_FileID);
        });
        EditorGUILayout.Space(10f);
        Horizontal(() =>
        {
            NEW_GUID = EditorGUILayout.TextField("新的GUID", NEW_GUID);
            NEW_FileID = EditorGUILayout.TextField("新的LocalID", NEW_FileID);
        });
        if (GUILayout.Button("尝试得到新的资源的GUID与FIleID"))
        {
            var obj = TryGetNewGUIDAndFIleID(OLDAssetName);
            if (ReferenceEquals(obj, null))
            {
                Debug.Log("没有找到同名的Texture2D资源");
            }
            else
            {
                AssetDatabase.TryGetGUIDAndLocalFileIdentifier(obj, out string guid, out long localId);
                {
                    NEW_GUID = guid;
                    NEW_FileID = localId.ToString();
                }
            }
        }
        if (GUILayout.Button("尝试替换所有预制体中的引用"))
        {
            ReplacePrefab();
        }
        if (GUILayout.Button("尝试替换所有场景中的引用"))
        {
            ReplaceScene();
        }
    }

    [MenuItem("Assets/资源替换/得到替换的GUID与FILEID")]
    private static void GetObjectGUIDAndLocalID()
    {
        OLDAssetName = Selection.activeObject.name;
        Debug.Log($"选中的激活目标为 : {OLDAssetName}");
        AssetDatabase.TryGetGUIDAndLocalFileIdentifier(Selection.activeObject, out string guid, out long localId);
        {
            OLD_GUID = guid;
            OLD_FileID = localId.ToString();
        }
    }

    private static Texture2D TryGetNewGUIDAndFIleID(string TargetName)
    {
        /*从资源文件夹下找到指定类型的所有文件，对比其中的文件名，如果找到则返回资源*/
        var FindAssetsByType = AssetDatabase.FindAssets("t:Texture2D");
        foreach (string guid in FindAssetsByType)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var Texture2D = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (Texture2D.name.Equals(TargetName))
            {
                Debug.Log("找到了同名资源文件夹");
                return Texture2D;
            }
        }
        return null;
    }

    private static void ReplacePrefab()
    {
        int Num = 0;
        var allPrefabsPaths = GetAllPrefabFilePath();
        foreach (string allPrefabsPath in allPrefabsPaths)
        {
            string text = System.IO.File.ReadAllText(allPrefabsPath);
            // Debug.Log(text);
            if (text.Contains(OLD_GUID))
            {
                string result = ReplaceGUIDAndLocalID(text, OLD_GUID, OLD_FileID, NEW_GUID, NEW_FileID);
                System.IO.File.WriteAllText(allPrefabsPath, result);
                Num++;
            }
        }
        Debug.Log($"处理了{Num}个预制体");
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
    }

    private static void ReplaceScene()
    {
        var allScenePaths = GetAllSceneFilePath();
        int Num = 0;
        foreach (string scenePath in allScenePaths)
        {
            string text = System.IO.File.ReadAllText(scenePath);
            if (text.Contains(OLD_GUID))
            {
                string result = ReplaceGUIDAndLocalID(text, OLD_GUID, OLD_FileID, NEW_GUID, NEW_FileID);
                System.IO.File.WriteAllText(scenePath, result);
                Num++;
            }
        }
        Debug.Log($"处理了{Num}个场景");
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
    }

    private static List<string> GetAllSceneFilePath()
    {
        List<string> allScenePaths = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled)
            {
                allScenePaths.Add(scene.path);
            }
        }
        return allScenePaths;
    }

    private static List<string> GetAllPrefabFilePath()
    {
        List<string> allPrefabPaths = new List<string>();
        string[] PrefabsGUIDs = AssetDatabase.FindAssets("t:Prefab");
        foreach (string prefabsGUID in PrefabsGUIDs)
        {
            string path = AssetDatabase.GUIDToAssetPath(prefabsGUID);
            allPrefabPaths.Add(path);
        }
        return allPrefabPaths;
    }

    private static string ReplaceGUIDAndLocalID(string targetText, string old_GUID, string old_FileID, string new_GUID, string new_FileID)
    {
        string result = targetText.Replace(old_FileID, new_FileID);
        string FinalResult = result.Replace(old_GUID, new_GUID);
        return FinalResult;
    }

    private void Horizontal(Action callback)
    {
        EditorGUILayout.BeginHorizontal();
        callback?.Invoke();
        EditorGUILayout.EndHorizontal();
    }

    private void Vertical(Action callback)
    {
        EditorGUILayout.BeginVertical();
        callback?.Invoke();
        EditorGUILayout.EndVertical();
    }

    private void OnDestroy()
    {
        OLD_GUID = "请输入旧的GUID";
        OLD_FileID = "请输入旧的LocalID";
        NEW_GUID = "请输入新的GUID";
        NEW_FileID = "请输入新的LocalID";
        OLDAssetName = string.Empty;
    }
}