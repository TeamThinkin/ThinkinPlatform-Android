using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EulerToQuaternionHelper : MonoBehaviour
{
    [MenuItem("Thinkin/Euler To Quaternion")]
    static void LogEulerToQuaternion()
    {
        if(Selection.activeGameObject != null)
        {
            Debug.Log(Selection.activeGameObject.transform.rotation.x.ToString("0.00000") + ", " 
                + Selection.activeGameObject.transform.rotation.y.ToString("0.00000") + ", " 
                + Selection.activeGameObject.transform.rotation.z.ToString("0.00000") + ", " 
                + Selection.activeGameObject.transform.rotation.w.ToString("0.00000"));
        }
    }
}
