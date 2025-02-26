using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class DeckCreator : MonoBehaviour
{
    public PanelSelectionHandler panelSelectionHandler;

    public List<Card> LoadAllCards()
    {
        List<Card> loadedCards = new List<Card>();
        string filePath = Path.Combine(Application.dataPath, "CardData.json");

        if(File.Exists(filePath))
        {
            string cardsJson = File.ReadAllText(filePath);
            if(!string.IsNullOrEmpty(cardsJson))   //Makes sure there is a CardData.json file
            {
                try
                {
                    // Directly deserialize into List<Card>
                    loadedCards = JsonUtility.FromJson<ListWrapper<Card>>(WrapListToJson(cardsJson)).cards;

                }
                catch (Exception e) // Catch any exceptions that occur during JSON parsing.
                {
                    Debug.LogError("Error parsing JSON: " + e.Message); // Log the error message.
                }
            } 
            else
            {
                Debug.LogError("CardData.json not found at " + filePath);
            }
        }
        
        return loadedCards;
    }

    public Card LoadSingleCard(string cardName)
    {
        Debug.Log("Name being passed through here is: " + cardName);
        List<Card> loadedCards = LoadAllCards();

        if(loadedCards != null)
        {
            foreach(Card card in loadedCards)
            {
                if(card.cardName == cardName)
                {
                    return card;
                }
            }
        }
        else
        {
            Debug.LogError("No cards loaded"!);
            return null;
        }

        Debug.LogError("Returned null!");
        return null;
    }

    private class ListWrapper<T>
    {
        public List<T> cards;
    }

    private string WrapListToJson(string json)
    {
        return "{\"cards\":" + json + "}";
    }
}
