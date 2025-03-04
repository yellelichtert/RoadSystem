using RoadSystem;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(Intersection))]
    public class IntersectionEditor : UnityEditor.Editor
    {
        private Intersection Intersection => (Intersection)target;
        
        
        private void OnSceneGUI()
        {
          
            Road[] roads = FindObjectsByType<Road>(FindObjectsSortMode.None);
            
            for (int i = 0; i < roads.Length; i++)
            {
                Path currentPath = roads[i].Path;
                float roadWidth = roads[i].GetRoadWidth();
                
                
                var firstControlPoint = currentPath.GetSegment(0).GetControlPoint(0);
                
                Handles.color = Intersection.ContainsNode(firstControlPoint)
                    ? Color.green
                    : Color.white;
                
                if (Handles.Button(firstControlPoint.GetPosition(), Quaternion.identity, 1, 1, Handles.DotHandleCap))
                {
                    Intersection.AddNode(firstControlPoint, roadWidth);
                }
                
                var lastControlPoint = currentPath.GetSegment(currentPath.SegmentAmount-1).GetControlPoint(2);
                
                Handles.color = Intersection.ContainsNode(lastControlPoint)
                    ? Color.green
                    : Color.white;
                    
                if (Handles.Button(lastControlPoint.GetPosition(), Quaternion.identity, 1, 1, Handles.DotHandleCap))
                {
                    if (!Intersection.ContainsNode(lastControlPoint))
                    {
                        Intersection.AddNode(lastControlPoint, roadWidth);
                    }
                    else
                    {
                        Intersection.RemoveNode(lastControlPoint);
                    }
                }
                
            }
        }
    }
}