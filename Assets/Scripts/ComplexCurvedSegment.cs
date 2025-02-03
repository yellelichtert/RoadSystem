﻿using System;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ComplexCurvedSegment : SimpleCurvedSegment
{
    public ComplexCurvedSegment()
        => MaxControlPoints = 5;

    protected override void GenerateSegment()
    {
        if (ControlPointAmount < 3) return;

        Nodes = Array.Empty<Vector3>();
        
        
        if (ControlPointAmount == 5)
        {
            
            for (float t = 0; t < 1; t += 0.05f)
            {
                
                Vector3 node = CalculateQuadraticBezierPoint(t,
                    GetControlPoint(0), 
                    GetControlPoint(1), 
                    GetControlPoint(2));
                
                AddNode(node);
                
            }

            
            for (float t = 0; t < 1; t+= 0.05f)
            {
                
                Vector3 node = CalculateQuadraticBezierPoint(t,
                    GetControlPoint(2), 
                    GetControlPoint(3), 
                    GetControlPoint(4));
                
                AddNode(node);
                
            }
            
        }
        else if (ControlPointAmount > 2)
        {
            
            for (int i = 0; i < ControlPointAmount; i++)
            {
                
                CalculateQuadraticBezierPoint(0.05f,
                    GetControlPoint(0), 
                    GetControlPoint(1), 
                    GetControlPoint(2));
                
            }
            
        }
        
    }
}