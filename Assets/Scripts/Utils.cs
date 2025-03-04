﻿using Vector3 = UnityEngine.Vector3;

public static class Utils
{
    public static Vector3 CalculateQuadraticBezierPoint(float t, Vector3 cp1, Vector3 cp2, Vector3 cp3)
    {
        Vector3 p1 = Vector3.Lerp(cp1, cp2, t);
        Vector3 p2 = Vector3.Lerp(cp2, cp3, t);
        
        return Vector3.Lerp(p1, p2, t);
    }
    
    
    
    public static Vector3 FindIntersectionPoint(Vector3 point1, Vector3 direction1, Vector3 point2, Vector3 direction2) 
    {
        
        Vector3 crossDir = Vector3.Cross(direction1, direction2);
        
        Vector3 diff = point2 - point1;
        float t = Vector3.Dot(Vector3.Cross(diff, direction1), crossDir) / crossDir.sqrMagnitude;

        return point1 + t * direction1;
        
    }

    
}