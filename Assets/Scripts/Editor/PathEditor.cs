using System;
using Model;
using RoadComponent;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(Road))]
    public class PathEditor : UnityEditor.Editor
    {
        private Path Path => ((Road)target).Path;
        
        
        private SegmentType _selectedSegmentType;
        private enum SegmentType 
        {Straight, Curved, ComplexCurved}
        
        
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
                    Vector3 controlPoint = segment.GetControlPoint(j).GetPosition();
                    
                    if (Handles.Button(controlPoint, Quaternion.identity, 0.5f, 0.5f,  Handles.DotHandleCap))
                    {
                        bool isCurrentlySelected = _selectedSegment == i && _selectedControlPoint == j;


                        if (segment.IsCompleted && e.shift)
                        {
                            Selection.activeObject = null;
                        }
                        else if (segment.IsCompleted)
                        {
                            CreateSegment();
                        }
                        else if (isCurrentlySelected)
                        {
                            Node node = Node.Create(hit.point, Path.ControlPoints); 
                            segment.AddControlPoint(node);
                            
                            
                            _selectedControlPoint = segment.ControlPointAmount-1;
                        }
                    }
                }
            }
            
            
            //Follow mouse
            if (_selectedSegment is not null && _selectedControlPoint is not null)
            {
                
                Segment segment = Path.GetSegment(_selectedSegment.Value);
                
                Node currentNode = segment.GetControlPoint(_selectedControlPoint.Value);

                if (currentNode.GetPosition() != hit.point)
                {
                    segment.SetControlPoint(_selectedControlPoint.Value, hit.point);
                    Path.PathChanged?.Invoke();
                }
            }
            
            
            
            // Draw lines
             for (int i = 0; i < Path.SegmentAmount; i++)
             {
                 Segment segment = Path.GetSegment(i);
                 
                 if (segment.ControlPointAmount == 1) return;
                 
                 if (segment.ControlPointAmount == 2)
                 {
                     
                     Handles.DrawLine(
                         segment.GetControlPoint(0).GetPosition(),
                         segment.GetControlPoint(1).GetPosition());
                     
                 }
                 else if (segment is CurvedSegment curvedSegment)
                 {
                     Vector3 previousNode = segment.GetControlPoint(0).GetPosition();
            
                     for (int j = 0; j < curvedSegment.NodeAmount; j++)
                     {
                         Vector3 currentPosition = curvedSegment.GetNode(j).GetPosition();
                         
                         Handles.DrawLine(currentPosition, previousNode);
                         previousNode = currentPosition;
                     }
                     
                     Vector3 lastControlPoint = segment.GetControlPoint(segment.ControlPointAmount-1).GetPosition();
                     Handles.DrawLine(lastControlPoint, previousNode);
                     
                 }
             }
            
        }




        private void CreateSegment()
        {
            Segment segment = _selectedSegmentType switch
            {
                SegmentType.Straight => new Segment(),
                SegmentType.Curved => new SimpleCurvedSegment(Path.Nodes),
                SegmentType.ComplexCurved => new ComplexCurvedSegment(Path.Nodes),
                _ => throw new ArgumentOutOfRangeException()
            };

            
            if (_selectedSegment is not null && Path.GetSegment(_selectedSegment.Value).IsCompleted)
            {
                
                Node lastcontrolPoint = Path.GetSegment(_selectedSegment.Value)
                    .GetControlPoint(Path.GetSegment(_selectedSegment.Value).ControlPointAmount-1);
                
                segment.AddControlPoint(lastcontrolPoint);
                
            }
            else if (_selectedSegment is not null)
            {
                
                Path.RemoveSegment(_selectedSegment.Value);
                
            }
            
            segment.AddControlPoint(Node.Create(Vector3.zero, Path.ControlPoints));
            
            Path.AddSegment(segment);

            _selectedSegment = Path.SegmentAmount-1;
            _selectedControlPoint = segment.ControlPointAmount-1;
        }
    }
}