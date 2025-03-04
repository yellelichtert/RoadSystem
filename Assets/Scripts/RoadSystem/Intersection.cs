using System.Collections.Generic;
using System.Linq;
using Model;
using UnityEngine;

namespace RoadSystem
{
    [ExecuteInEditMode]
    public class Intersection : MonoBehaviour
    {
        private List<Node> _nodes = new();
        private List<Waypoint[]> _waypoints = new();

        private Transform _waypointParent;

        private void Awake()
        {
            _waypointParent = new GameObject("Waypoints").transform;
            _waypointParent.parent = transform;
        }


        public bool ContainsNode(Node node)
            => _nodes.Contains(node);
        
        
        public void AddNode(Node node, float roadWidth)
        {
            if (!ContainsNode(node))
                _nodes.Add(node);
            
            Collider[] waypoints = Physics.OverlapSphere(node.GetPosition(), roadWidth);
            
            List<Waypoint> waypointsInRoad = new List<Waypoint>();
            for (int i = 0; i < waypoints.Length; i++)
            {
                if (waypoints[i].gameObject.TryGetComponent(out Waypoint waypoint) && (waypoint.PreviousWaypoint is null || waypoint.NextWaypoint is null))
                {
                    waypointsInRoad.Add(waypoint);
                }
            }

            _waypoints.Add(waypointsInRoad.ToArray());
            
            CreateLinks();
            GenerateMesh();
            
        }


        public void RemoveNode(Node node)
        {
            Debug.Log("Removing Node");
            
            _nodes.Remove(node);
            
            CreateLinks();
            GenerateMesh();
        }
            

        private void CreateLinks()
        {
            Debug.Log("Creating links");
            
            //Todo: Destroy all link waypoints before creating new links
            
            //Find empty NextPoints
            List<Waypoint> emptyNext = new List<Waypoint>();
            for (int i = 0; i < _waypoints.Count; i++)
            {
                emptyNext.AddRange(_waypoints[i].Where(wp => wp.NextWaypoint is null));
            }


            for (int i = 0; i < _waypoints.Count; i++)
            {
                var connections = emptyNext
                    .Where(wp => !_waypoints[i].Contains(wp))
                    .ToArray();


                var currentPoints = _waypoints[i]
                    .Where(wp => wp.PreviousWaypoint is null)
                    .ToArray();

                for (int c = 0; c < currentPoints.Length; c++)
                {
                    for (int j = 0; j < connections.Length; j++)
                    {
                        connections[j].AddLink(currentPoints[c], _waypointParent);
                    }
                }
            }
        }
        
        
        
        private void GenerateMesh()
        {
            Debug.Log("Generating Intersection Mesh");
            //Todo: Implement Intersection mesh generation.
        }


        private void OnDrawGizmosSelected()
        {
            if (!_waypoints.Any())
                return;

            Gizmos.color = Color.cyan;
            for (int i = 0; i < _waypoints.Count; i++)
            {
                Waypoint[] currentRoad = _waypoints[i];

                for (int j = 0; j < currentRoad.Length; j++)
                {
                    Gizmos.DrawSphere(currentRoad[j].GetPosition(), 2f);
                }
            }
            
        }
    }
}