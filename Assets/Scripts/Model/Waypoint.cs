using System.Collections.Generic;
using JetBrains.Annotations;
using NUnit.Framework;
using UnityEngine;

namespace Model
{
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

            Waypoint previousPoint = null;
            for (float t = 0; t < 1; t += 0.05f)
            {
                Vector3 nextPostition = Vector3.Lerp(GetPosition(), link.GetPosition(), t);
                
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
               Gizmos.color = isLink ? Color.blue : Color.green; 
               Gizmos.DrawLine(GetPosition(), PreviousWaypoint.GetPosition());
           }
           
        }
    }
}