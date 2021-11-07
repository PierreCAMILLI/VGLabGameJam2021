using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorExtension
{

    public static Vector3 XZ(this Vector2 vector)
    {
        return new Vector3(vector.x, 0f, vector.y);
    }

    public static Vector2 XZ(this Vector3 vector)
    {
        return new Vector2(vector.x, vector.z);
    }

    public static Vector3 X0Z(this Vector3 vector)
    {
        return new Vector3(vector.x, 0f, vector.z);
    }
}