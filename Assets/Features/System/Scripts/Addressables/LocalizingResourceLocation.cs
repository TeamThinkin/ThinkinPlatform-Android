//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.ResourceManagement.ResourceLocations;

///// <summary>
///// Replaces url's in resource location with the hosting addresses domain authority and scheme
///// </summary>
//public class LocalizingResourceLocation : IResourceLocation
//{
//    private IResourceLocation sourceLocation;
//    private List<IResourceLocation> _dependencies;
//    private string _internalId;

//    // Something like this is in source.InternalId
//    // http://localhost:8000/Android/11468c80d76563d597afc98a29886fd5.bundle
//    //
//    // Something like this is in the host address
//    // https://storage.googleapis.com/matriculate-assets/{PLATFORM}/catalog_ThinkinContent.json#Assets/Scenes/Classroom.unity
//    //
//    // This will translate the source to this so that it can requested from the same server that provided the catalog
//    // https://storage.googleapis.com/matriculate-assets/Android/11468c80d76563d597afc98a29886fd5.bundle

//    public LocalizingResourceLocation(IResourceLocation source, string catalogAddress)
//    {
//        this.sourceLocation = source;

//        _internalId = source.InternalId;
//        if (source.InternalId.StartsWith("http"))
//        {
//            var sourceSplitIndex = source.InternalId.IndexOf(Config.PlatformKey);
//            var catalogSplitIndex = catalogAddress.IndexOf("{PLATFORM}");
//            string sourcePrefix = source.InternalId.Substring(0, sourceSplitIndex);
//            string hostPrefix = catalogAddress.Substring(0, catalogSplitIndex);
//            _internalId = _internalId.Replace(sourcePrefix, hostPrefix);
//        }

//        if (source.Dependencies != null)
//        {
//            _dependencies = new List<IResourceLocation>();
//            foreach (var dep in source.Dependencies)
//            {
//                _dependencies.Add(new LocalizingResourceLocation(dep, catalogAddress));
//            }
//        }
//    }

//    public string InternalId => _internalId;

//    public string ProviderId => sourceLocation.ProviderId;

//    public IList<IResourceLocation> Dependencies => _dependencies;

//    public int DependencyHashCode => sourceLocation.DependencyHashCode;

//    public bool HasDependencies => sourceLocation.HasDependencies;

//    public object Data => sourceLocation.Data;

//    public string PrimaryKey => sourceLocation.PrimaryKey;

//    public Type ResourceType => sourceLocation.ResourceType;

//    public int Hash(Type resultType)
//    {
//        return sourceLocation.Hash(resultType);
//    }
//}
