﻿using System.Collections.Generic;
using System.Linq;
using Model;
using UnityEngine;

namespace RoadSystem
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class Intersection : MonoBehaviour
    {
        
        private Dictionary<Node, (float roadWidth, Waypoint[] waypoints, bool invertCorners)> _nodes = new();
        private Transform _waypointParent;
        private Mesh _mesh;
        private Vector3 _center;
        
        public int NodeCount => _nodes.Count;
        
        
        private void Awake()
        {
            transform.position = Vector3.zero;
            
            _waypointParent = new GameObject("Waypoints").transform;
            _waypointParent.parent = transform;

            _mesh = new Mesh();
            
            GetComponent<MeshFilter>().mesh = _mesh;

            GetComponent<MeshRenderer>().material = new Material(Resources.Load<Material>("Asphalt"))
            {
                mainTextureScale = new Vector2(Mathf.Max(transform.localScale.x, transform.localScale.z), transform.localScale.y*2)
            };
            
        }
        
        
        public bool ContainsNode(Node node)
            => _nodes.ContainsKey(node);

        
        public void AddNode(Node node, float roadWidth, bool invertCorners = false)
        {
            
            if (ContainsNode(node))
                return;
            
            
            var waypoints = Physics.OverlapSphere(node.GetPosition(), roadWidth);
            var waypointsInRoad = new List<Waypoint>();
            
            
            for (int i = 0; i < waypoints.Length; i++)
            {
                
                if (waypoints[i].gameObject.TryGetComponent(out Waypoint waypoint) && !waypoint.isLink && (waypoint.PreviousWaypoint is null || waypoint.NextWaypoint is null))
                {
                    
                    waypointsInRoad.Add(waypoint);
                    
                }
                
            }  
            
            
            _nodes.Add(node, (roadWidth, waypointsInRoad.ToArray(), invertCorners));

            
           GenerateIntersection();
            
        }
        

        
        
        public void RemoveNode(Node node)
        {
            _nodes.Remove(node);
            GenerateIntersection();
            
        }
            

        
        private void GenerateIntersection()
        {
            
            for (int i = 0; i < _waypointParent.childCount;)
                DestroyImmediate(_waypointParent.GetChild(i).gameObject);
            
            if (_nodes.Count < 2) return;
            
            _center = CurveUtils.FindCenter(_nodes
                .Select(n => n.Key.GetPosition())
                .ToArray()
            );
            
            
            CreateLinks();
            GenerateMesh();
        }
        
        
        private void CreateLinks()
        {
            
            var connections = new List<Waypoint>();
            
            
            foreach (var n in _nodes)
            {
                //Get all possible connections
                var waypoints = n.Value.waypoints;
                var connectingPoint = waypoints.Where(wp => wp.PreviousWaypoint is null);
                
                connections.AddRange(connectingPoint);
                
                //Create u turns
                var connection = waypoints.First(wp => wp.PreviousWaypoint is null);
                var turningLane = waypoints.First(wp => wp.NextWaypoint is null);
                
                turningLane.AddLink(connection, _waypointParent, _center);
            }
            
            
            
            foreach (var n in _nodes)
            {
                
                var waypoints = n.Value.waypoints;
                var currentConnections = connections
                    .Where(wp => !waypoints.Contains(wp))
                    .ToArray();

                for (int i = 0; i < waypoints.Length; i++)
                {

                    if (waypoints[i].NextWaypoint is not null)
                        continue;

                    for (var j = 0; j < currentConnections.Length; j++)
                    {

                        waypoints[i].AddLink(currentConnections[j], _waypointParent, _center);

                    }

                }

            }
            
        }
        
        

        
        private void GenerateMesh()
        {
            
            _mesh.Clear();
            
            //todo: Handle creating quad (temporary) or implement smooth edges
            if (_nodes.Count == 2)
            {
                Debug.Log("CREATE QUAD");
                return;
            }
            
            
            var cornerPoints = new List<Vector3[]>();
            foreach (var n in _nodes)
            {
                var node = n.Key;
                var width = n.Value.roadWidth;


                if (!n.Value.invertCorners)
                {
                    cornerPoints.Add(new []
                    {
                        node.GetPosition() + node.transform.right * width,
                        node.GetPosition() + -node.transform.right * width
                    });
                }
                else
                {
                    cornerPoints.Add(new []
                    {
                        node.GetPosition() + -node.transform.right * width,
                        node.GetPosition() + node.transform.right * width
                    });
                }
            }
            
            
            //Start creating mesh.
            var vertices = new List<Vector3>();
            var triangles = new List<int>();
            var uvs = new List<Vector2>();

            vertices.Add(_center);
            
            
            //Generate triangles attached to roads.
            for (int i = 0; i < cornerPoints.Count; i++)
            {
                
                vertices.Add(cornerPoints[i][0]);
                vertices.Add(cornerPoints[i][1]);
                
                triangles.Add(vertices.IndexOf(cornerPoints[i][0]));
                triangles.Add(vertices.IndexOf(cornerPoints[i][1]));
                triangles.Add(vertices.IndexOf(_center));
                
            }
            
            
            //Generate edge triangles.
            for (int i = 0; i < cornerPoints.Count; i++)
            {
                //Offset makes sure the correct corner gets selected.
                var cornerWithOffset = cornerPoints[i][1] + -_nodes.ElementAt(i).Key.transform.right * 4;
                
                Vector3? closestPoint = null;
                for (int j = 0; j < cornerPoints.Count; j++)
                {
                    if (cornerPoints[i] == cornerPoints[j]) continue;
                    
                    
                    if (!closestPoint.HasValue || Vector3.Distance(cornerWithOffset, cornerPoints[j][0]) < Vector3.Distance(cornerWithOffset, closestPoint.Value))
                    {
                        closestPoint = cornerPoints[j][0];
                    }
                    
                }
                
                triangles.Add(vertices.IndexOf(cornerPoints[i][1]));
                triangles.Add(vertices.IndexOf(closestPoint.Value));
                triangles.Add(vertices.IndexOf(_center));

            }

            
            for (int i = 0; i < vertices.Count; i++)
            {
                vertices[i] += Vector3.up * 0.01f; //Fix z-fighting
            }
            
            
            _mesh.vertices = vertices.ToArray();
            _mesh.triangles = triangles.ToArray();
            _mesh.RecalculateNormals();
            _mesh.RecalculateBounds();
            
            Bounds bounds = _mesh.bounds;
            for (int i = 0; i < vertices.Count; i++)
            {
                uvs.Add(new Vector2(vertices[i].x / bounds.size.x, vertices[i].z / bounds.size.z));
            }
            
            _mesh.uv = uvs.ToArray();
            
        }
        
        
        
        private void OnDrawGizmosSelected()
        {
            
            if (!_nodes.Any()) 
                return;

            
            Gizmos.color = Color.cyan;
            
            
            foreach (var n in _nodes)
            {
                
                var waypoints = n.Value.waypoints;
                
                for (int j = 0; j < waypoints.Length; j++)
                {
                    
                    Gizmos.DrawSphere(waypoints[j].GetPosition(), 2f);
                    
                }
                
            }

            Gizmos.DrawSphere(_center,  1);
            
        }
    }
}