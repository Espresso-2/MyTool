using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AdGeneric.Editor
{
    /// <summary>
    /// 用于更改纹理的尺寸
    /// 操作方式：拖拽到指定位置，然后进行操作
    /// </summary>
    public class WebGLTextureExe : EditorWindow
    {
        private const string TipName = "需要标准化的文件 For WebGL";
        private const string Platform = "WebGL";                                        //发布的平台 

        private static readonly Color FilesColor = new Color(0, 1, 072f, 0.5f);

        private readonly List<string> imagePaths = new List<string>();
        private bool isToBigger = true; //向着更大缩放
        private bool compressAble=false;
        private int compressionQuality=50;


        [MenuItem("AdGeneric/WebGL图片处理", priority = 101)]
        private static void TextureEx()
        {
            Debug.Log("打开窗口");
            GetWindowWithRect<WebGLTextureExe>(new Rect(Screen.width / 2, Screen.height / 2, 500, 500)).titleContent = new GUIContent("图片处理");
        }


        private void OnGUI()
        {
            //! 实现拖拽
            Rect drawRect = EditorGUILayout.BeginHorizontal();
            GUILayout.Box(TipName, GUILayout.MinHeight(64), GUILayout.MinWidth(512));
            EditorGUILayout.EndHorizontal();

            //? 文件夹和文件
            GUILayout.Space(10);

            GUI.color = FilesColor;
            GUILayout.Label($"选择了 {imagePaths.Count} 个文件");
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
                        var paths = GetTotalFiles(DragAndDrop.paths);
                        foreach (var t in paths.Where(e=>!imagePaths.Contains(e))) imagePaths.Add(t);

                        break;
                }
            }

            GUILayout.Space(10);
            GUILayout.BeginHorizontal();

            GUIStyle style = GUI.skin.toggle;
            style.fontSize = 14;
            GUI.color = Color.white;
            isToBigger = GUILayout.Toggle(isToBigger, "向着更大的标准化化处理", style);

            GUI.color = Color.yellow;
            compressAble = GUILayout.Toggle(compressAble, "启用压缩", style);
        
            GUILayout.EndHorizontal();
            if (compressAble)
            {
                GUI.color=Color.white;
                GUILayout.Space(20);
                GUILayout.BeginHorizontal();
                compressionQuality = EditorGUILayout.IntSlider(compressionQuality, 0, 100);
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            GUI.color = Color.white;
            if (GUILayout.Button("清除", GUILayout.Width(256), GUILayout.Height(40))) imagePaths.Clear();
            if (GUILayout.Button("开始处理图像", GUILayout.Width(256), GUILayout.Height(40))) ExeFile();
            GUILayout.EndHorizontal();
        }


        private void ExeFile()
        {
            int sc = 0, fc = 0, imgc = 0;
            for (int i = 0; i < imagePaths.Count; i++)
            {
                EditorUtility.DisplayProgressBar("开始处理", $"{i + 1}/{imagePaths.Count}", (float)i / imagePaths.Count);

                string path = imagePaths[i];
                if (AssetDatabase.LoadMainAssetAtPath(path) is Texture2D)
                {
                    imgc++;
                    try
                    {
                        ExeTex(path);  //!处理贴图
                        sc++;
                    }
                    catch 
                    {
                        fc++;
                    }
                }
                else fc++;
            }

            Debug.Log(
                $"总共 :{imagePaths.Count}\n" +
                $"图像 :{imgc}\n" +
                $"成功 :{sc}\n" +
                $"失败 :{fc}\n" );
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
            imagePaths.Clear();

        }

        private void ExeTex(string path)
        {
            var texture = (Texture2D)AssetDatabase.LoadMainAssetAtPath(path);
            var importer = (TextureImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(texture));
            if (texture.width % 4 != 0 || texture.height % 4 != 0)
            {
                importer.isReadable = true;
                importer.SaveAndReimport();

                Vector2Int v2 = GetFourSize(texture.width, texture.height);
                Texture2D texCopy = new Texture2D(v2.x, v2.y);
                //从原来图像上根据现在的大小计算像素点
                for (int h = 0; h < v2.y; h++)
                for (int w = 0; w < v2.x; w++)
                    texCopy.SetPixel(w, h, texture.GetPixelBilinear(w / (v2.x * 1.0f), h / (v2.y * 1.0f)));

                texCopy.Apply();
                File.WriteAllBytes(path, texCopy.EncodeToPNG());
                importer.isReadable = false;
            }
            if (compressAble)
            {
                TextureImporterFormat targetFormat;
                switch (importer.GetAutomaticFormat(Platform))
                {
                    case TextureImporterFormat.DXT5:
                        targetFormat = TextureImporterFormat.DXT5Crunched;
                        break;
                    case TextureImporterFormat.DXT1:
                        targetFormat = TextureImporterFormat.DXT1Crunched;
                        break;
                    case TextureImporterFormat.DXT5Crunched:
                        targetFormat=TextureImporterFormat.DXT5Crunched;
                        break;
                    case TextureImporterFormat.DXT1Crunched:
                        targetFormat = TextureImporterFormat.DXT1Crunched;
                        break;
                    default: return;
                }
                // 创建特定平台压缩实例
                var platformSettings = new TextureImporterPlatformSettings
                {
                    overridden = true,
                    name = Platform,
                    textureCompression = TextureImporterCompression.Compressed,
                    format = targetFormat,
                    compressionQuality = compressionQuality,
                    maxTextureSize = GetMaxSize(texture)
                };

                importer.SetPlatformTextureSettings(platformSettings);
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
        
            return files.Where(e => !".meta".Equals(new FileInfo(e).Extension, StringComparison.CurrentCultureIgnoreCase));
        }


        private static int GetMaxSize(Texture2D texture)
        {
            //分辨率区间的预备
            var start = new List<int> { 0, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192, 16384 };
            var end = new List<int> { 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192, 16384, 100000 };
            var zones = start.Zip(end, (item1, item2) => (startIdx: item1, endIdx: item2)).ToList();

            //取分辨率高宽的最大值
            var size = Math.Max(texture.width, texture.height);  //取【宽】【高】中的最大值

            //判断所属的区间
            var maxSize = zones
                .First(x => x.startIdx <= size && size <= x.endIdx)
                .endIdx;
            //Debug.Log($"图的分辨率 = {texture.width} * {texture.height} size = {size}, MaxSize = {maxSize}");
            return Mathf.Clamp(maxSize,24,8192);
        }


        /// <summary>
        /// 目标尺寸，宽高整数4处理
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private Vector2Int GetFourSize(int width, int height)
        {
            if (isToBigger)
            {
                while (width % 4 != 0) width++;

                while (height % 4 != 0) height++;
            }
            else
            {
                while (width % 4 != 0) width--;

                while (height % 4 != 0) height--;
            }

            return new Vector2Int(Mathf.Max(4, width), Mathf.Max(4, height));
        }
    }
}
