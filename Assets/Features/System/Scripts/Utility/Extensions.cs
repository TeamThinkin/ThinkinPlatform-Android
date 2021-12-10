using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class Extensions
{
    public static Color Solid(this Color color)
    {
        color.a = 1;
        return color;
    }

    public static bool Contains(this LayerMask mask, int layer)
    {
        return mask == (mask | (1 << layer));
    }

    public static Task GetTask(this AsyncOperation asyncOperation)
    {
        var tcs = new TaskCompletionSource<object>();

        asyncOperation.completed += (AsyncOperation e) =>
        {
            tcs.SetResult(null);
        };

        return tcs.Task;
    }

    public static Task<T> GetTask<T>(this AsyncOperationHandle<T> asyncOpHandle)
    {
        var tcs = new TaskCompletionSource<T>();

        asyncOpHandle.Completed += (AsyncOperationHandle<T> e) =>
        {
            tcs.SetResult(e.Result);
        };

        return tcs.Task;
    }


    /// <summary>
    /// Resets the transforms local position, rotation and scale properties to default values
    /// </summary>
    /// <param name="transform"></param>
    public static void Reset(this Transform transform)
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

    /// <summary>
    /// Remaps the value from one number range to another
    /// </summary>
    public static float Remap(this float value, float originalMin, float originalMax, float targetMin, float targetMax, bool isClamped = false)
    {
        var newValue = (value - originalMin) / (originalMax - originalMin) * (targetMax - targetMin) + targetMin;
        if (isClamped) newValue = Mathf.Clamp(newValue, Mathf.Min(targetMax, targetMin), Mathf.Max(targetMax, targetMin));
        return newValue;
    }

    public static IEnumerable<Transform> GetChildren(this Transform parent)
    {
        foreach (Transform child in parent)
        {
            yield return child;
        }
    }

    public static void ClearChildren(this Transform parent)
    {
        if (parent == null) return;
        foreach (Transform child in parent)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    public static Vector3 FlattenX(this Vector3 vector)
    {
        vector.x = 0;
        return vector;
    }

    public static Vector3 FlattenY(this Vector3 vector)
    {
        vector.y = 0;
        return vector;
    }

    public static Vector3 FlattenZ(this Vector3 vector)
    {
        vector.z = 0;
        return vector;
    }

    public static Vector3 IsolateX(this Vector3 vector)
    {
        vector.y = 0;
        vector.z = 0;
        return vector;
    }

    public static Vector3 IsolateY(this Vector3 vector)
    {
        vector.x = 0;
        vector.z = 0;
        return vector;
    }

    public static Vector3 IsolateZ(this Vector3 vector)
    {
        vector.x = 0;
        vector.y = 0;
        return vector;
    }

    public static Vector3 Scale(this Vector3 vector, float x, float y, float z)
    {
        vector.x *= x;
        vector.y *= y;
        vector.z *= z;
        return vector;
    }

    public static Quaternion Straighten(this Quaternion rot)
    {
        return Quaternion.Euler(0, rot.eulerAngles.y, 0);
    }

    public static Vector3 Absolute(this Vector3 vector)
    {
        vector.x = Mathf.Abs(vector.x);
        vector.y = Mathf.Abs(vector.y);
        vector.z = Mathf.Abs(vector.z);
        return vector;
    }

    public static string ToJSON(this object o)
    {
        return JsonConvert.SerializeObject(o);
    }

    public static Vector3 GetRaycastPoint(this Plane plane, Ray ray)
    {
        plane.Raycast(ray, out float enter);
        return ray.origin + ray.direction * enter;
    }

    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> list)
    {
        return list.Where(i => i != null);
    }

    public static IEnumerable<TResult> SelectNotNull<T, TResult>(this IEnumerable<T> list, Func<T, TResult> selector)
    {
        return list.Select(selector).WhereNotNull();
    }

    public static string StripQuotes(this string s)
    {
        return s?.Substring(1, s.Length - 2);
    }
}