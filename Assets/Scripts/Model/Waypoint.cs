using JetBrains.Annotations;
using UnityEngine;

namespace Model
{
    public class Waypoint
    {
        public Vector3 Position { get;}
        
        [CanBeNull] public Waypoint PreviousWaypoint { get; set; }
        [CanBeNull] public Waypoint NextWaypoint { get; set; }
        
        
        public Waypoint(Vector3 position)
        {
            Position = position;
        }
    }
}