using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace RoadSystem
{
    [ExecuteInEditMode]
    public class RoadNetwork : MonoBehaviour
    {
        private List<Road> _roads;
        private List<Intersection> _intersections;
        
        private Transform _roadParent;
        private Transform _intersectionParent;


        public static RoadNetwork Create()
        {
            var network = new GameObject("RoadNetwork")
                .AddComponent<RoadNetwork>();

            return network;
        }
        
        
        private void Awake()
        {
            _roadParent = new GameObject("Roads").transform;
            _roadParent.parent = transform;
            
            _intersectionParent = new GameObject("Intersections").transform;
            _intersectionParent.parent = transform;
        }

        private void OnEnable()
        {
            _roads = new();
            for (int i = 0; i < _roadParent.childCount; i++)
            {
                Road road = _roadParent.GetChild(i).GetComponent<Road>();
                _roads.Add(road);
            }

            _intersections = new();
            for (int i = 0; i < _intersectionParent.childCount; i++)
            {
                Intersection intersection = _intersectionParent.GetChild(i).GetComponent<Intersection>();
                _intersections.Add(intersection);
            }
        }


        //Road Crud
        public Road[] GetAllRoads()
            => _roads.ToArray();

        
        public Road CreateRoad()
        {
            var road = new GameObject($"Road #{_roadParent.childCount+1}" )
                .AddComponent<Road>();
            
            _roads.Add(road);
            road.transform.parent = _roadParent;
            
            return road;
        }


        public void DeleteRoad(Road road)
        {
            _roads.Remove(road);
            DestroyImmediate(road.gameObject);
        }
        
        
        
        
        //Intersection Crud
        public Intersection[] GetAllInterSections()
            => _intersections.ToArray();
        
        
        public Intersection CreateIntersection()
        {
            var intersection = new GameObject($"Intersection #{_intersectionParent.childCount+1}")
                .AddComponent<Intersection>();
            
            _intersections.Add(intersection);
            intersection.transform.parent = _intersectionParent;
            return intersection;
        }

        public void DeleteIntersection(Intersection intersection)
        {
            _intersections.Remove(intersection);
            DestroyImmediate(intersection.gameObject);
        }
    }
}