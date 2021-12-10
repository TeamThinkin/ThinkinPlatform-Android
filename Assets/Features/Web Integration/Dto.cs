using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegisterDeviceRequestDto
{
    [JsonProperty("uid")]
    public string Uid { get; set; }
}

public class RegisterDeviceResultDto : UserDto
{
    //[JsonProperty("domains")]
    //public DomainDto[] Domains { get; set; }
}

public class UserDto
{ 
    [JsonProperty("_id")]
    public string Id { get; set; }

    [JsonProperty("token")]
    public string Token { get; set; }

    [JsonProperty("hasPassword")]
    public bool HasPassword { get; set; }

    [JsonProperty("displayName")]
    public string DisplayName { get; set; }

    [JsonProperty("email")]
    public string Email { get; set; }

    [JsonProperty("avatarUrl")]
    public string AvatarUrl { get; set; }

    [JsonProperty("homeRoomUrl")]
    public string HomeRoomUrl { get; set; }

    [JsonProperty("currentRoomUrl")]
    public string CurrentRoomUrl { get; set; }
}

public class MapDto
{
    [JsonProperty("_id")]
    public string Id { get; set; }

    [JsonProperty("displayName")]
    public string DisplayName { get; set; }
}

//public class DomainDto
//{
//    [JsonProperty("_id")]
//    public string Id { get; set; }

//    [JsonProperty("displayName")]
//    public string DisplayName { get; set; }

//    [JsonProperty("manifestUrl")]
//    public string ManifestUrl { get; set; }
//}

public class RoomDto
{
    [JsonProperty("_id")]
    public string Id { get; set; }

    [JsonProperty("displayName")]
    public string DisplayName;

    [JsonProperty("domainId")]
    public string DomainId;

    [JsonProperty("environmentUrl")]
    public string EnvironmentUrl;
}


public class CollectionContentItemDto
{
    [JsonProperty("_id")]
    public string Id { get; set; }

    [JsonProperty("mimeType")]
    public string MimeType { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("displayName")]
    public string DisplayName { get; set; }
}

public class FileContentItemDto : CollectionContentItemDto
{
    [JsonProperty("url")]
    public string Url { get; set; }
}

[MimeType("environment/unity.addressable")]
public class EnvironmentContentItemDto : FileContentItemDto
{
}

[MimeType("environment/local")]
public class LocalSceneContentItemDto : CollectionContentItemDto
{
    [JsonProperty("path")]
    public string Path { get; set; }
}

[MimeType("image/jpg", "image/jpeg", "image/png")]
public class ImageContentItemDto : FileContentItemDto
{
    [JsonProperty("corsFriendly")]
    public bool IsCorsFriendly { get; set; }
}

[MimeType("link/room")]
public class RoomLinkContentItemDto : FileContentItemDto
{
}

[MimeType("layout/3d")]
public class Layout3dContentItemDto : FileContentItemDto
{
}

[MimeType("link/collection")]
public class CollectionLinkContentItemDto : FileContentItemDto
{
}

[MimeType("item-info/transform")]
public class ItemTransformDto : CollectionContentItemDto
{
    [JsonProperty("itemId")]
    public string ItemId { get; set; }

    [JsonProperty("position")]
    public Vector3Dto Position { get; set; }

    [JsonProperty("rotation")]
    public Vector4Dto Rotation{ get; set; }

    [JsonProperty("scale")]
    public Vector3Dto Scale { get; set; }
}

public class Vector3Dto
{
    public float x { get; set; }
    public float y { get; set; }
    public float z { get; set; }

}
public class Vector4Dto
{
    public float x { get; set; }
    public float y { get; set; }
    public float z { get; set; }
    public float w { get; set; }
}
