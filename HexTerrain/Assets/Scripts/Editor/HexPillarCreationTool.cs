using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace HexTerrain
{
    public static class HexPillarCreationTool
    {
        enum PillarCreationState
        {
            None,
            ChooseHorizontalPositions,
            ChooseTopHeight,
            ChooseBottomHeight
        }
        static PillarCreationState pillarCreationState = PillarCreationState.None;
        public static bool creationInProgress
        {
            get { return pillarCreationState != PillarCreationState.None; }
        }

        static Vector3 createPlanePoint;
        const float distanceToHorizonalPlane = 10f;

        static List<HexGrid.Coord> newPillarCoords = new List<HexGrid.Coord>();
        static float newPillarTopHeight;
        static float newPillarBottomHeight;

        static float totalMouseDelta;

        public static void CancelPillarCreation()
        {
            pillarCreationState = PillarCreationState.None;
        }

        public static void UpdatePillarCreationState()
        {
            if (!HexTerrainEditor.selectedTerrain)
            {
                return;
            }

            switch (pillarCreationState)
            {
                case PillarCreationState.ChooseHorizontalPositions:
                    ChooseHorizontalPositionsForNewPillars(HexTerrainEditor.selectedTerrain);
                    break;

                case PillarCreationState.ChooseTopHeight:
                    ChooseTopHeightForNewPillars(HexTerrainEditor.selectedTerrain);
                    break;

                case PillarCreationState.ChooseBottomHeight:
                    ChooseBottomHeightForNewPillars(HexTerrainEditor.selectedTerrain);
                    break;
            }
        }

        static void ChooseHorizontalPositionsForNewPillars(HexTerrain terrain)
        {
            if (!Camera.current)
            {
                pillarCreationState = PillarCreationState.None;
                return;
            }

            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

            createPlanePoint = Camera.current.transform.position + Camera.current.transform.forward * distanceToHorizonalPlane;

            Plane plane = new Plane(terrain.transform.up, createPlanePoint);
            Ray ray = Camera.current.ScreenPointToRay(new Vector2(Event.current.mousePosition.x, Screen.height - Event.current.mousePosition.y));

            float rayDistance;
            if (plane.Raycast(ray, out rayDistance))
            {
                Vector3 cursorWorldPosition = ray.GetPoint(rayDistance);

                Plane topPlane = new Plane(terrain.transform.up, terrain.transform.position + terrain.transform.up * terrain.maxHeight);
                Plane bottomPlane = new Plane(terrain.transform.up, terrain.transform.position + terrain.transform.up * terrain.minHeight);

                Handles.color = new Color(0f, 1f, 0f);
                Handles.DrawLine(
                    cursorWorldPosition - topPlane.normal * topPlane.GetDistanceToPoint(cursorWorldPosition),
                    cursorWorldPosition - bottomPlane.normal * bottomPlane.GetDistanceToPoint(cursorWorldPosition)
                    );

                HexGrid.Coord coord = terrain.GetCoordForWorldPosition(cursorWorldPosition);

                if (Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag)
                {
                    if (Event.current.button == 0 && !newPillarCoords.Contains(coord))
                    {
                        newPillarCoords.Add(coord);
                    }
                    else if (Event.current.button == 1)
                    {
                        pillarCreationState = PillarCreationState.None;
                    }
                }
                else if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
                {
                    pillarCreationState = PillarCreationState.ChooseTopHeight;
                    totalMouseDelta = 0f;
                }

                Handles.color = new Color(0f, 1f, 0.75f);
                newPillarTopHeight = Vector3.Dot(terrain.transform.up, createPlanePoint);
                foreach (HexGrid.Coord newPillarCoord in newPillarCoords)
                {
                    DrawRingAtCoord(terrain, newPillarCoord, newPillarTopHeight, true);
                }

                DrawVerticalLinesAtCoord(terrain, coord);
                DrawRingAtCoord(terrain, coord, newPillarTopHeight, false);
            }
        }

        static void ChooseTopHeightForNewPillars(HexTerrain terrain)
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

            if (Event.current.isMouse)
            {
                totalMouseDelta -= Event.current.delta.y;

                newPillarTopHeight = Vector3.Dot(terrain.transform.up, createPlanePoint);
                newPillarTopHeight += (totalMouseDelta / Screen.height) * (terrain.maxHeight - terrain.minHeight);
                newPillarTopHeight = Mathf.Round(newPillarTopHeight / terrain.heightSnap) * terrain.heightSnap;
                newPillarTopHeight = Mathf.Clamp(newPillarTopHeight, terrain.minHeight, terrain.maxHeight);
            }

            foreach (HexGrid.Coord newPillarCoord in newPillarCoords)
            {
                Handles.color = new Color(0f, 1f, 0.75f);
                DrawVerticalLinesAtCoord(terrain, newPillarCoord);
                Handles.color = IsSpaceAvailable(newPillarCoord, newPillarTopHeight) ? Color.green : Color.red;
                DrawRingAtCoord(terrain, newPillarCoord, newPillarTopHeight, true);
            }

            if (Event.current.type == EventType.MouseUp)
            {
                if (Event.current.button == 0)
                {
                    RemoveInvalidNewPillarCoords(newPillarTopHeight);

                    newPillarBottomHeight = newPillarTopHeight;

                    pillarCreationState = newPillarCoords.Count > 0 ? PillarCreationState.ChooseBottomHeight : PillarCreationState.None;
                    totalMouseDelta = 0f;
                }
                else if (Event.current.button == 1)
                {
                    newPillarCoords.Clear();
                    pillarCreationState = PillarCreationState.None;
                }
            }
        }

        static void ChooseBottomHeightForNewPillars(HexTerrain terrain)
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

            if (Event.current.isMouse)
            {
                totalMouseDelta -= Event.current.delta.y;

                newPillarBottomHeight = newPillarTopHeight;
                newPillarBottomHeight += (totalMouseDelta / Screen.height) * (terrain.maxHeight - terrain.minHeight);
                newPillarBottomHeight = Mathf.Round(newPillarBottomHeight / terrain.heightSnap) * terrain.heightSnap;
                newPillarBottomHeight = Mathf.Clamp(newPillarBottomHeight, terrain.minHeight, newPillarTopHeight);
            }

            foreach (HexGrid.Coord newPillarCoord in newPillarCoords)
            {
                Handles.color = new Color(0f, 1f, 0.75f);
                DrawVerticalLinesAtCoord(terrain, newPillarCoord);
                Handles.color = IsSpaceAvailable(newPillarCoord, newPillarBottomHeight) ? Color.green : Color.red;
                DrawRingAtCoord(terrain, newPillarCoord, newPillarTopHeight, false);
                DrawRingAtCoord(terrain, newPillarCoord, newPillarBottomHeight, true);
            }

            if (Event.current.type == EventType.MouseUp)
            {
                if (Event.current.button == 0)
                {
                    RemoveInvalidNewPillarCoords(newPillarBottomHeight);

                    List<GameObject> newPillarObjects = new List<GameObject>();

                    foreach (HexGrid.Coord newPillarCoord in newPillarCoords)
                    {
                        HexPillar newPillar = terrain.AddNewPillar(newPillarCoord, newPillarTopHeight, newPillarBottomHeight);
                        Undo.RegisterCreatedObjectUndo(newPillar.gameObject, "Create Pillar");
                        newPillarObjects.Add(newPillar.gameObject);
                    }

                    Selection.objects = newPillarObjects.ToArray();

                    newPillarCoords.Clear();
                    pillarCreationState = PillarCreationState.None;
                }
                else if (Event.current.button == 1)
                {
                    newPillarCoords.Clear();
                    pillarCreationState = PillarCreationState.None;
                }
            }
        }

        static void DrawVerticalLinesAtCoord(HexTerrain terrain, HexGrid.Coord coord)
        {
            Plane topPlane = new Plane(terrain.transform.up, terrain.transform.position + terrain.transform.up * terrain.maxHeight);
            Plane bottomPlane = new Plane(terrain.transform.up, terrain.transform.position + terrain.transform.up * terrain.minHeight);

            Vector3 hexLocalPosition = terrain.GetLocalPositionForCoord(coord);

            for (HexCornerDirection direction = 0; direction < HexCornerDirection.MAX; ++direction)
            {
                Vector3 cornerLocalPosition = hexLocalPosition + HexHelper.GetCornerDirectionVector(direction) * terrain.hexRadius;
                Vector3 cornerWorldPosition = terrain.transform.TransformPoint(cornerLocalPosition);

                Handles.DrawLine(
                        cornerWorldPosition - topPlane.normal * topPlane.GetDistanceToPoint(cornerWorldPosition),
                        cornerWorldPosition - bottomPlane.normal * bottomPlane.GetDistanceToPoint(cornerWorldPosition));
            }
        }

        static void DrawRingAtCoord(HexTerrain terrain, HexGrid.Coord coord, float height, bool innerRing)
        {
            Vector3 hexLocalPosition = terrain.GetLocalPositionForCoord(coord);
            height = Mathf.Clamp(height, terrain.minHeight, terrain.maxHeight);

            // Outter ring
            for (HexCornerDirection direction = 0; direction < HexCornerDirection.MAX; ++direction)
            {
                Vector3 corner1LocalPosition = hexLocalPosition + HexHelper.GetCornerDirectionVector(direction) * terrain.hexRadius;
                corner1LocalPosition.y = height;
                Vector3 corner1WorldPosition = terrain.transform.TransformPoint(corner1LocalPosition);

                HexCornerDirection nextDirection = direction + 1;
                if (nextDirection == HexCornerDirection.MAX)
                    nextDirection = 0;

                Vector3 corner2LocalPosition = hexLocalPosition + HexHelper.GetCornerDirectionVector(nextDirection) * terrain.hexRadius;
                corner2LocalPosition.y = height;
                Vector3 corner2WorldPosition = terrain.transform.TransformPoint(corner2LocalPosition);

                Handles.DrawLine(corner1WorldPosition, corner2WorldPosition);
            }

            // Inner ring
            if (innerRing)
            {
                for (HexCornerDirection direction = 0; direction < HexCornerDirection.MAX; ++direction)
                {
                    Vector3 corner1LocalPosition = hexLocalPosition + HexHelper.GetCornerDirectionVector(direction) * terrain.hexRadius / 2f;
                    corner1LocalPosition.y = height;
                    Vector3 corner1WorldPosition = terrain.transform.TransformPoint(corner1LocalPosition);

                    HexCornerDirection nextDirection = direction + 1;
                    if (nextDirection == HexCornerDirection.MAX)
                        nextDirection = 0;

                    Vector3 corner2LocalPosition = hexLocalPosition + HexHelper.GetCornerDirectionVector(nextDirection) * terrain.hexRadius / 2f;
                    corner2LocalPosition.y = height;
                    Vector3 corner2WorldPosition = terrain.transform.TransformPoint(corner2LocalPosition);

                    Handles.DrawLine(corner1WorldPosition, corner2WorldPosition);
                }
            }
        }

        public static void OnCreateNewPillarsPressed()
        {
            if (pillarCreationState == PillarCreationState.None)
            {
                pillarCreationState = PillarCreationState.ChooseHorizontalPositions;
            }
        }

        static bool IsSpaceAvailable(HexGrid.Coord coord, float height)
        {
            if (!HexTerrainEditor.selectedTerrain.pillarGrid.ContainsItemAtCoord(coord))
                return true;

            List<HexPillar> pillarsAtCoord = HexTerrainEditor.selectedTerrain.pillarGrid[coord];

            foreach (HexPillar pillar in pillarsAtCoord)
            {
                if (height <= pillar.topEnd.centerHeight &&
                    height >= pillar.bottomEnd.centerHeight)
                    return false;

                for (HexCornerDirection direction = 0; direction < HexCornerDirection.MAX; ++direction)
                {
                    if (height <= pillar.topEnd.corners[(int)direction].height &&
                        height >= pillar.bottomEnd.corners[(int)direction].height)
                        return false;
                }
            }

            return true;
        }

        static void RemoveInvalidNewPillarCoords(float height)
        {
            List<HexGrid.Coord> newNewPillarCoords = new List<HexGrid.Coord>();

            foreach (HexGrid.Coord newPillarCoord in newPillarCoords)
            {
                if (IsSpaceAvailable(newPillarCoord, height))
                    newNewPillarCoords.Add(newPillarCoord);
            }

            newPillarCoords = newNewPillarCoords;
        }
    }
}