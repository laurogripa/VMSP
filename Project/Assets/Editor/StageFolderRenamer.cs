using UnityEditor;
using UnityEngine;

public static class StageFolderRenamer
{
    [MenuItem("Tools/Defense/Rename Stage Folders")]
    public static void Rename()
    {
        RenameFolder("Assets/Sprites/Stage 0", "Assets/Sprites/Main Menu");
        RenameFolder("Assets/Sprites/Stage 1", "Assets/Sprites/Labyrinth");
        RenameFolder("Assets/Sprites/Stage 2", "Assets/Sprites/Genius");

        RenameFolder("Assets/Scripts/Stage 1", "Assets/Scripts/Labyrinth");
        RenameFolder("Assets/Scripts/Stage 2", "Assets/Scripts/Genius");

        RenameFolder("Assets/Prefabs/Stage 1", "Assets/Prefabs/Labyrinth");
        RenameFolder("Assets/Materials/Stage 1", "Assets/Materials/Labyrinth");
        RenameFolder("Assets/Resources/Stage 2", "Assets/Resources/Genius");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Stage folder rename attempt finished.");
    }

    private static void RenameFolder(string from, string to)
    {
        if (!AssetDatabase.IsValidFolder(from)) return;
        var parent = System.IO.Path.GetDirectoryName(to).Replace('\\','/');
        var leaf = System.IO.Path.GetFileName(to);
        if (!AssetDatabase.IsValidFolder(parent))
        {
            var gp = System.IO.Path.GetDirectoryName(parent).Replace('\\','/');
            var gl = System.IO.Path.GetFileName(parent);
            if (!AssetDatabase.IsValidFolder(parent) && AssetDatabase.IsValidFolder(gp))
                AssetDatabase.CreateFolder(gp, gl);
        }
        if (!AssetDatabase.IsValidFolder(to))
        {
            AssetDatabase.CreateFolder(parent, leaf);
        }
        var err = AssetDatabase.MoveAsset(from, to);
        if (!string.IsNullOrEmpty(err))
        {
            // Fall back: move children
            var guids = AssetDatabase.FindAssets("*", new[] { from });
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (path == from) continue;
                var file = System.IO.Path.GetFileName(path);
                var target = System.IO.Path.Combine(to, file).Replace('\\','/');
                AssetDatabase.MoveAsset(path, target);
            }
            // Try deleting empty source
            AssetDatabase.DeleteAsset(from);
        }
    }
}
