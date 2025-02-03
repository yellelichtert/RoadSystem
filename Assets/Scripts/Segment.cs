using System;
using UnityEngine;

public class Segment
{
       public readonly int MaxControlPointAmount = 2;
       
       private Vector3[] _controlPoints = Array.Empty<Vector3>();
       
       public int ControlPointAmount
           => _controlPoints.Length;
       
       public Vector3 GetControlPoint(int index)
           => _controlPoints[index];
       
       public Vector3 SetControlPoint(int index, Vector3 position)
           => _controlPoints[index] = position;

       public virtual void AddControlPoint(Vector3 position)
       {
               Array.Resize(ref _controlPoints, _controlPoints.Length + 1);
                _controlPoints[_controlPoints.Length - 1] = position;
       }
}