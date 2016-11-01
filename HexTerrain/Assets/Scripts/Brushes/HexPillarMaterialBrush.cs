using System.IO;
using UnityEditor;
using UnityEngine;

namespace HexTerrain
{
    public class HexPillarMaterialBrush : ScriptableObject
    {
        public Material[] materials;

        public bool canPaintFloors = true;
        public bool canPaintSides = true;
        public bool canPaintCeilings = true;

        public enum EndPaintStyle
        {
            PerHexWithoutRotation,
            PerHexWithRandomRotation,
            //Overlay   // NOT SUPPORTED YET
        }
        public EndPaintStyle endPaintStyle;
        public float endPaintOverlayScale = 1f;

        public enum SidePaintStyle
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
        public SidePaintStyle sidePaintStyle;


        [MenuItem("Assets/Create/Hex Pillar Material Brush")]
        private static void Create()
        {
            HexPillarMaterialBrush asset = CreateInstance<HexPillarMaterialBrush>();

            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (path == "")
            {
                path = "Assets";
            }
            else if (Path.GetExtension(path) != "")
            {
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }

            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/NewHexPillarMaterialBrush.asset");

            AssetDatabase.CreateAsset(asset, assetPathAndName);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }
    }
}