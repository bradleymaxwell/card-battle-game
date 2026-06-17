using UnityEditor;
using UnityEngine;

namespace Map
{
    [CustomEditor(typeof(MapSpaceContainer))]
    public class MapSpaceContainerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            var spaceContainer = (MapSpaceContainer)target;
            var selectedSpace = GetSelectedSpacePrefab();
            SyncReferenceSpaceWithSelection(spaceContainer, selectedSpace);
            EditorGUILayout.Space();
            DrawAnchorControls(spaceContainer);
            EditorGUILayout.Space();
            DrawReferenceSpaceInfo(selectedSpace);
            EditorGUILayout.Space();
            DrawPreviewDirectionControls(spaceContainer, selectedSpace);
            EditorGUILayout.Space();
            DrawCreateDirectionControls(spaceContainer, selectedSpace);
        }

        private static void SyncReferenceSpaceWithSelection(
            MapSpaceContainer spaceContainer,
            MapSpacePrefab selectedMapSpace)
        {
            if (selectedMapSpace == null)
            {
                return;
            }

            if (!selectedMapSpace.transform.IsChildOf(spaceContainer.transform))
            {
                return;
            }

            if (spaceContainer.ReferenceMapSpace == selectedMapSpace)
            {
                return;
            }

            Undo.RecordObject(spaceContainer, "Set Space Reference");
            spaceContainer.ReferenceMapSpace = selectedMapSpace;
            EditorUtility.SetDirty(spaceContainer);
            SceneView.RepaintAll();
        }

        private static void DrawAnchorControls(MapSpaceContainer spaceContainer)
        {
            EditorGUILayout.LabelField("Anchor", EditorStyles.boldLabel);

            if (GUILayout.Button("Create Anchor Space"))
            {
                CreateAnchorSpace(spaceContainer);
            }

            if (GUILayout.Button("Regenerate Space Positions"))
            {
                RegenerateSpacePositions(spaceContainer);
            }
        }

        private static void RegenerateSpacePositions(MapSpaceContainer spaceContainer)
        {
            Undo.RegisterFullObjectHierarchyUndo(spaceContainer.gameObject, "Regenerate Space Positions");
            spaceContainer.RegenerateSpacePositions();
            EditorUtility.SetDirty(spaceContainer);
            SceneView.RepaintAll();
        }

        private static void DrawReferenceSpaceInfo(MapSpacePrefab selectedMapSpace)
        {
            EditorGUILayout.LabelField("Reference Space", EditorStyles.boldLabel);

            if (selectedMapSpace == null)
            {
                EditorGUILayout.HelpBox(
                    "Select a SpacePrefab child of this SpaceContainer to use it as the reference space.",
                    MessageType.Info);
                return;
            }

            EditorGUILayout.LabelField("Selected", selectedMapSpace.name);
            EditorGUILayout.LabelField("Q", selectedMapSpace.Q.ToString());
            EditorGUILayout.LabelField("R", selectedMapSpace.R.ToString());
        }

        private static void DrawPreviewDirectionControls(
            MapSpaceContainer spaceContainer,
            MapSpacePrefab selectedMapSpace)
        {
            EditorGUILayout.LabelField("Preview", EditorStyles.boldLabel);

            using (new EditorGUI.DisabledScope(selectedMapSpace == null))
            {
                var previewDirection = (MapDirection)EditorGUILayout.EnumPopup(
                    "Direction",
                    spaceContainer.PreviewDirection);

                if (previewDirection != spaceContainer.PreviewDirection)
                {
                    Undo.RecordObject(spaceContainer, "Change Preview Direction");
                    spaceContainer.PreviewDirection = previewDirection;
                    EditorUtility.SetDirty(spaceContainer);
                    SceneView.RepaintAll();
                }

                if (selectedMapSpace != null)
                {
                    var canCreate = spaceContainer.CanCreateSpaceFrom(selectedMapSpace, spaceContainer.PreviewDirection);
                    var message = canCreate
                        ? "Green preview means the target space is available."
                        : "Red preview means the target space is already occupied.";

                    EditorGUILayout.HelpBox(message, canCreate ? MessageType.Info : MessageType.Warning);
                }
            }
        }

        private static void DrawCreateDirectionControls(
            MapSpaceContainer spaceContainer,
            MapSpacePrefab selectedMapSpace)
        {
            EditorGUILayout.LabelField("Create From Reference", EditorStyles.boldLabel);

            using (new EditorGUI.DisabledScope(selectedMapSpace == null))
            {
                DrawDirectionButton(spaceContainer, selectedMapSpace, MapDirection.Left, "Create Left");
                DrawDirectionButton(spaceContainer, selectedMapSpace, MapDirection.Right, "Create Right");
                DrawDirectionButton(spaceContainer, selectedMapSpace, MapDirection.LeftUp, "Create Left Up");
                DrawDirectionButton(spaceContainer, selectedMapSpace, MapDirection.RightUp, "Create Right Up");
                DrawDirectionButton(spaceContainer, selectedMapSpace, MapDirection.LeftDown, "Create Left Down");
                DrawDirectionButton(spaceContainer, selectedMapSpace, MapDirection.RightDown, "Create Right Down");
            }
        }

        private static void DrawDirectionButton(
            MapSpaceContainer spaceContainer,
            MapSpacePrefab selectedMapSpace,
            MapDirection direction,
            string label)
        {
            var canCreate = selectedMapSpace != null && spaceContainer.CanCreateSpaceFrom(selectedMapSpace, direction);
            var previousColor = GUI.color;
            GUI.color = canCreate ? Color.green : Color.red;

            if (GUILayout.Button(label))
            {
                SetPreviewDirection(spaceContainer, direction);

                if (canCreate)
                {
                    CreateSpaceFromSelected(spaceContainer, selectedMapSpace, direction);
                }
                else
                {
                    Debug.LogWarning($"Cannot create space {direction}. That location is already occupied.", spaceContainer);
                }
            }

            GUI.color = previousColor;
        }

        private static void SetPreviewDirection(MapSpaceContainer spaceContainer, MapDirection direction)
        {
            Undo.RecordObject(spaceContainer, "Set Preview Direction");
            spaceContainer.PreviewDirection = direction;
            EditorUtility.SetDirty(spaceContainer);
            SceneView.RepaintAll();
        }

        private static void CreateAnchorSpace(MapSpaceContainer spaceContainer)
        {
            Undo.RegisterFullObjectHierarchyUndo(spaceContainer.gameObject, "Create Anchor Space");

            var createdSpace = spaceContainer.CreateAnchorSpace();

            if (createdSpace == null)
            {
                return;
            }

            Undo.RegisterCreatedObjectUndo(createdSpace.gameObject, "Create Anchor Space");

            Selection.activeGameObject = createdSpace.gameObject;

            Undo.RecordObject(spaceContainer, "Set Space Reference");
            spaceContainer.ReferenceMapSpace = createdSpace;

            EditorUtility.SetDirty(spaceContainer);
            SceneView.RepaintAll();
        }

        private static void CreateSpaceFromSelected(
            MapSpaceContainer spaceContainer,
            MapSpacePrefab selectedMapSpace,
            MapDirection direction)
        {
            Undo.RegisterFullObjectHierarchyUndo(spaceContainer.gameObject, $"Create Space {direction}");

            var createdSpace = spaceContainer.CreateSpaceFrom(selectedMapSpace, direction);

            if (createdSpace == null)
            {
                return;
            }

            Undo.RegisterCreatedObjectUndo(createdSpace.gameObject, $"Create Space {direction}");

            Selection.activeGameObject = createdSpace.gameObject;

            Undo.RecordObject(spaceContainer, "Set Space Reference");
            spaceContainer.ReferenceMapSpace = createdSpace;
            spaceContainer.PreviewDirection = direction;

            EditorUtility.SetDirty(spaceContainer);
            SceneView.RepaintAll();
        }

        private static MapSpacePrefab GetSelectedSpacePrefab()
        {
            if (Selection.activeGameObject == null)
            {
                return null;
            }

            return Selection.activeGameObject.GetComponent<MapSpacePrefab>();
        }
    }
}