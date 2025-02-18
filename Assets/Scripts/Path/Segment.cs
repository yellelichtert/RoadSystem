using System;
using Model;
using UnityEngine;

public class Segment
{
    private const int MaxControlPoints = 2;

    protected Node[] ControlPoints = Array.Empty<Node>();

    public bool IsCompleted
        => GetMaxControlPoints() == ControlPointAmount;

    public int ControlPointAmount
        => ControlPoints.Length;

    public Node GetControlPoint(int index)
        => ControlPoints[index];

    public virtual void SetControlPoint(int index, Vector3 position)
    {
        ControlPoints[index].SetPosition(position);

        if (ControlPointAmount > 1)
        {
            GetControlPoint(index-1)
                .transform.LookAt(position);
        }
    }
        

    public virtual int GetMaxControlPoints()
        => MaxControlPoints;

    public void AddControlPoint(Node node)
    {
        Array.Resize(ref ControlPoints, ControlPoints.Length + 1);
        ControlPoints[ControlPoints.Length - 1] = node;
    }
}