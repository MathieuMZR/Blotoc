using UnityEngine;

public static class Helper
{
    public static Vector3 Multiply(this Vector3 v1, Vector3 v2)
    {
        return new Vector3(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);
    }

    public static Vector3 Round(this Vector3 v1)
    {
        return new Vector3(
            Mathf.RoundToInt(v1.x),
            Mathf.RoundToInt(v1.y),
            Mathf.RoundToInt(v1.z));
    }
}
