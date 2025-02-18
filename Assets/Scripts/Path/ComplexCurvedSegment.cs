using System;
using Model;
using UnityEngine;

public class  ComplexCurvedSegment : SimpleCurvedSegment
{
    public ComplexCurvedSegment(Transform nodeParent) : base(nodeParent) 
        => MaxControlPoints = 4;
    
    

    protected override void GenerateSegment()
    {
        if (ControlPointAmount < 3) return;

        
        Debug.Log("Generating nodes....");
        
        if (ControlPointAmount < MaxControlPoints)
        {
            
            base.GenerateSegment();
            
        }
        else if (ControlPointAmount == MaxControlPoints)
        {
           
            Nodes = Array.Empty<Node>(); 
            
            for (float t = 0; t <  1; t += 0.05f)
            {
                
                Vector3 p1 = CalculateQuadraticBezierPoint(t,
                    GetControlPoint(0).GetPosition(),
                    GetControlPoint(1).GetPosition(),
                    GetControlPoint(2).GetPosition()
                    );
                
                Vector3 p2 = CalculateQuadraticBezierPoint(t,
                    GetControlPoint(0).GetPosition(),
                    GetControlPoint(2).GetPosition(),
                    GetControlPoint(3).GetPosition()
                    );
                
                Vector3 position = Vector3.Lerp(p1, p2, t);
                
                AddNode(Node.Create(position, _nodeParent));
            }
            
            
            GetNode(NodeAmount-1).transform.LookAt(GetControlPoint(ControlPointAmount-1).GetPosition());
        }
        
        Debug.Log("NodeCount: " + NodeAmount);
        
    }
}