using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Unity.VisualScripting;
using System.Resources;


public class CardCreator : MonoBehaviour
{
    public List<Card> allCards = new List<Card>(); // List to store all cards
    string filePath = Path.Combine(Application.dataPath, "CardData.json");

    public void SearchScryfall(string searchQuery)
    {   
        string url = $"https://api.scryfall.com/cards/search?q={UnityWebRequest.EscapeURL(searchQuery)}";
        StartCoroutine(GetCardData(url));
    }

    IEnumerator GetCardData(string searchQuery)
    {
        while(!string.IsNullOrEmpty(searchQuery))
        using (UnityWebRequest webRequest = UnityWebRequest.Get(searchQuery))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string json = webRequest.downloadHandler.text;
                ScryfallSearchResult searchResult = JsonConvert.DeserializeObject<ScryfallSearchResult>(json);

                if (searchResult != null && searchResult.data != null)
                {
                    foreach(ScryfallCard scryfallCard in searchResult.data)
                    {
                        Card newCard = CreateCardFromScryfall(scryfallCard);
                        if (newCard != null)
                        {
                            allCards.Add(newCard);
                            //WriteCardToFile(newCard);
                        }

                    }

                    //Check for the next page
                    if (!string.IsNullOrEmpty(searchResult.next_page))
                    {
                        searchQuery = searchResult.next_page;
                    }
                    else
                    {
                        searchResult = null; //No more pages
                        searchQuery = null;
                    }
                }
            }
            else
            {
                Debug.LogError("No cards found or invalid JSON response.");
                searchQuery = null; //Close the loop
            }
        }

                    WriteAllCardsToFile();
    }
    public Card CreateCardFromScryfall(ScryfallCard scryfallCard)
    {
        if (scryfallCard == null) return null;

        Card newCard = new Card();
        newCard.cardName = scryfallCard.name;

        // Parse mana cost 
        if (!string.IsNullOrEmpty(scryfallCard.mana_cost))
        {
            newCard.manaCost = (int)scryfallCard.cmc;
        }

        // Parse color
        if (scryfallCard.colors != null && scryfallCard.colors.Length > 0)
        {

            newCard.colors = new List<string>();

            Debug.Log("Colors: " + string.Join(", ", scryfallCard.colors)); // Inspect the array
            if (Array.IndexOf(scryfallCard.colors, "W") >= 0) newCard.colors.Add("W");
            if (Array.IndexOf(scryfallCard.colors, "U") >= 0) newCard.colors.Add("U");
            if (Array.IndexOf(scryfallCard.colors, "B") >= 0) newCard.colors.Add("B");
            if (Array.IndexOf(scryfallCard.colors, "R") >= 0) newCard.colors.Add("R");
            if (Array.IndexOf(scryfallCard.colors, "G") >= 0) newCard.colors.Add("G");

            // Check for multicolored
            if(newCard.colors.Count > 1)
            {
                newCard.isMulticolored = true;
                newCard.colors.Add("M");
            }
            
            //Check for colorless
            if(newCard.colors.Count == 0)
            {
                newCard.colors.Add("C");
            }
        }
        else
        {
            newCard.colors = new List<string>();
            newCard.colors.Add("C");
        }

    

        // Parse rarity
        if (!string.IsNullOrEmpty(scryfallCard.rarity))
        {
            if (scryfallCard.rarity.ToLower() == "common") newCard.rarity = CardRarity.Common;
            if (scryfallCard.rarity.ToLower() == "uncommon") newCard.rarity = CardRarity.Uncommon;
            if (scryfallCard.rarity.ToLower() == "rare") newCard.rarity = CardRarity.Rare;
            if (scryfallCard.rarity.ToLower() == "mythic") newCard.rarity = CardRarity.Mythic;
        }

        // Parse type line to determine creature or removal
        if (!string.IsNullOrEmpty(scryfallCard.type_line))
        {
            newCard.isCreature = scryfallCard.type_line.Contains("Creature");
            newCard.isRemoval = scryfallCard.type_line.Contains("Destroy") || scryfallCard.type_line.Contains("Exile"); // Example
        }

        // You'll need to expand this to handle archetypes and other properties
        newCard.archetypes = new List<string>(); // Example: Add archetypes if you can parse them
        newCard.type_line = scryfallCard.type_line;

        newCard.image_Uris = new Image_Uris();
        newCard.image_Uris = scryfallCard.image_Uris;

        return newCard;
    }
    
    public void WriteAllCardsToFile()
    {
        ListOfCards listOfCards = new ListOfCards();
        listOfCards.cards = allCards;
        string json = JsonUtility.ToJson(listOfCards, true);
        File.WriteAllText(filePath, json);
    }
    
    public class ListOfCards
    {
        public List<Card> cards;
    }
}