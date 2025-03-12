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
        [CanBeNull] public List<(Waypoint dest, Waypoint link)> LinkedWaypoints { get; private set; }
        
        public bool isLink;
        
        
        public Vector3 GetPosition()
            => transform.position;

        
        
        
        public void AddLink(Waypoint link, Transform parent, Vector3 curvePoint)
        {
            
            if (LinkedWaypoints is null) 
                LinkedWaypoints = new();

            var containtsIndex = LinkedWaypoints.FindIndex(x => x.dest == link);
            if (containtsIndex != -1)
                LinkedWaypoints.RemoveAt(containtsIndex);
            
            
            Waypoint previousPoint = this;
            for (float t = 0; t < 1; t += 0.05f)
            {
                Vector3 nextPostition = CurveUtils.CalculateCurvePoint(t,
                    GetPosition(),
                    curvePoint,
                    link.GetPosition()
                );
                
                Waypoint newPoint = Create(nextPostition, parent, true);
                
                if (Mathf.Approximately(t, 0.05f))
                    LinkedWaypoints.Add((link, newPoint));
                
                
                newPoint.PreviousWaypoint = previousPoint;

                if (previousPoint != this)
                    previousPoint.NextWaypoint = newPoint;
                
                previousPoint = newPoint;

                if (Mathf.Approximately(t, 0.95f))
                {
                    newPoint.NextWaypoint = link;
                }

            }
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


            if (!isLink)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(GetPosition(), 0.5f);
            }
            
                
            
           
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