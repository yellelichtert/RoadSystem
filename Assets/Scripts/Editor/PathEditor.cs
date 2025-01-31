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
        private Path Path => (Path)target;
        
        private int? _selectedNode;
        private Vector3? _originalPosition;
        
        
        private void Awake()
        {
            Tools.current = Tool.None;
            
        }
        
        

        private void OnSceneGUI()
        {
            
            
            Event e = Event.current;
            
            
            RaycastHit hit;
            Vector3 mousePosition = e.mousePosition;
            Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
            Physics.Raycast(ray, out hit);
            
            
            if (Path.NodeCount == 0)
            {
                CreateNewNode();
            }
            
            //Draw Buttons
            for (int i = 0; i < Path.NodeCount; i++)
            {
                Vector3 position = Path.GetNode(i);
                if (Handles.Button(position, quaternion.identity, 0.2f, 0.2f, Handles.SphereHandleCap))
                {
                    if (_selectedNode == i)
                    {
                        CreateNewNode();
                    }
                    else
                    {
                        _selectedNode = i;
                        _originalPosition = position;
                    }
                }
            }
            
            //Follow Mouse
            if (_selectedNode.HasValue)
            {
                Vector3 nodePostition = Path.GetNode(_selectedNode.Value);
                Vector3 newPostition = hit.point;

                if (nodePostition != newPostition)
                {
                    Path.SetNode(_selectedNode.Value, newPostition);
                }
            }

            
            //Undo Section
            if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Backspace && _selectedNode.HasValue)
            {
                Path.RemoveNode(_selectedNode.Value);

                if (_selectedNode.Value == Path.NodeCount)
                {
                    _selectedNode = Path.NodeCount-1;
                    _originalPosition = Path.GetNode(_selectedNode.Value);
                }
                else
                {
                    _selectedNode = null;
                }
            }
            
            
            //Exit Edit Mode
            if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape && _selectedNode.HasValue)
            {
                
                if (_originalPosition.HasValue)
                {
                    Path.SetNode(_selectedNode!.Value, _originalPosition.Value);
                    _originalPosition = null;
                }
                else
                {
                    Path.RemoveNode(Path.NodeCount-1);
                }
                
                _selectedNode = null;
                Selection.activeObject = null;
            }
        }


        
        private void CreateNewNode()
        {
            Path.AddNode(Vector3.zero);
            _selectedNode = Path.NodeCount-1;
            _originalPosition = null;
        }
    }
}
