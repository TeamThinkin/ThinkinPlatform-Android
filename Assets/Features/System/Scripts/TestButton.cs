using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.InputSystem;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.SceneManagement;
using MongoDB.Bson;
using MongoDB.Driver;

public class TestButton : MonoBehaviour
{
    private XRIDefaultInputActions XRInputActions;

    public string SceneUrl1 = "http://localhost:9000/{PLATFORM}/catalog_ThinkinContent.json#Assets/Scenes/Classroom.unity";
    public string SceneUrl2 = "http://localhost:9000/{PLATFORM}/catalog_ThinkinContent.json#Assets/Scenes/Classroom.unity";
    public string SceneUrl3 = "http://localhost:9000/{PLATFORM}/catalog_ThinkinContent.json#Assets/Scenes/Classroom.unity";

    private const string MONGO_URI = "mongodb+srv://daemon:3f7T1Wkj9MRX6ld4@cluster0.zt9ue.mongodb.net/thinkin?retryWrites=true&w=majority"; //"mongodb://127.0.0.1:27017";
    private const string DATABASE_NAME = "thinkin";
    private MongoClient client;
    private IMongoDatabase db;
    public TMPro.TMP_Text label;


    private void Awake()
    {
        XRInputActions = new XRIDefaultInputActions();
        XRInputActions.UIControls.TestButton1.performed += TestButton1_performed;
        XRInputActions.UIControls.TestButton1.Enable();

        XRInputActions.UIControls.TestButton2.performed += TestButton2_performed;
        XRInputActions.UIControls.TestButton2.Enable();

        XRInputActions.UIControls.TestButton3.performed += TestButton3_performed;
        XRInputActions.UIControls.TestButton3.Enable();

        
    }

    private void TestButton1_performed(InputAction.CallbackContext obj)
    {
        Debug.Log("Test button fired");
        AppSceneManager.Instance.LoadRemoteScene(SceneUrl1);
    }

    private void TestButton2_performed(InputAction.CallbackContext obj)
    {
        Debug.Log("Test button fired");
        AppSceneManager.Instance.LoadRemoteScene(SceneUrl2);
    }

    private void TestButton3_performed(InputAction.CallbackContext obj)
    {
        Debug.Log("Test button fired");
        AppSceneManager.Instance.LoadRemoteScene(SceneUrl3);
    }

    private async void doDBTest()
    {
        label.text = "Fetching db records";
        
        var s = await Task.Run(() =>
        {
            client = new MongoClient(MONGO_URI);
            db = client.GetDatabase(DATABASE_NAME);
            IMongoCollection<DBUser> userCollection = db.GetCollection<DBUser>("users");

            string s = "DB Records:\n";
            var userModelList = userCollection.Find(user => true).ToList();
            foreach (var user in userModelList)
            {
                s += user.name + ": " + user.key + "\n";
            }
            return s;
        });
        label.text = s;

        
    }

    public void OnSelect()
    {
        transform.position += Vector3.up;
        //LoadScene("https://storage.googleapis.com/matriculate-assets/{PLATFORM}/catalog_ThinkinContent.json#Assets/Scenes/Classroom.unity");
        doDBTest();
    }
}


public class DBUser
{
    public ObjectId _id { set; get; }
    public string name { set; get; }

    public string key { get; set; }
}
