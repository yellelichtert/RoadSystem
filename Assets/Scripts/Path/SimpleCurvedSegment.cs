using System;
using Model;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;

public class SimpleCurvedSegment : CurvedSegment
{
    protected new int MaxControlPoints;

    public SimpleCurvedSegment(Transform nodeParent) : base(nodeParent) 
        => MaxControlPoints = 3;


    public override int GetMaxControlPoints()
        => MaxControlPoints;
    
    protected void AddNode(Node node)
    {
        Array.Resize(ref Nodes, Nodes.Length + 1);
        Nodes[NodeAmount - 1] = node;

        if (NodeAmount > 1)
        {
            GetNode(NodeAmount-2).transform.LookAt(node.GetPosition());
        }
    }

    protected override void GenerateSegment()
    {   
        if (ControlPointAmount < 3) return;

        foreach (var node in Nodes)
        {
            Object.DestroyImmediate(node.gameObject);
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
        
    }

}