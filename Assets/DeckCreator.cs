using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

/*public class DeckCreator : MonoBehaviour
{
    public PanelSelectionHandler panelSelectionHandler;
    public TextAsset cardListSource;
    public CardCreator cardCreator;

    public CardCreator.ListOfCards LoadAllCards()
    {
        CardCreator.ListOfCards listOfCards = new CardCreator.ListOfCards(); //ListOfCards contains List<Card> cards
        List<Card> loadedCards;
        string filePath = Path.Combine(Application.dataPath, "CardData.json");

            string cardsJson = cardListSource.text;
            Debug.Log("JSON: " + cardsJson);
            if(!string.IsNullOrEmpty(cardsJson))   //Makes sure there is a CardData.json file
            {
                try
                {
                    //Create a new ListOfCards object, which can be deserialized into a JSON
                    listOfCards = JsonUtility.FromJson<CardCreator.ListOfCards>(cardsJson);
                    loadedCards = listOfCards.cards; //Retrieve list 'cards' from within ListOfCards
                    Debug.Log("loadedCards: " + loadedCards + " contains " + loadedCards.Count);

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



}*/
