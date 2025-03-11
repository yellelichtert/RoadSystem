using System;
using System.Collections.Generic;
using System.Linq;
using Model;
using UnityEngine;

namespace Utils
{
    public class Pathfinding
    {
        public static Waypoint[] CalculateRoute(Waypoint startPoint, Waypoint endPoint)
        {
            var openSet = new List<WpValues>();
            var closedSet = new List<Waypoint>();
            
            
            var start = new WpValues()
            {
                Point = startPoint,
                GScore = 0,
                HScore = Vector3.Distance(startPoint.GetPosition(), endPoint.GetPosition())
            };
            
            openSet.Add(start);
            
            
            while (openSet.Any())
            {
                int lowestF = 0;
                for (int i = 0; i < openSet.Count && openSet.Count > 1; i++)
                {
                    if (openSet[i].FScore < openSet[lowestF].FScore)
                        lowestF = i;
                }
            
                
                WpValues currentPoint = openSet[lowestF];
                
                
                if (currentPoint.Point == endPoint)
                {
                    var path = new List<Waypoint>();

                    while (currentPoint.Point != startPoint)
                    {
                        path.Add(currentPoint.Point);
                        currentPoint = currentPoint.CameFrom;
                    }

                    if (path.Contains(startPoint))
                    {
                        Debug.Log("NO NEED TO ADD");
                    }
                    else
                    {
                        path.Add(startPoint);
                    }

                    path.Reverse();
                    return path.ToArray();

                }
            
            
                openSet.Remove(currentPoint);
                closedSet.Add(currentPoint.Point);
            
                
                if (currentPoint.Point.NextWaypoint is not null)
                {
                    CheckWaypoint(currentPoint, currentPoint.Point.NextWaypoint);
                }
            
                if (currentPoint.Point.LinkedWaypoints is not null)
                {
                    CheckLinks(currentPoint);
                }
            }
            
            
            
            Debug.Log("Route not found");
            return null;
            
            
            
            bool CheckWaypoint(WpValues currentPoint ,Waypoint nextPoint, bool addAtEnd = true)
            {
                var result = new WpValues() { Point = nextPoint, CameFrom = currentPoint};
                
                if (closedSet.Contains(result.Point)) return false;
            
                float tentativeScore = 
                    currentPoint.GScore + Vector3.Distance(currentPoint.Point.GetPosition(), nextPoint.GetPosition());
            
                if (currentPoint.GScore < tentativeScore)
                {
                    result.GScore = tentativeScore;
                    result.HScore = Vector3.Distance(nextPoint.GetPosition(), endPoint.GetPosition());
            
                    if (addAtEnd)
                        openSet.Add(result);
                    
                    return true;
                }
            
                return false;
            }
            
            
            
            
            void CheckLinks(WpValues wpValues)
            {
                var linkedPoints = wpValues.Point.LinkedWaypoints;

                for (int i = 0; i < linkedPoints.Count; i++)
                {
                    if (CheckWaypoint(wpValues, linkedPoints[i].dest, false))
                    {
                        var nextPoint = new WpValues()
                        {
                            Point = linkedPoints[i].link,
                            CameFrom = wpValues
                        };

                        while (nextPoint.Point != linkedPoints[i].dest)
                        {
                            nextPoint = new WpValues()
                            {
                                Point = nextPoint.Point.NextWaypoint,
                                CameFrom = nextPoint,
                            };
                        }
                        
                        nextPoint.GScore = wpValues.GScore + Vector3.Distance(wpValues.Point.GetPosition(), nextPoint.Point.GetPosition());
                        nextPoint.HScore = Vector3.Distance(nextPoint.Point.GetPosition(), endPoint.GetPosition());
                        
                        openSet.Add(nextPoint);
                    }
                }
            }
            
        }
        

        private class WpValues
        {
            public Waypoint Point;
            public WpValues CameFrom;
            
            public float GScore;
            public float HScore;
            public float FScore => GScore + HScore;
        }
    }
}