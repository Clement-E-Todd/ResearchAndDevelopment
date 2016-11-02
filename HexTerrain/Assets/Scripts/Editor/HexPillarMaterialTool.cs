using UnityEngine;
using UnityEditor;

namespace HexTerrain
{
    public static class HexPillarMaterialTool
    {
        public static void DebugLog()
        {
            Vector2 mousePosition = new Vector2(Event.current.mousePosition.x, Screen.height - Event.current.mousePosition.y - 38);
            Ray ray = SceneView.currentDrawingSceneView.camera.ScreenPointToRay(mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                HexPillar pillar = hit.collider.GetComponent<HexPillar>();

                if (pillar)
                {
                    string message = hit.triangleIndex.ToString() + ": ";

                    if (pillar.IsFloorTriangle(hit.triangleIndex))
                    {
                        message += "FLOOR";
                        DrawRingOnFloor(pillar);
                    }

                    if (pillar.IsSideTriangle(hit.triangleIndex))
                        message += "SIDE";

                    if (pillar.IsCeilingTriangle(hit.triangleIndex))
                    {
                        message += "CEILING";
                        DrawRingOnCeiling(pillar);
                    }

                    Debug.Log(message);
                }
            }
        }

        static void DrawRingOnFloor(HexPillar pillar)
        {
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
    }
}