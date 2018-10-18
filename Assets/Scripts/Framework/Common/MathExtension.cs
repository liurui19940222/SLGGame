using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathExtension {

    public static void Truncate(this Vector3 vec3, float length)
    {
        float mag = vec3.magnitude;
        if (mag <= length)
            return;
        vec3 = vec3 / mag * length;
    }

}
