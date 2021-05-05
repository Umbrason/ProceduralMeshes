using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class ShapeMath
{
    public static Vector3 RandomPointOnTriangle(Vector3 A, Vector3 B, Vector3 C, out Vector3 normal)
    {        
        Vector3 AB = B - A;
        Vector3 AC = C - A;
        float x = Random.Range(0, 1f);
        float y = Random.Range(0, 1f);
        if (x + y > 1)
        {
            x = 1 - x;
            y = 1 - y;
        }
        normal = Vector3.Cross(AB, AC).normalized;
        return x * AB + y * AC + A;
    }
    
}
