using System.Collections.Generic;
using Model;
using UnityEngine;

namespace RoadComponent
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class Road : MonoBehaviour
    {
        [SerializeField] private int laneCount = 1;
        [SerializeField] private float laneWidth = 8;
        [SerializeField] private bool oneWay;
        
        
        private List<Waypoint> _waypoints = new();

        public Path Path { get; private set; }
        private Mesh _mesh;
        
        private void Awake()
        {
            transform.position = Vector3.zero;
            
            _mesh = new Mesh() { name = "RoadMesh" };
            GetComponent<MeshFilter>().mesh = _mesh;

            Transform nodes = new GameObject("Nodes").transform;
            Transform controlPoints = new GameObject("ControlPoints").transform;

            nodes.parent = transform;
            controlPoints.parent = transform;

            Path = new Path(nodes, controlPoints);
            
            Path.PathChanged += PathChanged;
        }

        
        
        private void PathChanged()
        {
         
            //Handle change
            List<Node> nodes = new();
            _waypoints = new();

            for (int s = 0; s < Path.SegmentAmount; s++)
            {
                Segment selectedSegment = Path.GetSegment(s);

                if (selectedSegment.ControlPointAmount < 2) return;

                if (selectedSegment is CurvedSegment curvedSegment)
                {
                    for (int n = 0; n < curvedSegment.NodeAmount; n++)
                    {
                        nodes.Add(curvedSegment.GetNode(n));
                    }
                }
                else
                {
                    nodes.Add(selectedSegment.GetControlPoint(0));
                    nodes.Add(selectedSegment.GetControlPoint(1));
                }
                
            }
            
            GenerateWaypoints(nodes.ToArray());

            if (!oneWay)
            {
                nodes.Reverse();
                GenerateWaypoints(nodes.ToArray(), true);
            }
            
            
            GenerateMesh();
        }
        
        
        
        private void GenerateWaypoints(Node[] points, bool left = false)
        {
            
            for (int i = 0; i < laneCount; i++)
            {

                Waypoint? previousPoint = null;
                
                
                for (int p = 0; p < points.Length; p++)
                {
                    Vector3 newPosition = points[p].transform.TransformPoint(
                        (left ? Vector3.right : Vector3.left) * ((laneWidth / 2) + (laneWidth*i) ));

                    
                    Waypoint wp = new Waypoint(newPosition);
                    _waypoints.Add(wp);
                    

                    if (previousPoint is not null)
                    {
                        wp.PreviousWaypoint = previousPoint;
                        previousPoint.NextWaypoint = wp;
                    }
                    
                    previousPoint = wp;
                }
            }
        }



        private void GenerateMesh()
        {
            
            List<Vector3> vertices = new();
            List<int> triangles = new();
            
            for (int i = 0; i < Path.SegmentAmount; i++)
            {
                Segment segment = Path.GetSegment(i);

                if (segment is CurvedSegment curvedSegment)
                {
                    Node startingNode = segment.GetControlPoint(0);

                    for (int j = 0; j < curvedSegment.NodeAmount; j++)
                    {
                        Node endNode = curvedSegment.GetNode(j);
                        
                        GenerateQuad(startingNode, endNode);

                        startingNode = endNode;
                    }
                    
                }
                else
                {
                    GenerateQuad(
                        segment.GetControlPoint(0),
                        segment.GetControlPoint(1)
                        );
                }
                
            }
            
            _mesh.Clear();
            _mesh.vertices = vertices.ToArray();
            _mesh.triangles = triangles.ToArray();
            
            

            void GenerateQuad(Node start, Node end)
            {
                if (vertices.Count == 0)
                {
                    
                    vertices.Add(start.transform.TransformPoint(((Vector3.right * laneWidth) * laneCount) +(Vector3.up *0.01f)));
                    vertices.Add(start.transform.TransformPoint(((Vector3.left * laneWidth) * laneCount) +(Vector3.up *0.01f)));
                    
                }
                
                vertices.Add(end.transform.TransformPoint(((Vector3.right * laneWidth) * laneCount) +(Vector3.up *0.1f)));
                vertices.Add(end.transform.TransformPoint(((Vector3.left * laneWidth) * laneCount) +(Vector3.up *0.01f)));
                
                
                triangles.AddRange(new []
                {
                    vertices.Count-4,
                    vertices.Count-3,
                    vertices.Count-2,
                    vertices.Count-1,
                    vertices.Count-2,
                    vertices.Count-3
                });
            }
        }


        private void OnDrawGizmos()
        {
            for (int i = 0; i < _waypoints.Count; i++)
            {
                Waypoint waypoint = _waypoints[i];

                Gizmos.color = Color.red;
                Gizmos.DrawSphere(waypoint.Position, 0.5f);

                
                if (waypoint.PreviousWaypoint is not null)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(waypoint.Position, waypoint.PreviousWaypoint.Position);
                }
            }
        }
    }
}