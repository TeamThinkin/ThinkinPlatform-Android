using System.IO;
using System.Text;
using System.Xml;
using UnityEditor.Android;
using UnityEngine;

public class ModifyAndroidManifest : IPostGenerateGradleAndroidProject
{
    public void OnPostGenerateGradleAndroidProject(string basePath)
    {
        var manifestPath = GetManifestPath(basePath);

        var contents = File.ReadAllText(manifestPath);
        contents = contents.Replace("<uses-permission android:name=\"android.permission.CAMERA\" />", "");
        File.WriteAllText(manifestPath, contents);
    }

    public int callbackOrder { get { return 1; } }

    private string _manifestFilePath;

    private string GetManifestPath(string basePath)
    {
        if (string.IsNullOrEmpty(_manifestFilePath))
        {
            var pathBuilder = new StringBuilder(basePath);
            pathBuilder.Append(Path.DirectorySeparatorChar).Append("src");
            pathBuilder.Append(Path.DirectorySeparatorChar).Append("main");
            pathBuilder.Append(Path.DirectorySeparatorChar).Append("AndroidManifest.xml");
            _manifestFilePath = pathBuilder.ToString();
        }
        return _manifestFilePath;
    }
}