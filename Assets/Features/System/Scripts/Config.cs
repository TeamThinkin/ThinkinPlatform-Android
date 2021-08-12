using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class Config : MonoBehaviour
{
#if UNITY_EDITOR
    public const string PlatformKey = "StandaloneWindows";
#elif UNITY_STANDALONE || UNITY_EDITOR
    public const string PlatformKey = "StandaloneWindows64";
#elif UNITY_ANDROID
    public const string PlatformKey = "Android";
#endif
}
