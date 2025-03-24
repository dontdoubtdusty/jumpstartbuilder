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
    public bool isArchetyped;
    public bool isMulticolored;
    public string type_line;
    public Image_Uris image_Uris; //Class defined in ScryfallCardHandler to store nested image_uri strings

    //This method is here for instances where CardData's MonoBehavior property prevents us from doing something
    public static Card FromCardData(CardData data)
    {
        if(data == null)
        {
            return null;
        }

        return new Card
        {
            cardName = data.cardName,
            colors = data.colors,
            rarity = data.rarity,
            manaCost = data.manaCost,
            oracle_text = data.oracle_text,
            archetypes = data.archetypes,
            isRemoval = data.isRemoval,
            isCreature = data.isCreature,
            isArchetyped = data.isArchetyped,
            isMulticolored = data.isMulticolored,
            type_line = data.type_line, 
            image_Uris = data.image_Uris
        };
    }
}
