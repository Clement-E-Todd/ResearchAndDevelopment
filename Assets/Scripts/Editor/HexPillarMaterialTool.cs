using UnityEngine;
using UnityEditor;

namespace HexTerrain
{
    public static class HexPillarMaterialTool
    {
        public static void UpdateMaterialsMode()
        {
            if (!HexTerrainEditor.selectedTerrain || !HexTerrainEditor.selectedTerrain.currentMaterialBrush)
                return;

            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

            Vector2 mousePosition = new Vector2(Event.current.mousePosition.x, Screen.height - Event.current.mousePosition.y - 38);
            Ray ray = SceneView.currentDrawingSceneView.camera.ScreenPointToRay(mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                HexPillar pillar = hit.collider.GetComponent<HexPillar>();

                if (pillar)
                {
                    // Paint on the floor
                    if (pillar.IsFloorTriangle(hit.triangleIndex))
                    {
                        DrawRingOnFloor(pillar);

                        if (HexTerrainEditor.selectedTerrain.currentMaterialBrush.canPaintFloors &&
                            Event.current.isMouse &&
                            (Event.current.type == EventType.MouseDown || (Event.current.type == EventType.MouseDrag && pillar.topMaterialBrush != HexTerrainEditor.selectedTerrain.currentMaterialBrush)) &&
                            Event.current.button == 0)
                        {
                            pillar.topMaterialBrush = HexTerrainEditor.selectedTerrain.currentMaterialBrush;
                            pillar.SelectTopMaterialFromBrush();
                            pillar.GenerateMesh();
                        }
                    }

                    // Paint on the walls
                    if (pillar.IsSideTriangle(hit.triangleIndex))
                    {
                        DrawSidesOfPillar(pillar);

                        if (HexTerrainEditor.selectedTerrain.currentMaterialBrush.canPaintSides &&
                            Event.current.isMouse &&
                            (Event.current.type == EventType.MouseDown || (Event.current.type == EventType.MouseDrag && pillar.sideMaterialBrush != HexTerrainEditor.selectedTerrain.currentMaterialBrush)) &&
                            Event.current.button == 0)
                        {
                            pillar.sideMaterialBrush = HexTerrainEditor.selectedTerrain.currentMaterialBrush;
                            pillar.SelectSideMaterialsFromBrush();
                            pillar.GenerateMesh();
                        }
                    }

                    // Paint on the ceiling
                    if (pillar.IsCeilingTriangle(hit.triangleIndex))
                    {
                        DrawRingOnCeiling(pillar);

                        if (HexTerrainEditor.selectedTerrain.currentMaterialBrush.canPaintCeilings &&
                            Event.current.isMouse &&
                            (Event.current.type == EventType.MouseDown || (Event.current.type == EventType.MouseDrag && pillar.bottomMaterialBrush != HexTerrainEditor.selectedTerrain.currentMaterialBrush)) &&
                            Event.current.button == 0)
                        {
                            pillar.bottomMaterialBrush = HexTerrainEditor.selectedTerrain.currentMaterialBrush;
                            pillar.SelectBottomMaterialFromBrush();
                            pillar.GenerateMesh();
                        }
                    }
                }
            }
        }

        static void DrawRingOnFloor(HexPillar pillar)
        {
            Handles.color = pillar.terrain.currentMaterialBrush.canPaintFloors ? Color.green : Color.red;

            for (HexEdgeDirection direction = 0; direction < HexEdgeDirection.MAX; ++direction)
            {
                HexCornerDirection counterCorner = HexHelper.GetCornerDirectionNextToEdge(direction, false);
                Vector3 counterCornerVector = HexHelper.GetCornerDirectionVector(counterCorner) * pillar.GetTerrain().hexRadius;
                Vector3 counterPoint = pillar.transform.TransformPoint(counterCornerVector + new Vector3(0, pillar.topEnd.corners[(int)counterCorner].height, 0));

                HexCornerDirection clockwiseCorner = HexHelper.GetCornerDirectionNextToEdge(direction, true);
                Vector3 clockwiseCornerVector = HexHelper.GetCornerDirectionVector(clockwiseCorner) * pillar.GetTerrain().hexRadius;
                Vector3 clockwisePoint = pillar.transform.TransformPoint(clockwiseCornerVector + new Vector3(0, pillar.topEnd.corners[(int)clockwiseCorner].height, 0));

                Handles.DrawLine(counterPoint, clockwisePoint);
            }
        }

        static void DrawRingOnCeiling(HexPillar pillar)
        {
            Handles.color = pillar.terrain.currentMaterialBrush.canPaintCeilings ? Color.green : Color.red;

            for (HexEdgeDirection direction = 0; direction < HexEdgeDirection.MAX; ++direction)
            {
                HexCornerDirection counterCorner = HexHelper.GetCornerDirectionNextToEdge(direction, false);
                Vector3 counterCornerVector = HexHelper.GetCornerDirectionVector(counterCorner) * pillar.GetTerrain().hexRadius;
                Vector3 counterPoint = pillar.transform.TransformPoint(counterCornerVector + new Vector3(0, pillar.bottomEnd.corners[(int)counterCorner].height, 0));

                HexCornerDirection clockwiseCorner = HexHelper.GetCornerDirectionNextToEdge(direction, true);
                Vector3 clockwiseCornerVector = HexHelper.GetCornerDirectionVector(clockwiseCorner) * pillar.GetTerrain().hexRadius;
                Vector3 clockwisePoint = pillar.transform.TransformPoint(clockwiseCornerVector + new Vector3(0, pillar.bottomEnd.corners[(int)clockwiseCorner].height, 0));

                Handles.DrawLine(counterPoint, clockwisePoint);
            }
        }

        static void DrawSidesOfPillar(HexPillar pillar)
        {
            Handles.color = pillar.terrain.currentMaterialBrush.canPaintSides ? Color.green : Color.red;

            for (HexEdgeDirection direction = 0; direction < HexEdgeDirection.MAX; ++direction)
            {
                HexCornerDirection counterCorner = HexHelper.GetCornerDirectionNextToEdge(direction, false);
                Vector3 counterCornerVector = HexHelper.GetCornerDirectionVector(counterCorner) * pillar.GetTerrain().hexRadius;
                
                HexCornerDirection clockwiseCorner = HexHelper.GetCornerDirectionNextToEdge(direction, true);
                Vector3 clockwiseCornerVector = HexHelper.GetCornerDirectionVector(clockwiseCorner) * pillar.GetTerrain().hexRadius;

                //Vertical Line
                Vector3 topPoint = pillar.transform.TransformPoint(counterCornerVector + new Vector3(0, pillar.topEnd.corners[(int)counterCorner].height, 0));
                Vector3 bottomPoint = pillar.transform.TransformPoint(counterCornerVector + new Vector3(0, pillar.bottomEnd.corners[(int)counterCorner].height, 0));
                Handles.DrawLine(topPoint, bottomPoint);

                // Top Line
                Vector3 counterPoint = pillar.transform.TransformPoint(counterCornerVector + new Vector3(0, pillar.topEnd.corners[(int)counterCorner].height, 0));
                Vector3 clockwisePoint = pillar.transform.TransformPoint(clockwiseCornerVector + new Vector3(0, pillar.topEnd.corners[(int)clockwiseCorner].height, 0));
                Handles.DrawLine(counterPoint, clockwisePoint);

                // Bottom Line
                counterPoint = pillar.transform.TransformPoint(counterCornerVector + new Vector3(0, pillar.bottomEnd.corners[(int)counterCorner].height, 0));
                clockwisePoint = pillar.transform.TransformPoint(clockwiseCornerVector + new Vector3(0, pillar.bottomEnd.corners[(int)clockwiseCorner].height, 0));
                Handles.DrawLine(counterPoint, clockwisePoint);
            }
        }
    }
}