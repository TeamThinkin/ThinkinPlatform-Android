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

    public string SceneUrl = "http://localhost:9000/{PLATFORM}/catalog_ThinkinContent.json#Assets/Scenes/Classroom.unity";
    public bool UseCloudAddress;

    //private const string MONGO_URI = "mongodb+srv://daemon:3f7T1Wkj9MRX6ld4@cluster0.zt9ue.mongodb.net/thinkin?retryWrites=true&w=majority"; //"mongodb://127.0.0.1:27017";
    //private const string DATABASE_NAME = "thinkin";
    //private MongoClient client;
    //private IMongoDatabase db;


    //private void Awake()
    //{
    //    XRInputActions = new XRIDefaultInputActions();
    //    XRInputActions.UIControls.TestButton1.performed += TestButton1_performed;
    //    XRInputActions.UIControls.TestButton1.Enable();

    //    XRInputActions.UIControls.TestButton2.performed += TestButton2_performed;
    //    XRInputActions.UIControls.TestButton2.Enable();

    //    XRInputActions.UIControls.TestButton3.performed += TestButton3_performed;
    //    XRInputActions.UIControls.TestButton3.Enable();
    //}

    //private void OnDestroy()
    //{
    //    XRInputActions.UIControls.TestButton1.performed -= TestButton1_performed;
    //    XRInputActions.UIControls.TestButton2.performed -= TestButton2_performed;
    //    XRInputActions.UIControls.TestButton3.performed -= TestButton3_performed;
    //}


    public void OnSelect()
    {
        transform.position += Vector3.up * 0.1f;
        string url = SceneUrl;
        if(UseCloudAddress) url = url.Replace("http://localhost:8000/", "https://storage.googleapis.com/matriculate-assets/");
        AppSceneManager.Instance.LoadRemoteScene(url);
    }

    //private void TestButton1_performed(InputAction.CallbackContext obj)
    //{
    //    Debug.Log("Test button 1 fired");
    //    AppSceneManager.Instance.LoadRemoteScene(SceneUrl1);
    //}

    //private void TestButton2_performed(InputAction.CallbackContext obj)
    //{
    //    Debug.Log("Test button 2 fired");
    //    AppSceneManager.Instance.LoadRemoteScene(SceneUrl2);
    //}

    //private void TestButton3_performed(InputAction.CallbackContext obj)
    //{
    //    Debug.Log("Test button 3 fired");
    //    if(UserInfo.CurrentUser != null)
    //        UserInfo.CurrentUser.Logout();
    //    else
    //    {
    //        Debug.Log("Cant log out because no user logged in???");
    //    }
    //    //AppSceneManager.Instance.LoadRemoteScene(SceneUrl3);
    //}

    //private async void doDBTest()
    //{
    //    label.text = "Fetching db records";
        
    //    var s = await Task.Run(() =>
    //    {
    //        client = new MongoClient(MONGO_URI);
    //        db = client.GetDatabase(DATABASE_NAME);
    //        IMongoCollection<DBUser> userCollection = db.GetCollection<DBUser>("users");

    //        string s = "DB Records:\n";
    //        var userModelList = userCollection.Find(user => true).ToList();
    //        foreach (var user in userModelList)
    //        {
    //            s += user.name + ": " + user.key + "\n";
    //        }
    //        return s;
    //    });
    //    label.text = s;

        
    //}

}


//public class DBUser
//{
//    public ObjectId _id { set; get; }
//    public string name { set; get; }

//    public string key { get; set; }
//}
