using System;
using Model;
using UnityEngine;

public abstract class CurvedSegment : Segment
{
    protected Transform NodeParent;

    protected CurvedSegment(Transform nodeParent)
        => NodeParent = nodeParent;
    
    protected Node[] Nodes = Array.Empty<Node>();
            
    public int NodeAmount
        => Nodes.Length;
    
            
    public override void SetControlPoint(int index, Vector3 position)
    {
        base.SetControlPoint(index, position);
        GenerateSegment();
        
    }
            
    public Node GetNode(int index)
        => Nodes[index];

    protected abstract void GenerateSegment();
    
}