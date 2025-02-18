using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Model;
using NUnit.Framework;
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
            List<Node> nodes = new();

            for (int s = 0; s < _path.SegmentAmount; s++)
            {
                Segment selectedSegment = _path.GetSegment(s);

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
    }
}