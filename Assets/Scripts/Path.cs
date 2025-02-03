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
        _segments[_segments.Length-1] = segment;
    }
    
    public void RemoveSegment(int index)
    {
        Array.Copy(_segments, index + 1, _segments, index, _segments.Length - index - 1);
        Array.Resize(ref _segments, _segments.Length - 1);
    }
    
}
