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

