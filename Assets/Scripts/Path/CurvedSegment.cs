using System;
using System.Threading.Tasks;
using Model;
using UnityEngine;

public abstract class CurvedSegment : Segment
{
    protected Transform _nodeParent;

    protected CurvedSegment(Transform nodeParent)
        => _nodeParent = nodeParent;
    
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