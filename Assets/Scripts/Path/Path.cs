using System;
using UnityEngine;

[ExecuteInEditMode]
public class Path : MonoBehaviour
{
    public Action PathChanged;
    
    private Segment[] _segments = Array.Empty<Segment>();

    public Transform Nodes { get; private set; }
    public Transform ControlPoints { get; private set; }

    private void Awake()
    {
        Nodes = new GameObject("Nodes").transform;
        Nodes.SetParent(transform);
        
        ControlPoints = new GameObject("Control points").transform;
        ControlPoints.SetParent(transform);
    }


    public int SegmentAmount
        => _segments.Length;
    
    public Segment GetSegment(int index)
        => _segments[index];
    
    public void AddSegment(Segment segment)
    {
        Array.Resize(ref _segments, _segments.Length+1);
        _segments[_segments.Length-1] = segment;
    }
    
    public void RemoveSegment(int index)
    {
        Segment segment = GetSegment(index);
        for (int i = 0; i < segment.ControlPointAmount; i++)
        {
            DestroyImmediate(segment.GetControlPoint(i));
        }
        

        if (segment is CurvedSegment curvedSegment)
        {
            for (int i = 0; i < curvedSegment.NodeAmount; i++)
            {
                DestroyImmediate(curvedSegment.GetNode(i));
            }
        }
        
        Array.Copy(_segments, index + 1, _segments, index, _segments.Length - index - 1);
        Array.Resize(ref _segments, _segments.Length - 1);
        
    }
    
}