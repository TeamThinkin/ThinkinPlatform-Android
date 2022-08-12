using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class SetKeystoreFieldsOnStartup
{
    static SetKeystoreFieldsOnStartup()
    {
        string path = "c:\\ThinkinKeystore\\Key.hash";
        if (File.Exists(path))
        {
            string key = File.ReadAllText(path);
            PlayerSettings.keystorePass = key;
            PlayerSettings.keyaliasPass = key;
        }
    }
}
