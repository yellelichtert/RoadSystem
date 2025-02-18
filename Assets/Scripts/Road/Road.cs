using System;
using JetBrains.Annotations;
using Model;
using Road;
using UnityEditor;
using UnityEngine;

namespace RoadComponent
{
    [ExecuteInEditMode]
    public class Road : MonoBehaviour
    {
        [SerializeField] private int laneCount = 1;
        [SerializeField] private float laneWidth = 8;
        [SerializeField] private bool oneWay;
        
        private Path _path;
        private Transform _waypoints;
        
        private void Awake()
        {
            _path = new GameObject("Path")
                .AddComponent<Path>();
            
            _path.transform.SetParent(transform);

            _waypoints = new GameObject("-Waypoints-")
                .transform;
            
            _waypoints.SetParent(transform);
            
            _path.PathChanged += PathChanged;
            
        }

        
        
        private void PathChanged()
        {
            //Clear existing waypoints
            for (int i = 0; i < _waypoints.childCount; i++)
            {
                DestroyImmediate(_waypoints.GetChild(i).gameObject);
            }
            
            //Handle change
            for (int s = 0; s < _path.SegmentAmount; s++)
            {
                Segment selectedSegment = _path.GetSegment(s);
                Node[] points;
                
                if(!selectedSegment.IsCompleted)
                    return;
                
                
                if (selectedSegment is CurvedSegment curvedSegment)
                {

                    points = new Node[curvedSegment.NodeAmount];
                    for (int i = 0; i < curvedSegment.NodeAmount; i++)
                    {
                        points[i] = curvedSegment.GetNode(i);
                    }
                    
                }
                else
                {
                    points = new[]
                    {
                        selectedSegment.GetControlPoint(0),
                        selectedSegment.GetControlPoint(1)
                    };
                }
                
                
                GenerateWaypoints(points);

                if (!oneWay)
                {
                    Array.Reverse(points);
                    GenerateWaypoints(points, true);
                }
            }
        }


        private void GenerateWaypoints(Node[] points, bool left = false)
        {
            
            for (int i = 0; i < laneCount; i++)
            {
                
                Waypoint? previousPoint = null;
                
                
                for (int p = 0; p < points.Length; p++)
                {
                    Vector3 newPosition = points[p].transform.TransformPoint(
                        (left ? Vector3.right : Vector3.left) * ((laneWidth / 2) + laneWidth * i));
                    
                    

                    Waypoint wp = new GameObject($"Waypoint #{_waypoints.childCount}")
                        .AddComponent<Waypoint>();

                    wp.transform.position = newPosition;
                    wp.transform.parent = _waypoints;

                    if (previousPoint)
                    {
                        wp.SetPreviousPoint(previousPoint);
                        previousPoint.SetNextPoint(wp);
                    }

                    previousPoint = wp;
                    
                }
            }
        }


        private void OnDrawGizmos()
        {
            for (int i = 0; i < _path.SegmentAmount; i++)
            {
                var selected = _path.GetSegment(i);

                if (!selected.IsCompleted) return;
         
                
                Gizmos.color = Color.magenta;
                if (selected is CurvedSegment segment)
                {
                    for (int j = 0; j < segment.NodeAmount; j++)
                    {
                        Gizmos.DrawSphere(segment.GetNode(j).GetPosition(), 0.5f);
                    }
                }
                else
                {
                    Gizmos.DrawSphere(selected.GetControlPoint(0).GetPosition(), 0.5f);
                    Gizmos.DrawSphere(selected.GetControlPoint(1).GetPosition(), 0.5f);

                }

            }
        }
    }
}