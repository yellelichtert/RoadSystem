using System.Collections.Generic;
using System.Linq;
using Model;
using NUnit.Framework;
using UnityEngine;

namespace RoadSystem
{
    [ExecuteInEditMode]
    public class Intersection : MonoBehaviour
    {
        private Dictionary<Node, Waypoint[]> _nodes = new();
        private Transform _waypointParent;

        private void Awake()
        {
            _waypointParent = new GameObject("Waypoints").transform;
            _waypointParent.parent = transform;
        }


        public bool ContainsNode(Node node)
            => _nodes.ContainsKey(node);
        
        
        public void AddNode(Node node, float roadWidth)
        {
            if (ContainsNode(node))
                return;
            
            Collider[] waypoints = Physics.OverlapSphere(node.GetPosition(), roadWidth);
            
            List<Waypoint> waypointsInRoad = new List<Waypoint>();
            for (int i = 0; i < waypoints.Length; i++)
            {
                if (waypoints[i].gameObject.TryGetComponent(out Waypoint waypoint) && (waypoint.PreviousWaypoint is null || waypoint.NextWaypoint is null))
                {
                    waypointsInRoad.Add(waypoint);
                }
            }  
            
            _nodes.Add(node, waypointsInRoad.ToArray());

            if (_nodes.Count > 1)
            {
                CreateLinks();
                GenerateMesh();
            }
            
        }


        public void RemoveNode(Node node)
        {
            for (int i = 0; i < _waypointParent.childCount;)
            {
                Debug.Log("Destroying point :" + _waypointParent.GetChild(i).name);
                DestroyImmediate(_waypointParent.GetChild(i).gameObject);
            }
            
            
            _nodes.Remove(node);

            if (_nodes.Count > 1)
            {
                CreateLinks();
                GenerateMesh();
            }
        }
            

        private void CreateLinks()
        {
            List<Waypoint> connections = new();
            foreach (var (_, waypoint) in _nodes)
            {
                connections.AddRange(waypoint.Where(wp => wp.PreviousWaypoint is null));
            }
            
            
            foreach (var (_, waypoints) in _nodes)
            {
                Waypoint[] currentConnections = connections
                    .Where(wp => !waypoints.Contains(wp))
                    .ToArray();

                for (int i = 0; i < waypoints.Length; i++)
                {
                    if (waypoints[i].NextWaypoint is not null)
                        continue;

                    for (var j = 0; j < currentConnections.Length; j++)
                    {
                        waypoints[i].AddLink(currentConnections[j], _waypointParent);
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
            if (!_nodes.Any())
                return;

            Gizmos.color = Color.cyan;
            foreach (var (node, waypoints) in _nodes)
            {
                for (int j = 0; j < waypoints.Length; j++)
                {
                    Gizmos.DrawSphere(waypoints[j].GetPosition(), 2f);
                }
            }
        }
    }
}