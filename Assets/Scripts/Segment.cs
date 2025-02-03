using System;
using UnityEngine;

public class Segment
{
       private const int MaxControlPoints = 2;
       
       private Vector3[] _controlPoints = Array.Empty<Vector3>();
       
       public bool IsCompleted
            => GetMaxControlPoints() == ControlPointAmount;
       
       public int ControlPointAmount
           => _controlPoints.Length;
       
       public Vector3 GetControlPoint(int index)
           => _controlPoints[index];
       
       public virtual void SetControlPoint(int index, Vector3 position)
           => _controlPoints[index] = position;

       public virtual int GetMaxControlPoints()
            => MaxControlPoints;
       
       public void AddControlPoint(Vector3 position)
       {
               Array.Resize(ref _controlPoints, _controlPoints.Length + 1);
                _controlPoints[_controlPoints.Length - 1] = position;
       }
       
       public void RemoveControlPoint(int index)
       {
               Array.Copy(_controlPoints, index + 1, _controlPoints, index, _controlPoints.Length - index - 1);
               Array.Resize(ref _controlPoints, _controlPoints.Length - 1);
       }
}