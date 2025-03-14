using Model;
using RoadSystem;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(Road))]
    public class PathEditor : UnityEditor.Editor
    {
        private Path Path => ((Road)target).Path;


        
        private int? _selectedSegment;
        private int? _selectedControlPoint;
        
        
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

            if (hit.point == Vector3.zero)
                return;
            
            
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
                    float terrainHeight = Terrain.activeTerrain.SampleHeight(hit.point);
                    
                    segment.SetControlPoint(_selectedControlPoint.Value, 
                        new Vector3(hit.point.x, terrainHeight, hit.point.z));
                    
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
                 else
                 {
                     Vector3 previousNode = segment.GetControlPoint(0).GetPosition();
            
                     for (int j = 0; j < segment.NodeAmount; j++)
                     {
                         Vector3 currentPosition = segment.GetNode(j).GetPosition();
                         
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
            
            var segment = Segment.Create(Path.Nodes);

            
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