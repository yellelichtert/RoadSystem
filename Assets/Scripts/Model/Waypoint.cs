﻿using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

namespace Model
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(SphereCollider))]
    public class Waypoint : MonoBehaviour
    {
        [CanBeNull] public Waypoint PreviousWaypoint { get; set; }
        [CanBeNull] public Waypoint NextWaypoint { get; set; }
        [CanBeNull] public List<Waypoint[]> LinkedWaypoints { get; private set; }
        
        public bool isLink;
        
        
        public Vector3 GetPosition()
            => transform.position;

        
        
        
        public void AddLink(Waypoint link, Transform parent)
        {
            
            if (LinkedWaypoints is null) 
                LinkedWaypoints = new();

            
            
            List<Waypoint> linkedPoints = new();

            
            
            Vector3 middlePoint = Utils.FindIntersectionPoint(
                GetPosition(), transform.forward
                ,link.GetPosition(), -link.transform.forward);
            
            
            Waypoint previousPoint = null;
            for (float t = 0; t < 1; t += 0.05f)
            {
                Vector3 nextPostition = Utils.CalculateCurvePoint(t,
                    GetPosition(),
                    middlePoint,
                    link.GetPosition()
                );
                
                Waypoint newPoint = Create(nextPostition, parent, true);
                
                newPoint.PreviousWaypoint = previousPoint ?? this;
                previousPoint = newPoint;
            
                linkedPoints.Add(newPoint);
                
            }
            
            
            LinkedWaypoints.Add(linkedPoints.ToArray());
            
        }
        
        
        

        public static Waypoint Create(Vector3 position, Transform parent, bool isLink = false)
        {
            
            Waypoint waypoint = new GameObject($"Waypoint #{parent.childCount+1}")
                .AddComponent<Waypoint>();
            
            
            waypoint.transform.position = position;
            waypoint.transform.parent = parent;
            waypoint.isLink = isLink;
            
            
            return waypoint;
            
        }


        private void OnDrawGizmos()
        {
            
            
            Gizmos.color = isLink ? Color.blue : Color.red;
            Gizmos.DrawSphere(GetPosition(), 0.5f);
                
            
           
            if (PreviousWaypoint is not null)
            {
                
                if (PreviousWaypoint.IsDestroyed())
                    return;
                
                
                Gizmos.color = isLink ? Color.blue : Color.green; 
                Gizmos.DrawLine(GetPosition(), PreviousWaypoint.GetPosition());
                
            }
            
        }
        
    }
    
}