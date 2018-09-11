using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cicle  {

    public Cicle(float radius, Vector2 center)
    {
        Radius = radius;
        Center = center;
    }

    public float Radius
    {
        get;
        private set;
    }

    public Vector2 Center
    {
        get;
        private set;
    }

    public bool PointIsInCircle(Vector2 point)
    {
        return Mathf.Pow(point.x - Center.x, 2) + Mathf.Pow(point.y - Center.y, 2) < Radius * Radius;
    }
}
