using UnityEngine;

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

        // Calculate the difference between starting points

        Vector3 diff = point2 - point1;

    

        // Calculate the denominator (cross product of direction vectors)

        float denominator = Vector3.Dot(direction1, Vector3.Cross(direction1, direction2));



        // Check for parallel lines (denominator close to zero)

        if (Mathf.Abs(denominator) < Mathf.Epsilon)

            return Vector3.zero; // Or handle parallel case appropriately



        // Solve for "t"

        float t = Vector3.Dot(diff, Vector3.Cross(direction2, direction1)) / denominator;



        // Calculate the intersection point

        return point1 + t * direction1;

    }

    
}