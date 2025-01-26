using UnityEditor;
using UnityEngine;

public class BatchMeshAutoAssign : EditorWindow
{
    [MenuItem("Tools/Batch Mesh Auto Assign")]
    public static void ShowWindow()
    {
        GetWindow<BatchMeshAutoAssign>("Batch Mesh Auto Assign");
    }

    void OnGUI()
    {
        if (GUILayout.Button("Auto Assign MeshColliders for Selected Objects"))
        {
            AutoAssignMeshCollidersForSelectedObjects();
        }
    }

    private void AutoAssignMeshCollidersForSelectedObjects()
    {
        GameObject[] selectedObjects = Selection.gameObjects; // 获取当前选中的对象

        foreach (var obj in selectedObjects)
        {
            AutoAssignMeshColliderForGameObject(obj);
        }

        Debug.Log("Batch Mesh Collider Auto Assign completed for selected objects.");
    }

    private void AutoAssignMeshColliderForGameObject(GameObject obj)
    {
        // 在这里编写类似前面示例的代码，根据对象的名称自动关联MeshCollider
        MeshCollider meshCollider = obj.GetComponent<MeshCollider>();

        if (meshCollider != null)
        {
            string objectName = obj.name;
            string meshName = objectName;

            Mesh foundMesh = FindMeshByName(meshName);

            if (foundMesh != null)
            {
                meshCollider.sharedMesh = foundMesh;
                Debug.Log("MeshCollider assigned successfully for " + objectName);
            }
            else
            {
                Debug.LogError("Mesh not found for " + objectName);
            }
        }
    }

    private Mesh FindMeshByName(string meshName)
    {
        // 与前面示例相同，根据Mesh名称查找Mesh
        string[] allMeshes = AssetDatabase.FindAssets("t:Mesh");

        foreach (var meshGUID in allMeshes)
        {
            string meshPath = AssetDatabase.GUIDToAssetPath(meshGUID);
            Mesh mesh = AssetDatabase.LoadAssetAtPath<Mesh>(meshPath);

            if (mesh != null && mesh.name == meshName)
            {
                return mesh;
            }
        }

        return null;
    }
}
