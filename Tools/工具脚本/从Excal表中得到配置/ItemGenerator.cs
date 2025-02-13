using System.IO;
using Script.Data;
using Script.ScriptableObjectClass;
using UnityEditor;
using UnityEngine;

public class ItemGenerator : MonoBehaviour
{
    [Header("CSV 文件")] public TextAsset csvFile; // 将 CSV 文件拖入此处

    [Header("保存路径")] public string outputPath = "Assets/Items"; // ScriptableObject 的保存路径

    [ContextMenu("批量生成物品")]
    public void GenerateItems()
    {
        if (csvFile == null)
        {
            Debug.LogError("请先指定 CSV 文件！");
            return;
        }
        Sprite[] itemSpirte = Resources.LoadAll<Sprite>("ItemICON");
        string[] lines = csvFile.text.Split('\n');
        for (int i = 1; i < lines.Length; i++) // 从第1行开始跳过标题
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;
            string[] fields = lines[i].Split(',');
            Item newItem = ScriptableObject.CreateInstance<Item>();
            newItem.ID = int.Parse(fields[0]);
            newItem.Name = fields[1];
            newItem.Description = fields[2];
            newItem.itemType = (ItemType)System.Enum.Parse(typeof(ItemType), fields[3]);
            newItem.MaxStack = int.Parse(fields[4]);
            newItem.Width = int.Parse(fields[5]);
            newItem.Height = int.Parse(fields[6]);
            newItem.DefaultCount = int.Parse(fields[7]);
            newItem.Precious = int.Parse(fields[8]);
            foreach (Sprite sprite in itemSpirte)
            {
                if (newItem.ID.ToString() == sprite.name)
                {
                    newItem.Icon = sprite;
                }
            }

// 保存 ScriptableObject 文件
            string assetPath = $"{outputPath}/Item_{newItem.ID}.asset";
            AssetDatabase.CreateAsset(newItem, assetPath);
            /*var ID = int.Parse(fields[0]);
            var Name = fields[1];
            var Description = fields[2];
            var itemType = (ItemType)System.Enum.Parse(typeof(ItemType), fields[3]);
            var MaxStack = int.Parse(fields[4]);
            var Width = int.Parse(fields[5]);
            var Height = int.Parse(fields[6]);

            Item newItem = new Item(ID, Name, Width, Height, itemType)
            {
                Description = Description,
                MaxStack = MaxStack
            };

            // 确保 Items 列表已经初始化
            itemsData.Items.Add(newItem);*/
        }
        // string assetPath = $"{outputPath}/ItemsData.asset";
        // 保存 ScriptableObject
        //  AssetDatabase.CreateAsset(itemsData, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("物品生成完成！");
    }
}

// 创建新的 ScriptableObject
/*Item newItem = ScriptableObject.CreateInstance<Item>();
newItem.ID = int.Parse(fields[0]);
newItem.Name = fields[1];
newItem.Description = fields[2];
newItem.itemType = (ItemType)System.Enum.Parse(typeof(ItemType), fields[3]);
/*newItem.Rarity = int.Parse(fields[4]);
newItem.Value = int.Parse(fields[5]);#1#
newItem.MaxStack = int.Parse(fields[4]);
newItem.Width = int.Parse(fields[5]);
newItem.Height = int.Parse(fields[6]);

// 保存 ScriptableObject 文件
string assetPath = $"{outputPath}/Item_{newItem.ID}.asset";
AssetDatabase.CreateAsset(newItem, assetPath);*/