using System;
using System.Collections.Generic;
using Model;
using UnityEngine;
using Object = UnityEngine.Object;

public class Segment : ScriptableObject
{
    protected const int MaxControlPoints = 3;
    protected List<Node> ControlPoints = new();
    protected Transform NodeParent;
    protected Node[] Nodes = Array.Empty<Node>();


    public static Segment Create(Transform nodeParent)
    {
        var segment = CreateInstance<Segment>();

        segment.NodeParent = nodeParent;

        return segment;
    }
    
    public bool IsCompleted
        => GetMaxControlPoints() == ControlPointAmount;

    public int ControlPointAmount
        => ControlPoints.Count;

    public int NodeAmount
        => Nodes.Length;
    
    public Node GetControlPoint(int index)
        => ControlPoints[index];

    public Node GetNode(int index)
        => Nodes[index];

    
    protected void AddNode(Node node)
    {
        Array.Resize(ref Nodes, Nodes.Length + 1);
        Nodes[NodeAmount - 1] = node;

        if (NodeAmount > 1)
        {
            GetNode(NodeAmount-2).transform.LookAt(node.GetPosition());
        }
    }
    
    
    public void SetControlPoint(int index, Vector3 position)
    {
        ControlPoints[index].SetPosition(position);

        if (ControlPointAmount > 1)
        {
            ControlPoints[index - 1].
                transform.LookAt(position);
        }

        if (ControlPointAmount >= 2)
        {

            GetControlPoint(ControlPointAmount - 1).transform.rotation = GetControlPoint(0).transform.rotation;

        }
        
        GenerateSegment();
    }
        

    public int GetMaxControlPoints()
        => MaxControlPoints;

    public void AddControlPoint(Node node)
    {
        ControlPoints.Add(node);
    }
    
    
    
    
    protected void GenerateSegment()
    {   
        if (ControlPointAmount < 3) return;

        foreach (var node in Nodes)
        {
            DestroyImmediate(node.gameObject);
        }
        
        Nodes = Array.Empty<Node>();
        
        
        for (float t = 0; t < 1; t += 0.05f)
        {
            Vector3 position = Utils.CalculateCurvePoint(t,
                GetControlPoint(0).GetPosition(),
                GetControlPoint(1).GetPosition(),
                GetControlPoint(2).GetPosition()
            );
            
            
            float terrainHeight = Terrain.activeTerrain.SampleHeight(position);
            AddNode(Node.Create(new Vector3(position.x, terrainHeight+0.05f, position.z), NodeParent));
            
            GetNode(NodeAmount-1).transform.LookAt(GetControlPoint(ControlPointAmount-1).GetPosition());
        }
        
        AddNode(Node.Create(GetControlPoint(MaxControlPoints-1).GetPosition(), NodeParent));
        
        GetNode(NodeAmount - 1).transform.rotation = GetNode(NodeAmount - 2).transform.rotation;
        GetControlPoint(ControlPointAmount - 1).transform.rotation = GetNode(NodeAmount - 1).transform.rotation;
    }
}