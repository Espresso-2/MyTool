using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;


public class AnimShrink : EditorWindow
{
        
    private const string TipName = "拖入需要裁剪的动画";
    private const string Title = "裁剪动画";

    private readonly List<string> filePaths = new List<string>();

    [MenuItem(nameof(AdGeneric)+"/" + Title, priority = 103)]
    private static void TextureEx()
    {
        GetWindowWithRect<AnimShrink>(new Rect(Screen.width / 2, Screen.height / 2, 500, 500)).titleContent =
            new GUIContent(Title);
    }

    public static readonly Color Khaki = new Color(0.9411765f, 0.9019608f, 0.5490196f, 1f);

    private void Awake() => filePaths.Clear();
    private Vector2 pos;
    private int space=1;
    private void OnGUI()
    {
        //! 实现拖拽
        Rect drawRect = EditorGUILayout.BeginHorizontal();
        GUILayout.Box(TipName, GUILayout.MinHeight(64), GUILayout.MinWidth(512));
        UnityEngine.Event currentEvent = UnityEngine.Event.current;
        //拖拽范围内
        if (drawRect.Contains(currentEvent.mousePosition))
        {
            switch (currentEvent.type)
            {
                case EventType.DragUpdated:
                    DragAndDrop.visualMode = DragAndDropVisualMode.Generic; //到达目标区域的显示方式
                    break;
                case EventType.DragPerform:
                    var paths = GetTotalFiles(DragAndDrop.paths)
                        .Where(e=>!filePaths.Contains(e))
                        .Distinct()
                        .Where(e => ".anim".Equals(Path.GetExtension(e),StringComparison.InvariantCultureIgnoreCase));
                    filePaths.AddRange(paths);
                    break;
            }
        }

        EditorGUILayout.EndHorizontal();
        
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.Label("采样率");
        space = EditorGUILayout.IntSlider(space, 0, 100);
        GUILayout.EndHorizontal();
        
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUI.color = new Color(0, 1, 072f, 0.5f);
        GUILayout.Label($"选择了 {filePaths.Count} 个文件", new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleLeft
        });
        GUI.color = Color.white;
        GUILayout.EndHorizontal();

        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();
        
        pos = GUILayout.BeginScrollView(pos, GUILayout.MaxHeight(400), GUILayout.MinHeight(20));
        var itemLabel = new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleLeft};
        foreach (var path in filePaths.ToArray())
        {
            GUILayout.BeginHorizontal();
            var itemBox = new GUIStyle(GUI.skin.box) {alignment = TextAnchor.MiddleLeft};
            GUI.color = Color.white;
            GUILayout.Label(AssetDatabase.GetCachedIcon(path), itemBox,
                GUILayout.Width(20), GUILayout.Height(20));
            GUI.color = Khaki;
            GUILayout.Label(Path.GetFileNameWithoutExtension(path), itemLabel);
            GUI.color = Color.white;
            if (GUILayout.Button("X", GUILayout.Width(20))) filePaths.Remove(path);

            GUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();

        GUI.color = Color.white;
        GUILayout.Space(20);
        GUILayout.BeginHorizontal();
        GUILayout.Label("1.可以从资源管理器拖入");
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("2.1 + 1 = 3");
        GUILayout.EndHorizontal();
        GUILayout.Space(20);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("清除", GUILayout.Width(256), GUILayout.Height(40))) filePaths.Clear();
        if (GUILayout.Button("执行", GUILayout.Width(256), GUILayout.Height(40))) ShrinkAnim();
        GUILayout.EndHorizontal();
    }
    private void ShrinkAnim()
    {
        if (filePaths.Count==0 || space <= 0) return;
        try
        {
            int c = 0, total = filePaths.Count;
            foreach (var path in filePaths)
            {
                if (EditorUtility.DisplayCancelableProgressBar(
                    "开始处理", 
                    $"{c + 1}/{total}", 
                    (float)c / total))
                    break;
                c++;
                var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
                var bindings = AnimationUtility.GetCurveBindings(clip);
                foreach (var binding in bindings)
                {
                    var curve = AnimationUtility.GetEditorCurve(clip,binding);
                    var keys = curve.keys;
                    if (keys.Length <= 2) continue;
                    curve.keys = keys.Where((t, i) => i % (space + 1) != 0 || i == 0 || i == keys.Length - 1).ToArray();
                    AnimationUtility.SetEditorCurve(clip,binding,curve);
                }
            }
        }
        finally
        {
            EditorUtility.ClearProgressBar();
            AssetDatabase.SaveAssets();
        }
        
        
    }
    private static IEnumerable<string> GetTotalFiles(IEnumerable<string> paths)
    {
        var files = new List<string>();
        foreach (var path in paths)
        {
            if (File.Exists(path)) files.Add(path);
            else if (Directory.Exists(path)) files.AddRange(Directory.EnumerateFiles(path));
        }

        return files.Where(e => !".meta".Equals(Path.GetExtension(e),StringComparison.InvariantCultureIgnoreCase));
    }

    
}
