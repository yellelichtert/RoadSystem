using System;
using UnityEngine;

public class SimpleCurvedSegment : CruvedSegment
{
    protected int MaxControlPoints;

    public SimpleCurvedSegment()
        => MaxControlPoints = 3;


    public override int GetMaxControlPoints()
        => MaxControlPoints;
    
    protected void AddNode(Vector3 node)
    {
        Array.Resize(ref Nodes, Nodes.Length + 1);
        Nodes[Nodes.Length - 1] = node;
    }

    protected override void GenerateSegment()
    {
        if (ControlPointAmount < 3) return;
        
        Nodes = Array.Empty<Vector3>();
        
        for (float t = 0; t < 1; t += 0.05f)
        {
            Vector3 node = CalculateQuadraticBezierPoint(t,
                GetControlPoint(0), 
                GetControlPoint(1), 
                GetControlPoint(2));
            
            AddNode(node);
        }
    }
    
    
    protected Vector3 CalculateQuadraticBezierPoint(float t, Vector3 cp1, Vector3 cp2, Vector3 cp3)
    {
        Vector3 p1 = Vector3.Lerp(cp1, cp2, t);
        Vector3 p2 = Vector3.Lerp(cp2, cp3, t);
        
        return Vector3.Lerp(p1, p2, t);
    }
}