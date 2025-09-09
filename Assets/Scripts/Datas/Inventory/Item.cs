using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class Item
{
    [JsonProperty]
    public int ID { get; set; }
    [JsonProperty]
    public int Star { get; set;}
    [JsonProperty]
    public int Level { get; set;}
}
