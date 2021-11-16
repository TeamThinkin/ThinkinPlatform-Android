using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepLevel : MonoBehaviour
{
    private void Update()
    {
        //transform.rotation *= Quaternion.FromToRotation(transform.up, Vector3.up);
        transform.rotation = Quaternion.LookRotation(transform.forward, Vector3.up);
    }
}
