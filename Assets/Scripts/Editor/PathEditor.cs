using System;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Editor
{
    [CustomEditor(typeof(Path))]
    public class PathEditor : UnityEditor.Editor
    {
        private int? _selectedNode;
        
        private void Awake()
        {
            Tools.current = Tool.None;
        }
        
        

        private void OnSceneGUI()
        {
            Path path = (Path)target;
            
            RaycastHit hit;
            Vector3 mousePosition = Event.current.mousePosition;
            Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
            Physics.Raycast(ray, out hit);
            
            
            if (path.NodeCount == 0)
            {
                Vector3 newNode = hit.point;
                _selectedNode = path.NodeCount;
            }
            
            
            for (int i = 0; i < path.NodeCount; i++)
            {
                Vector3 position = path.GetNode(i);
                if (Handles.Button(position, quaternion.identity, 0.2f, 0.2f, Handles.SphereHandleCap))
                {
                    if (_selectedNode == i)
                    {
                        EditorGUI.BeginChangeCheck();
                        path.AddNode(hit.point);
                        _selectedNode = path.NodeCount-1;
                    }
                    else
                    {
                        _selectedNode = i;
                    }
                }
            }
            
            
            if (_selectedNode.HasValue)
            {
                Vector3 nodePostition = path.GetNode(_selectedNode.Value);
                Vector3 newPostition = hit.point;

                if (nodePostition != newPostition)
                {
                    path.SetNode(_selectedNode.Value, newPostition);
                }
            }
        }
    }
}
