using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace HexTerrain
{
    public static class HexPillarEditor
    {
        public static void OnSelectionModePillars()
        {
            foreach (HexPillar selectedPillar in HexTerrainEditor.selectedPillars)
            {
                float delta = Handle(selectedPillar);

                if (delta != 0f)
                {
                    HexPillarEndEditor.MoveEndByAmount(selectedPillar.topEnd, delta, true);
                    HexPillarEndEditor.MoveEndByAmount(selectedPillar.bottomEnd, delta, true);
                    HexTerrainEditor.RedrawSelections();
                }
            }
        }

        public static void OnSelectionModeVertices()
        {
            foreach (HexPillar selectedPillar in HexTerrainEditor.selectedPillars)
            {
                for (HexCornerDirection direction = 0; direction < HexCornerDirection.MAX; ++direction)
                {
                    HexPillarCornerEditor.SelectionButton(selectedPillar, direction, true);
                    HexPillarCornerEditor.SelectionButton(selectedPillar, direction, false);
                }

                HexPillarEndEditor.SelectionButton(selectedPillar, true);
                HexPillarEndEditor.SelectionButton(selectedPillar, false);
            }
        }

        static float Handle(HexPillar pillar)
        {
            Vector3 positionBefore = pillar.transform.TransformPoint((pillar.topEnd.transform.localPosition + pillar.bottomEnd.transform.localPosition) / 2);
            Vector3 direction = pillar.transform.TransformDirection(pillar.transform.up);

            Handles.color = new Color(1f, 0.75f, 0f);

            EditorGUI.BeginChangeCheck();
            Vector3 positionAfter = Handles.Slider(positionBefore, direction, 0.25f, Handles.CylinderCap, 0f);
            if (EditorGUI.EndChangeCheck())
            {
                foreach (HexPillarEnd selectedEnd in HexTerrainEditor.selectedEnds)
                {
                    Undo.RecordObject(selectedEnd, "Edit Pillar");
                }
            }

            Vector3 positionDelta = positionAfter - positionBefore;
            return Vector3.Dot(positionDelta, direction);
        }

        public static bool ShouldHideUnityTools()
        {
            return HexTerrainEditor.selectedPillars != null && (HexTerrainEditor.selectedPillars.Length > 0 || HexPillarCreationTool.creationInProgress);
        }
    }
}