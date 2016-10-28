using System.IO;
using UnityEditor;
using UnityEngine;

public class HexPillarSideBrush : ScriptableObject
{
    public Material[] materials;

    public enum WrapStyle
    {
        RandomMaterialOnEveryFace,
        WrapFromRandomCorner,
        WrapFromEastCorner,
        WrapFromSouthEastCorner,
        WrapFromSouthWestCorner,
        WrapFromWestCorner,
        WrapFromNorthWestCorner,
        WrapFromNorthEastCorner
    }
    public WrapStyle wrapStyle;

    [MenuItem("Assets/Create/Hex Pillar Side Brush")]
    private static void Create()
    {
        HexPillarSideBrush asset = CreateInstance<HexPillarSideBrush>();

        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (path == "")
        {
            path = "Assets";
        }
        else if (Path.GetExtension(path) != "")
        {
            path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
        }

        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/NewHexPillarSideBrush.asset");

        AssetDatabase.CreateAsset(asset, assetPathAndName);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
}