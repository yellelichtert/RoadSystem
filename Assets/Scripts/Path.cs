using System;
using UnityEditor;
using UnityEngine;

public class Path : MonoBehaviour
{
    private Segment[] _segments = Array.Empty<Segment>();
    
    
    public int SegmentAmount
        => _segments.Length;
    
    public Segment GetSegment(int index)
        => _segments[index];

    public void AddSegment(Segment segment)
    {
        Array.Resize(ref _segments, _segments.Length+1);
        _segments[_segments.Length - 1] = segment;
    }
    
    
    void OnDrawGizmos()
    {
        for (int i = 0; i < _segments.Length; i++)
        {
            Segment segment = _segments[i];

            if (segment is not ICurvedSegment curvedSegment && segment.ControlPointAmount == 2)
            {
                Gizmos.DrawLine(segment.GetControlPoint(0), segment.GetControlPoint(1));   
            }
        }
    }
    
}
