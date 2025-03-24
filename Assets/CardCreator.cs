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
    public bool isSearching = false;

    public void SearchScryfall(string searchQuery)
    {   
        isSearching = true;
        string url = $"https://api.scryfall.com/cards/search?q={UnityWebRequest.EscapeURL(searchQuery)}";
        StartCoroutine(GetCardData(url, searchQuery));
    }

    IEnumerator GetCardData(string searchUrl, string searchQuery)
    {
        while(!string.IsNullOrEmpty(searchUrl))
        using (UnityWebRequest webRequest = UnityWebRequest.Get(searchUrl))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string json = webRequest.downloadHandler.text;
                ScryfallSearchResult searchResult = JsonConvert.DeserializeObject<ScryfallSearchResult>(json);
                SaveHandler saveHandler = SaveHandler.instance;

                if (searchResult != null && searchResult.data != null)
                {
                    foreach(ScryfallCard scryfallCard in searchResult.data)
                    {
                        Card newCard = CreateCardFromScryfall(scryfallCard);
                        if (newCard != null)
                        {
                            saveHandler.allCards.Add(newCard);
                        }

                    }

                    //Check for the next page
                    if (!string.IsNullOrEmpty(searchResult.next_page))
                    {
                        searchUrl = searchResult.next_page;
                    }
                    else
                    {
                        searchResult = null; //No more pages
                        searchUrl = null;
                    }
                }
            }
            else
            {
                Debug.LogError("No cards found or invalid JSON response.");
                searchUrl = null; //Close the loop
            }
        }

                string url = $"https://api.scryfall.com/cards/search?q={UnityWebRequest.EscapeURL(searchQuery + " otag:removal")}";
                StartCoroutine(GetRemoval(url));
    }

    IEnumerator GetRemoval(string searchUrl)
    {
        while(!string.IsNullOrEmpty(searchUrl))
        using (UnityWebRequest webRequest = UnityWebRequest.Get(searchUrl + ""))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string json = webRequest.downloadHandler.text;
                ScryfallSearchResult searchResult = JsonConvert.DeserializeObject<ScryfallSearchResult>(json);
                SaveHandler saveHandler = SaveHandler.instance;

                if (searchResult != null && searchResult.data != null)
                {
                    foreach(ScryfallCard scryfallCard in searchResult.data)
                    {
                        Card newCard = CreateCardFromScryfall(scryfallCard);
                        if (newCard != null)
                        {
                            foreach(Card card in saveHandler.allCards)
                            {
                                if(card.cardName == newCard.cardName)
                                {
                                    card.isRemoval = true;
                                    //Debug.Log(card.cardName + " added to removal!");
                                }
                            }
                        }

                    }

                    //Check for the next page
                    if (!string.IsNullOrEmpty(searchResult.next_page))
                    {
                        searchUrl = searchResult.next_page;
                    }
                    else
                    {
                        searchResult = null; //No more pages
                        searchUrl = null;
                        isSearching = false;
                    }
                }
            }
            else
            {
                Debug.LogError("No cards found or invalid JSON response.");
                searchUrl = null; //Close the loop
            }
        }
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

            //Debug.Log("Colors: " + string.Join(", ", scryfallCard.colors)); // Inspect the array
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

        // Parse type line to determine creature or noncreature
        if (!string.IsNullOrEmpty(scryfallCard.type_line))
        {
            newCard.isCreature = scryfallCard.type_line.Contains("Creature");
        }

        
        newCard.type_line = scryfallCard.type_line;

        newCard.oracle_text = scryfallCard.oracle_text;
        newCard.image_Uris = new Image_Uris();
        newCard.image_Uris = scryfallCard.image_Uris;
        //newCard.archetypes = new List<string>(); 

        return newCard;
    }
    
}