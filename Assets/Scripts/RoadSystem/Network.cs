using System.Collections.Generic;
using UnityEngine;

namespace RoadSystem
{
    [ExecuteInEditMode]
    public class Network : MonoBehaviour
    {
        private List<Road> _roads = new();
        private Transform _roadParent;

        
        
        private void Awake()
        {
            _roadParent = new GameObject("Roads").transform;
            _roadParent.parent = transform;
        }


        public List<Road> GetAllRoads()
            => _roads;

        public Road GetRoad(int index)
            => _roads[index];

        public void CreateRoad()
        {
            //Veranderen naar road.CREATE
            Road road = new GameObject($"Road #{_roadParent.childCount}").AddComponent<Road>();
            road.transform.parent = _roadParent;
            
            _roads.Add(road);
        }
        
    }
}