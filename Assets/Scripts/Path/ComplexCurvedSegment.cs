﻿using System;
using UnityEngine;

public class  ComplexCurvedSegment : SimpleCurvedSegment
{
    public ComplexCurvedSegment()
        => MaxControlPoints = 4;
    
    

    protected override void GenerateSegment()
    {
        if (ControlPointAmount < 3) return;

        
        if (ControlPointAmount < MaxControlPoints)
        {
            
            base.GenerateSegment();
            
        }
        else if (ControlPointAmount == MaxControlPoints)
        {
           
            Nodes = Array.Empty<Vector3>();
            
            for (float t = 0; t <  1; t += 0.05f)
            {
                
                Vector3 p1 = CalculateQuadraticBezierPoint(t,
                    GetControlPoint(0),
                    GetControlPoint(1),
                    GetControlPoint(2));
                
                Vector3 p2 = CalculateQuadraticBezierPoint(t,
                    GetControlPoint(0),
                    GetControlPoint(2),
                    GetControlPoint(3));

                Vector3 node = Vector3.Lerp(p1, p2, t);
                
                AddNode(node);
                
            }
            
        }
    }
}