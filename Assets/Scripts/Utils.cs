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
        Debug.Log("Finding point");

        Vector3 temp1;
        Vector3 temp2;
        
        for (float t = 0; t < 1; t+=0.05f)
        {
            temp1 = Vector3.Lerp(point1, direction1, t);
            temp2 = Vector3.Lerp(point2, direction2, t);

            var obj1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj1.transform.position = temp1;
            
            var obj2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj2.transform.position = temp2;

        }

        return Vector3.zero;
    }

    
}