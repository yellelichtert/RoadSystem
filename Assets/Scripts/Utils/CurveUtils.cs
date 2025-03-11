using Vector3 = UnityEngine.Vector3;

public static class CurveUtils
{
    
    public static Vector3[] CalculateCurve(int pCount, Vector3 cp1, Vector3 cp2, Vector3 cp3)
    {
        Vector3[] points = new Vector3[pCount];
        
        for (int i = 0; i < pCount; i++)
        {
            // ReSharper disable once PossibleLossOfFraction
            points[i - 1] = CalculateCurvePoint(1 / i, cp1, cp2, cp3);
        }

        return points;
    }
    

    
    public static Vector3 CalculateCurvePoint(float t, Vector3 cp1, Vector3 cp2, Vector3 cp3)
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


    public static Vector3 FindCenter(Vector3[] points)
    {
        
        Vector3? sum = null;

        for (int i = 0; i < points.Length; i++)
        {
            if (sum.HasValue)
            {
                sum += points[i];
            }
            else
            {
                sum = points[i];
            }
            
            
            
        }

        return sum.Value / points.Length;
    }

    
}