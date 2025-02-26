using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GetAllNGUI : EditorWindow
{
    private string saveFilePath => Application.dataPath + "/AllNGUI";

    private List<string> AllFilePath = new();

    private Vector2 scorllViewPosition;

    private void OnEnable()
    {

        GetFilePath();
    }

    private void GetFilePath()
    {
        if (AllFilePath.Count > 0)
            AllFilePath.Clear();
        //TODO:找到这个名字m_AddComponentMenu
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (Assembly assmbly in assemblies)
        {
            Type[] types = assmbly.GetTypes();
            foreach (Type type in types)
            {
                if (!typeof(MonoBehaviour).IsAssignableFrom(type))
                    continue;
                AddComponentMenu attribute = type.GetCustomAttribute<AddComponentMenu>();
                if (attribute == null)
                    continue;

                FieldInfo field = attribute.GetType().GetField("m_AddComponentMenu", BindingFlags.NonPublic | BindingFlags.Instance);
                if (field == null)
                    continue;

                string Title = field.GetValue(attribute) as string;
                if (Title.Contains("NGUI/"))
                {

                    Debug.Log($"脚本名字 ： {type.Name}");

                    string path = FindScriptFilePath(type);
                    AllFilePath.Add(path);


                }
            }
        }
    }

    [MenuItem("Tool/获得所有NGUI脚本编辑器窗口")]
    public static void ShowWindow()
    {
        GetAllNGUI getAllNGUI = GetWindow<GetAllNGUI>();
        getAllNGUI.titleContent = new GUIContent("获得所有NGUI脚本的编辑器");

    }

    private string FindScriptFilePath(Type type)
    {
        //找到所有
        string[] typeGUID = AssetDatabase.FindAssets($"t:MonoScript {type.Name}");
        for (int i = 0; i < typeGUID.Length; i++)
        {
            string AssetPath = AssetDatabase.GUIDToAssetPath(typeGUID[i]);
            MonoScript monoScript = AssetDatabase.LoadAssetAtPath<MonoScript>(AssetPath);
            if (monoScript != null && monoScript.GetClass() == type)
            {
                return AssetPath;
            }
        }
        return "未找到";
    }

    private void MoveAssetFile()
    {
        if (AllFilePath.Count > 0)
        {
            foreach (string path in AllFilePath)
            {
                string AssetName = path.Split("/")[^1];
                AssetDatabase.MoveAsset(path, "Assets/AllNGUI/" + AssetName);
            }
        }
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
    }

    private void OnGUI()
    {
        GUILayout.TextField($"默认集合地址是：{saveFilePath}");

        if (AllFilePath.Count > 0)
        {
            scorllViewPosition = GUILayout.BeginScrollView(scorllViewPosition);

            for (int i = 0; i < AllFilePath.Count; i++)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.TextField($"脚本文件地址是：", AllFilePath[i]);
                if (GUILayout.Button("X"))
                {
                    AllFilePath.RemoveAt(i);

                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            if (GUILayout.Button("开始转移文件"))
            {
                MoveAssetFile();
            }

        }



    }
}
