﻿using System.Collections.Generic;
using Model;
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
                if (Handles.Button(firstControlPoint.GetPosition(), Quaternion.identity, 1, 1, Handles.DotHandleCap))
                {
                    Intersection.AddNode(firstControlPoint, roadWidth);
                }
                
                var lastControlPoint = currentPath.GetSegment(currentPath.SegmentAmount-1).GetControlPoint(2);
                if (Handles.Button(lastControlPoint.GetPosition(), Quaternion.identity, 1, 1, Handles.DotHandleCap))
                {
                    Intersection.AddNode(lastControlPoint, roadWidth);
                }
                
            }
        }
    }
}