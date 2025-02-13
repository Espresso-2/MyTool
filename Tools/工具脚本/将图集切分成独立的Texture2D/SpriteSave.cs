using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;

namespace Editor
{
    public class SpriteSave
    {
        [MenuItem("Assets/自定义功能/精灵导出图片", false, 1000)]
        static void SaveSprite()
        {
            foreach (Object obj in Selection.objects)
            {
                string selectionPath = AssetDatabase.GetAssetPath(obj);
                var AllAssets = AssetDatabase.LoadAllAssetsAtPath(selectionPath);
                foreach (var Asset in AllAssets)
                {
                    if (Asset is Sprite)
                    {
                        string outPath = Application.dataPath + "/outSprite/" + System.IO.Path.GetFileNameWithoutExtension(selectionPath);
                        if (!System.IO.Directory.Exists(outPath))
                        {
                            System.IO.Directory.CreateDirectory(outPath);
                        }
                        var sprite = Asset as Sprite;
                        Texture2D tex = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height, sprite.texture.format, false);
                        tex.SetPixels(sprite.texture.GetPixels((int)sprite.rect.xMin, (int)sprite.rect.yMin, (int)sprite.rect.width,
                            (int)sprite.rect.height));
                        tex.Apply();
                        System.IO.File.WriteAllBytes(outPath + "/" + sprite.name + ".png", tex.EncodeToPNG());
                    }
                }
            }
            AssetDatabase.Refresh();
        }

        [MenuItem("Assets/自定义功能/快速重命名", false, 1001)]
        public static void ReName()
        {
            string[] Guid = Selection.assetGUIDs;
            foreach (var guid in Guid)
            {
                string assetsPath = AssetDatabase.GUIDToAssetPath(guid);
                if (AssetDatabase.IsValidFolder(assetsPath))
                {
                    Debug.Log($"正在处理文件:{assetsPath}");
                    string[] assetGUIDs = AssetDatabase.FindAssets("", new[] { assetsPath });
                    for (int i = 0; i < assetGUIDs.Length; i++)
                    {
                        string assetPath = AssetDatabase.GUIDToAssetPath(assetGUIDs[i]);
                        string fileExtension = System.IO.Path.GetExtension(assetPath);
                        string newName = $"{i}{fileExtension}";
                        AssetDatabase.RenameAsset(assetPath, newName);
                    }
                    AssetDatabase.Refresh();
                }
                else
                {
                    Debug.LogError("不是文件夹");
                }
            }
        }
    }
}