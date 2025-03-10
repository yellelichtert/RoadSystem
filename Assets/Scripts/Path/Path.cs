using System;
using System.Collections.Generic;
using UnityEngine;

public class Path : ScriptableObject
{
    public Action PathChanged;
    
    private List<Segment> _segments = new();

    public Transform Nodes { get; private set; }
    public Transform ControlPoints { get; private set; }

    
    public static Path Create(Transform nodes, Transform controlPoints)
    {
        var path = CreateInstance<Path>();
        
        path.Nodes = nodes;
        path.ControlPoints = controlPoints;
        
        return path;
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