using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Road
{
    public class Waypoint : MonoBehaviour
    {
        [CanBeNull] private Waypoint previousWaypoint;
        [CanBeNull] private Waypoint nextWaypoint;

        public Vector3 GetPosition()
            => transform.position;

        public void SetPreviousPoint(Waypoint point)
            => previousWaypoint = point;
        
        public void SetNextPoint(Waypoint point)
            => nextWaypoint = point;
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(GetPosition(), 0.5f);

            if (previousWaypoint != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(GetPosition(), previousWaypoint.GetPosition());
            }
        }
    }
}