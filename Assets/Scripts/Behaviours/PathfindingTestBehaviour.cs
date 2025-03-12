using System;
using JetBrains.Annotations;
using Model;
using UnityEngine;
using Utils;

namespace Behaviours
{
    [ExecuteInEditMode]
    public class PathfindingTestBehaviour : MonoBehaviour
    {
        [CanBeNull] private Waypoint[] _route;
        [CanBeNull] private Waypoint _start;
        [CanBeNull] private Waypoint _end;


        public void SetStart(Waypoint point)
            => _start = point;

        public void SetEnd(Waypoint point)
            => _end = point;
        
        public void CalculateRoute()
            => _route = Pathfinding.CalculateRoute(_start, _end);

        public void ClearRoute()
        {
            _start = null;
            _end = null;
            _route = null;
        }
            
        
        private void OnDrawGizmos()
        {

            Gizmos.color = Color.yellow;
            if (_start is not null)
                Gizmos.DrawSphere(_start.GetPosition(), 1);
            
            if (_end is not null)
                Gizmos.DrawSphere(_end.GetPosition(), 1);
            
            
            
            if (_route is null) return;
            
            for (int i = 0; i < _route.Length; i++)
            {
                Gizmos.DrawSphere(_route[i].GetPosition(), 1);
            }
        }
    }
}