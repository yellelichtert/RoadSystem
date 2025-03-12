using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RoadSystem;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class RoadNetworkTool : EditorWindow
    {
        [CanBeNull] private RoadNetwork _network;
        [CanBeNull] private Road _selectedRoad;
        [CanBeNull] private Intersection _selectedIntersection;
        
        
        private string[] _tabs = { "Roads", "Intersections" };
        private int _selectedTab = 0;
        private bool _disableIntersectionTab;


        
        
        [MenuItem("Tools/RoadEditor")]
        public static void ShowWindow()
           => GetWindow<RoadNetworkTool>();


        private void OnSelectionChange()
        {
            
            if (_selectedRoad is not null && !_selectedRoad.Path.GetSegment(0).IsCompleted)
            {
                _network.DeleteRoad(_selectedRoad);
            }
            else if (_selectedIntersection is not null && _selectedIntersection.NodeCount < 2)
            {
                _network.DeleteIntersection(_selectedIntersection);
            }
            

            
            var selectedObject = Selection.activeObject.GameObject();
            
            
            if (selectedObject is null)
            {
                ResetValues();
            }
            else if (selectedObject.TryGetComponent(out _selectedRoad))
            {
                _selectedIntersection = null;
            }
            else if (selectedObject.TryGetComponent(out _selectedIntersection))
            {
                _selectedRoad = null;
            }
            else
            {
                ResetValues();
            }
            

            void ResetValues()
            {
                _selectedRoad = null;
                _selectedIntersection = null;
            }
        }


        private void OnGUI()
        {
            
            EditorGUILayout.BeginVertical();

            
            _network = (RoadNetwork)EditorGUILayout.ObjectField("Network", _network, typeof(RoadNetwork), true);
            
            if (!_network)
            {
                EditorGUILayout.HelpBox("Please select a road Network", MessageType.Error);
                if (GUILayout.Button("Create network"))
                {
                    _network = RoadNetwork.Create();
                }
            }
            else
            {
                
                _selectedTab = GUILayout.Toolbar(_selectedTab, _tabs);

                CreateSpaces(2);
                
                EditorGUILayout.BeginHorizontal();

                CreateSpaces(2);

                
                if (GUILayout.Button("Create"))
                {
                    
                    switch (_selectedTab)
                    {
                        case 0:
                            Selection.activeObject = _network.CreateRoad();
                            break;
                        case 1:
                            if (_disableIntersectionTab) break;
                            Selection.activeObject = _network.CreateIntersection();
                            break;
                    }
                }
                
                //todo: Implement Edit
                // if (GUILayout.Button("E"))
                // {
                //     Debug.Log("Edit");
                // }
                //

                
                CreateSpaces(2);

                
                if (GUILayout.Button("Delete"))//Delete
                {
                    if(Selection.activeObject is null) return;
                    
                    switch (_selectedTab)
                    {
                        case 0:
                            if (_selectedRoad is not null)
                                _network.DeleteRoad(_selectedRoad);
                            break;
                        case 1:
                            if (_selectedIntersection is not null)
                                _network.DeleteIntersection(_selectedIntersection);
                            break;
                    }
                    
                  
                }
                
                CreateSpaces(2);

                
                EditorGUILayout.EndHorizontal();

                CreateSpaces(2);
                
                
                switch (_selectedTab)
                {
                    case 0:
                        ShowRoadsOverView();
                        break;
                    case 1:
                        ShowIntersectionsOverView();
                        break;
                        
                }

            }
            
            
            EditorGUILayout.EndVertical();
        }


        private void ShowRoadsOverView()
        {
            EditorGUILayout.BeginHorizontal();

            var roads = _network.GetAllRoads();

            _disableIntersectionTab = roads.Length < 2;
            
            if (!roads.Any())
            {
                EditorGUILayout.HelpBox("Create your first Road", MessageType.Info);
            }
            else
            {
                ShowList(_network.GetAllRoads());
            
            
                //Data overview
                EditorGUILayout.BeginVertical();
                if (_selectedRoad is null)
                {
                    EditorGUILayout.HelpBox("Please select a road", MessageType.Info);
                }
                else if (!_selectedRoad.IsDestroyed())
                {
                    CreateSpaces(2);
                    
                    EditorGUILayout.BeginHorizontal();
                    
                    EditorGUILayout.LabelField("Road name:");   
                    _selectedRoad.name = EditorGUILayout.TextField(_selectedRoad.name);
                    
                    EditorGUILayout.EndHorizontal();
                    
                    CreateSpaces(1);
                    
                    EditorGUILayout.BeginHorizontal();
                    
                    EditorGUILayout.LabelField("Road Width:");
                    _selectedRoad.LaneWidth = EditorGUILayout.FloatField(_selectedRoad.LaneWidth);
                    
                    EditorGUILayout.EndHorizontal();
                    
                }
            
            
                EditorGUILayout.EndVertical();

            }
            
            
            EditorGUILayout.EndHorizontal();
        }
        
        
        private void ShowIntersectionsOverView()
        {
            EditorGUILayout.BeginHorizontal();

            var intersections = _network.GetAllInterSections();

            if (_disableIntersectionTab)
            {
                EditorGUILayout.HelpBox("Create 2 roads before you create an intersection!", MessageType.Error);
            }
            else if (!intersections.Any())
            {
                EditorGUILayout.HelpBox("Create your first intersection.", MessageType.Info);
            }
            else
            {
                ShowList(_network.GetAllInterSections());
            
            
                EditorGUILayout.BeginVertical();
                if (_selectedIntersection is null)
                {
                    EditorGUILayout.HelpBox("Please select an intersection.", MessageType.Info);
                }
                else
                {
                    EditorGUILayout.LabelField("Selected intersection: " +  _selectedIntersection.name);
                }
            
                EditorGUILayout.EndVertical();

            }
            
            EditorGUILayout.EndHorizontal();
        }
        
        private void ShowList<T>(T[] obj) where T : MonoBehaviour
        {
            EditorGUILayout.BeginVertical();

            for (int i = 0; i < obj.Length; i++)
            {
                if (!obj[i].IsDestroyed() && GUILayout.Button(obj[i].name))
                {
                    Selection.activeObject = obj[i].GameObject();
                }
            }

            EditorGUILayout.EndVertical();
        }


        private void CreateSpaces(int count)
        {
            for (int i = 0; i < count; i++) 
                EditorGUILayout.Space();
        }
    }
}