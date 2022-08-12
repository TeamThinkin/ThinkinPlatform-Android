using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor;
using System.IO;

public class GenerateInfoAtPrebuild : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        GenerateInfoFile();
    }

    [MenuItem("Thinkin/Geneate Info")]
    public static void GenerateInfoFile()
    {
        File.WriteAllText("Assets/Generated/GeneratedInfo.cs", "public static class GeneratedInfo { public static string BundleVersionCode = \"" + PlayerSettings.Android.bundleVersionCode + "\"; }");
    }
}
