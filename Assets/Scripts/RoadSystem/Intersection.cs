using System.Collections.Generic;
using System.Linq;
using Model;
using Unity.VisualScripting;
using UnityEngine;

namespace RoadSystem
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class Intersection : MonoBehaviour
    {
        
        private Dictionary<Node, (float roadWidth, Waypoint[] waypoints)> _nodes = new();
        private Transform _waypointParent;
        private Mesh _mesh;
        
        
        private void Awake()
        {
            transform.position = Vector3.zero;
            
            _waypointParent = new GameObject("Waypoints").transform;
            _waypointParent.parent = transform;

            _mesh = new Mesh();
            
            GetComponent<MeshFilter>().mesh = _mesh;
            GetComponent<MeshRenderer>().material = Resources.Load<Material>("RoadMaterial");
        }
        
        
        
        public bool ContainsNode(Node node)
            => _nodes.ContainsKey(node);
        
        
        
        
        public void AddNode(Node node, float roadWidth)
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
            
            
            _nodes.Add(node, (roadWidth, waypointsInRoad.ToArray()));

            
           GenerateIntersection();
            
        }
        

        
        
        public void RemoveNode(Node node)
        {
            
            for (int i = 0; i < _waypointParent.childCount;)
            {
                
                DestroyImmediate(_waypointParent.GetChild(i).gameObject);
                
            }
            
            
            _nodes.Remove(node);
            

            GenerateIntersection();
            
        }
            

        
        private void GenerateIntersection()
        {
            _meshCurves = new();
            
            if (_nodes.Count > 1)
            {
                
                CreateLinks();
                GenerateMesh();
                
                
            }
        }
        
        
        private void CreateLinks()
        {
            
            var connections = new List<Waypoint>();
            
            
            foreach (var n in _nodes)
            {
                
                var waypoints = n.Value.waypoints;
                connections.AddRange(waypoints.Where(wp => wp.PreviousWaypoint is null));
                
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

                        waypoints[i].AddLink(currentConnections[j], _waypointParent);

                    }

                }

            }
            
        }


        
        
        private void GenerateMesh()
        {
            //todo: Handle creating quad (temporary) or implement smooth edges
            if (_nodes.Count == 2)
            {
                Debug.Log("CREATE QUAD");
                return;
            }
            
            
            var centerPoint = Utils.FindCenter(_nodes
                .Select(n => n.Key.GetPosition())
                .ToArray()
            );
            
            
            var cornerPoints = new List<Vector3>();
            foreach (var n in _nodes)
            {
                var node = n.Key;
                var width = n.Value.roadWidth;
                
                cornerPoints.Add(node.GetPosition() + node.transform.right * width);
                cornerPoints.Add(node.GetPosition() + -node.transform.right * width);
            }
            
            
            
            //Start creating mesh.
            var vertices = new List<Vector3>();
            var triangles = new List<int>();

            vertices.Add(centerPoint);
            
            
            //Generate triangles attached to roads.
            for (int i = 0; i < cornerPoints.Count; i+=2)
            {
                
                vertices.Add(cornerPoints[i]);
                vertices.Add(cornerPoints[i+1]);
                
                triangles.Add(vertices.IndexOf(cornerPoints[i]));
                triangles.Add(vertices.IndexOf(cornerPoints[i+1]));
                triangles.Add(vertices.IndexOf(centerPoint));
                
            }

            
            //Generate edge triangles.
             var usedPoints = new List<Vector3>();
            
             for (int i = 0; i < cornerPoints.Count && _nodes.Count > 2; i += 2)
             {
                 if (usedPoints.Any(v => v == cornerPoints[i])) return;
                 
                 
                 //Get closest point
                 Vector3? closestPoint = null;
                 foreach (var corner in cornerPoints)
                 {
                     if (corner == cornerPoints[i] || usedPoints.Any(v => v == corner)) continue; //Kan mss weg?
                 
                     if (!closestPoint.HasValue)
                     {
                         closestPoint = corner;
                         continue;
                     }
                     
                     if (Vector3.Distance(cornerPoints[i], corner) < Vector3.Distance(cornerPoints[i], closestPoint.Value))
                     {
                         closestPoint = corner;
                     }
                     
                     
                     Debug.Log("Looped once");
                     
                     triangles.Add(vertices.IndexOf(cornerPoints[i]));
                     triangles.Add(vertices.IndexOf(closestPoint.Value));
                     triangles.Add(vertices.IndexOf(centerPoint));
                 }
             }
            
             
            _mesh.Clear();
            _mesh.vertices = vertices.ToArray();
            _mesh.triangles = triangles.ToArray();
            
            
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
        }
        
    }
    
}