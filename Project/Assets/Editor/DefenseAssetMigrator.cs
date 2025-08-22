using UnityEditor;
using UnityEngine;

public static class DefenseAssetMigrator
{
    [MenuItem("Tools/Defense/Migrate Stage3 To Defense")]
    public static void Migrate()
    {
        Move("Assets/Prefabs/Stage 3/Enemies", "Assets/Prefabs/Defense/Enemies");
        Move("Assets/Prefabs/Stage 3/SubStages", "Assets/Prefabs/Defense/SubStages");
        Move("Assets/Prefabs/Stage 3", "Assets/Prefabs/Defense");
        Move("Assets/Sprites/Stage 3", "Assets/Sprites/Defense");
        Move("Assets/Scripts/Stage 3", "Assets/Scripts/Defense");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Migration attempt done. Check Console for move results.");
    }

    private static void Move(string from, string to)
    {
        if (!AssetDatabase.IsValidFolder(from) && AssetDatabase.FindAssets("", new[] { from }).Length == 0)
        {
            Debug.Log($"Skip: {from} not found");
            return;
        }
        if (!AssetDatabase.IsValidFolder(to))
        {
            var parent = System.IO.Path.GetDirectoryName(to).Replace('\\','/');
            var leaf = System.IO.Path.GetFileName(to);
            AssetDatabase.CreateFolder(parent, leaf);
        }
        var error = AssetDatabase.MoveAsset(from, to);
        if (!string.IsNullOrEmpty(error))
        {
            var guids = AssetDatabase.FindAssets("*", new[] { from });
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var filename = System.IO.Path.GetFileName(path);
                var target = System.IO.Path.Combine(to, filename).Replace('\\','/');
                var e2 = AssetDatabase.MoveAsset(path, target);
                if (!string.IsNullOrEmpty(e2))
                {
                    Debug.LogWarning($"Failed to move {path} -> {target}: {e2}");
                }
            }
        }
        else
        {
            Debug.Log($"Moved {from} -> {to}");
        }
    }
}
