using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class SaveHandler : MonoBehaviour
{
    public TextAsset cardListSource;
    public CardCreator cardCreator;
    public List<Card> allCards = new List<Card>(); // List to store all cards
    string filePath = Path.Combine(Application.dataPath, "CardData.json");
    string cardsJson;

    public class ListOfCards
    {
        public List<Card> cards;
    }

    public ListOfCards LoadAllCards()
    {
        ListOfCards listOfCards = new ListOfCards(); //ListOfCards contains List<Card> cards
        List<Card> loadedCards;
        string filePath = Path.Combine(Application.dataPath, "CardData.json");

        if(File.Exists(filePath))
        {
            try
            {
                cardsJson = File.ReadAllText(filePath);
                listOfCards = JsonUtility.FromJson<ListOfCards>(cardsJson);
                loadedCards = listOfCards.cards; //Retrieve list 'cards' from within ListOfCards
                Debug.Log("loadedCards: " + loadedCards + " contains " + loadedCards.Count);
            }
            catch (Exception e)
            {
                Debug.Log("Error: " + e);
            }
        }
        else
        {
            Debug.LogError("CardData file not found at " + filePath);
        }

        if(listOfCards == null)
        {
            Debug.LogError("listOfCards is null!");
        }
        return listOfCards;
    }

    public Card LoadSingleCard(string cardName)
    {
        Debug.Log("Name being passed through here is: " + cardName);
        List<Card> loadedCards = LoadAllCards().cards;

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

    public void UpdateCard(Card cardToEdit)
    //Method that passes a Card object through and updates/edits the archetypes and isArchetyped bool
    {
        LoadCardsFromFile();
        Debug.Log("allCards in UpdateCard: " + allCards.Count);
        for(int i = 0; i < allCards.Count; i++)
        {
            if(allCards[i].cardName == cardToEdit.cardName)
            {
                allCards[i].archetypes = cardToEdit.archetypes;
                allCards[i].isArchetyped = cardToEdit.isArchetyped;
                WriteAllCardsToFile();
                LoadCardsFromFile();
                //Debug.Log(allCards[i] + " now contains: " + allCards[i].archetypes[0]);
                //Debug.Log("allCards at index " + i + " is " + allCards[i].cardName + " and has archetypes " + allCards[i].archetypes[0]);
            }
        }   
    }

    public void WriteAllCardsToFile()
    {
        ListOfCards listOfCards = new ListOfCards();
        listOfCards.cards = allCards;
        Debug.Log("Writing " + allCards.Count + " cards to file.");
        string json = JsonUtility.ToJson(listOfCards, true);
        File.WriteAllText(filePath, json);
    }

    public void LoadCardsFromFile()
    {
        allCards = LoadAllCards().cards;
    }

}
