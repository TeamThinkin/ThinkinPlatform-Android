using Jint;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AngleSharp;
using AngleSharp.Js;
using AngleSharp.XPath;
using System.Threading.Tasks;
using UnityEngine.Networking;
using AngleSharp.Scripting;
using System.Reflection;
using System.Linq;

public class TestDocumentLoader : MonoBehaviour
{
    public string Url;

    void Start()
    {
        //testJavascript();
        loadRoom();
    }

    private async void loadRoom()
    {
        Debug.Log("Loading...");
        await DocumentManager.LoadUrl(Url, this.transform);
        Debug.Log("Load complete");
    }

    private void testJavascript()
    {
       
        var engine = new Engine().SetValue("log", new Action<object>((o) =>
        {
            Debug.Log(o);
        }));


        engine.Execute(@"
            function hello() { 
                log('Hello World');
            };
 
            hello();
        ");

        Debug.Log("Javascript tested");
    }
}
