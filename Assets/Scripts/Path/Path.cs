using System;
using System.Collections.Generic;
using UnityEngine;

public class Path
{
    public Action PathChanged;
    
    private List<Segment> _segments = new();
    
    public readonly Transform Nodes;
    public readonly Transform ControlPoints;

    public Path(Transform nodes, Transform controlPoints)
    {
        Nodes = nodes;
        ControlPoints = controlPoints;
    }
    
    public int SegmentAmount
        => _segments.Count;
    
    
    public Segment GetSegment(int index)
        => _segments[index];

    public void AddSegment(Segment segment)
        => _segments.Add(segment);

    public void RemoveSegment(int index)
        => _segments.RemoveAt(index);
}