using System.Collections.Generic;
using Model;
using UnityEngine;

public class Segment
{
    protected const int MaxControlPoints = 2;
    
    protected List<Node> ControlPoints = new();

    public bool IsCompleted
        => GetMaxControlPoints() == ControlPointAmount;

    public int ControlPointAmount
        => ControlPoints.Count;

    public Node GetControlPoint(int index)
        => ControlPoints[index];

    
    public virtual void SetControlPoint(int index, Vector3 position)
    {
        ControlPoints[index].SetPosition(position);

        if (ControlPointAmount > 1)
        {
            ControlPoints[index - 1].
                transform.LookAt(position);
        }
    }
        

    public virtual int GetMaxControlPoints()
        => MaxControlPoints;

    public void AddControlPoint(Node node)
    {
        ControlPoints.Add(node);
    }

    public void RemoveLastControlPoint()
        => ControlPoints.RemoveAt(ControlPoints.Count - 1);
}