using System;
using Behaviours;
using JetBrains.Annotations;
using Model;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Utils;

namespace Editor
{
    public class PathFindingTestTool : EditorWindow
    {
        private Waypoint _start;
        private Waypoint _end;

        private PathfindingTestBehaviour pathFinderTester;
        
        
        [MenuItem("Tools/Pathfinding Tools")]
        public static void ShowWindow()
        {
            GetWindow<PathFindingTestTool>();
        }

        private void OnEnable()
        {
            Selection.selectionChanged += OnSelectionChanged;
        }

        private void OnDestroy()
        {
            Selection.selectionChanged -= OnSelectionChanged;
        }
            
        
        
        private void OnSelectionChanged()
        {
            pathFinderTester.ClearRoute();

            if (Selection.objects.Length < 1 || Selection.objects.Length > 2) return;
            
            if (Selection.objects[0].GameObject().TryGetComponent(out _start))
                pathFinderTester.SetStart(_start);
            
            if (Selection.objects.Length != 2)
                return;

            if (Selection.objects[1].GameObject().TryGetComponent(out _end))
                pathFinderTester.SetEnd(_end);
        }


        private void OnGUI()
        {
            pathFinderTester = (PathfindingTestBehaviour)EditorGUILayout.ObjectField("Test object", pathFinderTester, typeof(PathfindingTestBehaviour), true);

            
            if (_start is null || _end is null)
            {
                EditorGUILayout.HelpBox("Please select 2 waypoints", MessageType.Info);
                return;
            }
            
            EditorGUILayout.LabelField($"Start point: {_start.name}");
            EditorGUILayout.LabelField($"Start point: {_end.name}");

            if (GUILayout.Button("Calculate Route"))
                pathFinderTester.CalculateRoute();
            
        }
    }
}