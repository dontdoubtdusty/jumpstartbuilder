using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardRarity {Common, Uncommon, Rare, Mythic}


[System.Serializable]
public class Card
{
    public string cardName;
    public List<string> colors;
    public CardRarity rarity;
    public int manaCost;
    public string oracle_text;
    public List<string> archetypes;
    public bool isRemoval;
    public bool isCreature;
    public bool isMulticolored = false;
    public string type_line;
    public Image_Uris image_Uris; //Class defined in ScryfallCardHandler to store nested image_uri strings
}
