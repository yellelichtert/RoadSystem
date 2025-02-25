using System;
using Model;
using UnityEngine;

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
        
        Nodes = Array.Empty<Node>();
        
        
        for (float t = 0; t < 1; t += 0.05f)
        {
            Vector3 position = CalculateQuadraticBezierPoint(t,
                GetControlPoint(0).GetPosition(),
                GetControlPoint(1).GetPosition(),
                GetControlPoint(2).GetPosition()
            );
            
            Debug.Log("Creating node: #" + t);
            
            AddNode(Node.Create(position, NodeParent));
            
            GetNode(NodeAmount-1).transform.LookAt(GetControlPoint(ControlPointAmount-1).GetPosition());
        }
    }
    
    
    protected Vector3 CalculateQuadraticBezierPoint(float t, Vector3 cp1, Vector3 cp2, Vector3 cp3)
    {
        Vector3 p1 = Vector3.Lerp(cp1, cp2, t);
        Vector3 p2 = Vector3.Lerp(cp2, cp3, t);
        
        return Vector3.Lerp(p1, p2, t);
    }
}