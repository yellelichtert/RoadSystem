using System;
using UnityEngine;

public class SimpleCurvedSegment : CruvedSegment
{
    private const int MaxControlPoints = 3;

    public override int GetMaxControlPoints()
        => MaxControlPoints;

    protected override void GenerateSegment()
    {
        if (ControlPointAmount < 3) return;
        
        Nodes = null;
        
        for (float t = 0; t < 1; t += 0.05f)
        {
            Vector3 p1 = Vector3.Lerp(GetControlPoint(0), GetControlPoint(1), t);
            Vector3 p2 = Vector3.Lerp(GetControlPoint(1), GetControlPoint(2), t);

            if (Nodes == null)
            {
                Nodes = new Vector3[1]{Vector3.Lerp(p1, p2, t)};
            }
            else
            {
                Array.Resize(ref Nodes, Nodes.Length + 1);
                Nodes[Nodes.Length - 1] = Vector3.Lerp(p1, p2, t);
            }
            
        }
    }
}