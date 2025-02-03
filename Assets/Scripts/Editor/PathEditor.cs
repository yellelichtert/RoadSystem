using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(Path))]
    public class PathEditor : UnityEditor.Editor
    {
        private Path Path => (Path)target;
        
        private int? _selectedSegment;
        private int? _selectedControlPoint;
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
                        
                        if (segment.ControlPointAmount == segment.MaxControlPointAmount)
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
                    Debug.Log("Selected segment" + _selectedSegment);
                    Debug.Log("Controlpoint count: " + segment.ControlPointAmount);
                    Debug.Log("Selected Control point:" + _selectedControlPoint);
                    
                    segment.SetControlPoint(_selectedControlPoint.Value, hit.point);
                }
            }
        }


        
        private void CreateSegment(Vector3? firstPoint = null)
        {
            Segment segment = new Segment();
            
            if (firstPoint is not null)
            {
                segment.AddControlPoint(firstPoint.Value);
            }
            
            segment.AddControlPoint(Vector3.zero);
            
            Path.AddSegment(segment);

            _selectedSegment = Path.SegmentAmount-1;
            _selectedControlPoint = segment.ControlPointAmount-1;

        }
    }
}
