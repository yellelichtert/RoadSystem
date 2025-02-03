using System;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(Path))]
    public class PathEditor : UnityEditor.Editor
    {
        private Path Path => (Path)target;
        
        
        private SegmentType _selectedSegmentType;
        private enum SegmentType 
        {Straight, Curved}
        
        
        private int? _selectedSegment;
        private int? _selectedControlPoint;
        
        
        private void Awake()
        {
            
            Tools.current = Tool.None;
            
        }


        public override void OnInspectorGUI()
        {
            
            GUILayout.Label("Current segment type: " + _selectedSegmentType);
            
            GUILayout.BeginHorizontal();
            
            SegmentType[] type = (SegmentType[]) Enum.GetValues(typeof(SegmentType));
            for (int i = 0; i < type.Length; i++)
            {
                
                if (GUILayout.Button(type[i].ToString()))
                {
                    
                    _selectedSegmentType = type[i];
                    CreateSegment();
                    
                }
                
            }
            
            GUILayout.EndHorizontal();
            
            base.OnInspectorGUI();
        }


        private void OnSceneGUI()
        {
            Event e = Event.current;
            
            RaycastHit hit;
            Vector3 mousePosition = e.mousePosition;
            Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
            Physics.Raycast(ray, out hit);
            
            
            if (Path.SegmentAmount == 0)
            {
                CreateSegment();
            }
            
            
            //Draw buttons
            for (int i = 0; i < Path.SegmentAmount; i++)
            {
                Segment segment = Path.GetSegment(i);
                
                for (int j = 0; j < segment.ControlPointAmount; j++)
                {
                    Vector3 controlPoint = segment.GetControlPoint(j);
                    
                    if (Handles.Button(controlPoint, Quaternion.identity, 0.5f, 0.5f,  Handles.DotHandleCap))
                    {
                        bool isCurrentlySelected = _selectedSegment == i && _selectedControlPoint == j;
                        
                        
                        if (segment.ControlPointAmount == segment.GetMaxControlPoints())
                        {
                            CreateSegment(controlPoint);
                        }
                        else if (isCurrentlySelected)
                        {
                            segment.AddControlPoint(hit.point);
                            _selectedControlPoint = segment.ControlPointAmount-1;
                        }
                    }
                }
            }
            
            
            //Follow mouse
            if (_selectedSegment is not null && _selectedControlPoint is not null)
            {
                
                Segment segment = Path.GetSegment(_selectedSegment.Value);
                
                Vector3 currentPostion = segment.GetControlPoint(_selectedControlPoint.Value);

                if (currentPostion != hit.point)
                {
                    segment.SetControlPoint(_selectedControlPoint.Value, hit.point);
                }
                
            }
            
            
            
            //Draw lines
            for (int i = 0; i < Path.SegmentAmount; i++)
            {
                Segment segment = Path.GetSegment(i);
                
                if (segment.ControlPointAmount == 1) return;
                
                if (segment.ControlPointAmount == 2)
                {
                    Handles.DrawLine(segment.GetControlPoint(0), segment.GetControlPoint(1));
                }
                else if (segment is CruvedSegment curvedSegment)
                {
                    Vector3 previousNode = segment.GetControlPoint(0);

                    for (int j = 0; j < curvedSegment.NodeAmount; j++)
                    {
                        Vector3 currentNode = curvedSegment.GetNode(j);
                        
                        Handles.DrawLine(currentNode, previousNode);
                        previousNode = currentNode;
                    }
                    
                    Vector3 lastControlPoint = segment.GetControlPoint(segment.ControlPointAmount-1);
                    Handles.DrawLine(lastControlPoint, previousNode);
                    
                }
            }
            
        }




        private void CreateSegment(Vector3? firstPoint = null)
        {
            Segment segment = _selectedSegmentType switch
            {
                SegmentType.Straight => new Segment(),
                SegmentType.Curved => new SimpleCurvedSegment(),
                _ => throw new ArgumentOutOfRangeException()
            };

            if (_selectedSegment is not null && Path.GetSegment(_selectedSegment.Value).IsCompleted)
            {
                
                Vector3 lastcontrolPoint = Path.GetSegment(_selectedSegment.Value)
                    .GetControlPoint(Path.GetSegment(_selectedSegment.Value).ControlPointAmount-1);
                
                segment.AddControlPoint(lastcontrolPoint);
                
            }
            else if (_selectedSegment is not null)
            {
                
                Path.RemoveSegment(_selectedSegment.Value);
                
            }
            
            segment.AddControlPoint(Vector3.zero);
            
            Path.AddSegment(segment);

            _selectedSegment = Path.SegmentAmount-1;
            _selectedControlPoint = segment.ControlPointAmount-1;
        }
    }
}
