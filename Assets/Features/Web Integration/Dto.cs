using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserDto
{ 
    [JsonProperty("_id")]
    public string Id { get; set; }

    [JsonProperty("hasPassword")]
    public bool HasPassword { get; set; }

    [JsonProperty("displayName")]
    public string DisplayName { get; set; }

    [JsonProperty("email")]
    public string Email { get; set; }

    [JsonProperty("AvatarUrl")]
    public string AvatarUrl { get; set; }
}

public class RegisterDeviceResultDto : UserDto
{
    [JsonProperty("domains")]
    public DomainDto[] Domains { get; set; }
}

public class DomainDto
{
    [JsonProperty("_id")]
    public string Id { get; set; }

    [JsonProperty("displayName")]
    public string DisplayName { get; set; }

    [JsonProperty("manifestUrl")]
    public string ManifestUrl { get; set; }
}

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