using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class UILabelChangeFont : EditorWindow
{
    private List<UILabel> uiLabels = new();

    [MenuItem("Tool/" + nameof(UILabelChangeFont) + "Open")]
    public static void OpenWindow()
    {
        EditorWindow editorWindow = GetWindow<UILabelChangeFont>("更换UILabel字体");
    }

    private void OnGUI()
    {
        HorizontalRegion(() =>
        {
            if (GUILayout.Button("得到当前场景中所有的UILayout组件"))
            {
                GetComponentUiLabel();
            }
        });
        GUILayout.Space(200);
        GUILayout.Label("当点击获取组件后会显示更换字体按钮");
        if (uiLabels.Count <= 0) return;
        GUILayout.Space(200);
        HorizontalRegion(() =>
        {
            if (GUILayout.Button("更换字体")) ChangeFont();
        });
    }

    private void ChangeFont()
    {
        if (uiLabels.Count == 0) return;
        foreach (UILabel uiLabel in uiLabels)
        {
            uiLabel.trueTypeFont = AssetDatabase.FindAssets("t:Font").Select(GUID => AssetDatabase.GUIDToAssetPath(GUID))
                .Where(path => System.IO.Path.GetFileNameWithoutExtension(path) == "simhei") // 过滤 SimHei 字体
                .Select(path => AssetDatabase.LoadAssetAtPath<Font>(path))
                .FirstOrDefault(); // 获取第
        }
        AssetDatabase.Refresh();
    }

    private void GetComponentUiLabel()
    {
        if (uiLabels.Count > 0) uiLabels.Clear();
        uiLabels = Resources.FindObjectsOfTypeAll<UILabel>().ToList();
    }

    private void HorizontalRegion(Action action)
    {
        GUILayout.BeginHorizontal();
        action?.Invoke();
        GUILayout.EndHorizontal();
    }
}