using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;


[System.Serializable]
public class CardData : MonoBehaviour
{
    public string cardName;
    public List<string> colors;
    public CardRarity rarity;
    public int manaCost;
    public string oracle_text;
    public List<string> archetypes;
    public bool isArchetyped;
    public bool isRemoval;
    public bool isCreature;
    public bool isMulticolored;
    public string type_line;
    public Image_Uris image_Uris;
}
