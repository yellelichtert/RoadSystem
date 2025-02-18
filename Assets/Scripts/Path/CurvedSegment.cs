using System;
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

    protected void DestroyNodes()
    {
        if (_nodeParent.childCount == 0) return;
        
        Debug.Log("Destroying nodes....");
        
        for (int i = 0; i < _nodeParent.childCount; i++)
        {
            UnityEngine.Object.DestroyImmediate(_nodeParent.GetChild(i).gameObject);
        }
        
        Debug.Log("Nodes destroyed... new count: " + _nodeParent.childCount);
    }
    
}