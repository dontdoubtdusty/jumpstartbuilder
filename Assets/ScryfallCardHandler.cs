using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

[System.Serializable]
public class Image_Uris
{
    public string small;
    public string normal;
    public string large;
    public string png;
    public string art_crop;
    public string border_crop;
}
[System.Serializable]
public class ScryfallCard
{
    public string name;
    public string mana_cost; 
    public float cmc;
    public string type_line;
    public string rarity;
    public string[] colors;
    public Image_Uris image_Uris;
}
[System.Serializable]
public class ScryfallSearchResult
{
    public ScryfallCard[] data;
    public string next_page;
    public bool has_more;
}


