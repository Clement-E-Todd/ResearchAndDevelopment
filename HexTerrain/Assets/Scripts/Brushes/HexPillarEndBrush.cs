using System.IO;
using UnityEditor;
using UnityEngine;

public class HexPillarEndBrush : ScriptableObject
{
    public Material[] materials;

    public enum RotationStyle
    {
        DoNotRotate,
        Random
    }
    public RotationStyle rotationStyle;

    [MenuItem("Assets/Create/Hex Pillar End Brush")]
    private static void Create()
    {
        HexPillarEndBrush asset = CreateInstance<HexPillarEndBrush>();

        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (path == "")
        {
            path = "Assets";
        }
        else if (Path.GetExtension(path) != "")
        {
            path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
        }

        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/NewHexPillarEndBrush.asset");

        AssetDatabase.CreateAsset(asset, assetPathAndName);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
}