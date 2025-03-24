using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using Unity.VisualScripting;

public class SaveHandler : MonoBehaviour
{
    public List<Card> allCards = new List<Card>(); // List to store all cards
    public List<ListOfDecks.DeckEntry> allDecks = new List<ListOfDecks.DeckEntry>();
    string filePath = Path.Combine(Application.dataPath, "CardData.json");
    string cardsJson;
    string decksJson;
    public SaveWrapper savedGame = new SaveWrapper();
    public string saveFileName = "Default";
    public ArchetypeList archetypeList;
    [System.Serializable]
    public class SaveWrapper
    //The final save object, containing all saved parameters
    {
        public ListOfCards savedCards = new ListOfCards();
        public ListOfDecks savedDecks = new ListOfDecks();
    }
    [System.Serializable]
    public class ListOfCards
    {
        public List<Card> cards = new List<Card>();
    }
 
    [System.Serializable]
    public class ListOfDecks
    {
        [System.Serializable]
        public struct DeckEntry
        {
            public string deckName;
            public string deckArchetype;
            public List<Card> deckContents;

            public DeckEntry(string name, string archetype)
            {
                deckName = name;
                deckArchetype = archetype;
                deckContents = new List<Card>();
            }
        }
        public List<DeckEntry> decks = new List<DeckEntry>();
    }

    [System.Serializable]
    public class LandCard
    {
        public string cardName;
        public string manaColor;
    }

    public LandCard[] savedLands = new LandCard[10];

    public static SaveHandler instance;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); //Make this gameObject persist between scenes
        }
        else
        {
            Destroy(gameObject); //If there is already a SaveHandler instance, destroy this once
        }
    }



    void Start()
    {
        GetLandsList();
    }

    private void GetLandsList()
    {
        string[] colorSuffix = {"Heath", "Isle", "Moor", "Bluff", "Grove"};
        string[] colorPrefix = {"Citadel", "Sea", "Black Dragon", "Cliff", "Manor"};
        string[] manaColors = {"W", "U", "B", "R", "G"};

        for(int i = 0; i < 5; i++)
        {
            savedLands[i].manaColor = manaColors[i];
            savedLands[i].cardName = "Thriving " + colorSuffix[i];
        }

        for (int i = 5; i < 10; i++)
        {
            savedLands[i].manaColor = manaColors[i - 5];
            if (colorPrefix[i - 5] == "Cliff") // Check if the prefix is "Cliff"
            {
                savedLands[i].cardName = "Cliffgate"; // No space, lowercase "gate"
            }
            else
            {
                savedLands[i].cardName = colorPrefix[i - 5] + " Gate"; // Add space, uppercase "Gate"
            }
        }
    }

    public void UpdateWrapperAndSaveGame(string saveName)
    {
        if(savedGame == null)
        {
            Debug.LogError("No saved game found!");
        }
        if(saveName == "" || saveName == null)
        {
            saveFileName = "Default";
        }
        Debug.Log("Save name: " + saveName);
        UpdateSaveWrapper(savedGame); //Apply changes to SaveWrapper savedGame
        SaveGame(savedGame, saveName);
    }

    public void UpdateSaveWrapper(SaveWrapper saveWrapper)
    {
        Debug.Log("You are here!");
        Debug.Log("allCards: " + allCards.Count);
        if(saveWrapper == null)
        {
            Debug.LogError("No SaveWrapper!");
        }
        Debug.Log("saveWrapper here is: " + saveWrapper);
        WriteCardListToWrapper(saveWrapper); //Updates savedGame.cards
        Debug.Log("You are here 2!");
        WriteDecksListToWrapper(saveWrapper); //Updates savedGame.decks
    }
    public void SaveGame(SaveWrapper saveWrapper, string saveName)
    //Handles converting and saving a SaveWrapper object to a json
    {
        //ex. Coding/Jumpstart Pack Creator/saves/
        string saveFolderPath = Path.Combine(Application.dataPath, "saves");

        //Create the "saves" folder if it doesn't exist
        if(!Directory.Exists(saveFolderPath))
        {
            Directory.CreateDirectory(saveFolderPath);
        }

        string fileName = saveName + ".json";
        string filePath = Path.Combine(saveFolderPath, fileName);
        string json = JsonUtility.ToJson(saveWrapper, true);

        try
        {
            File.WriteAllText(filePath, json);
            Debug.Log("Game saved to: " + filePath);
        }
        catch(System.Exception ex)
        {
            Debug.Log("Error saving game: " + ex.Message);
        }

    }

    public SaveWrapper LoadGame(string saveName)
    //Function to load from a json
    //We turn a SaveWrapper into a json when we save
    //We want to turn a json back into a SaveWrapper when we load
    {
        string saveFolderPath = Path.Combine(Application.dataPath, "saves");
        string fileName = saveName + ".json";
        string filePath = Path.Combine(saveFolderPath, fileName);

        if(File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            SaveWrapper loadedData = JsonUtility.FromJson<SaveWrapper>(json);
            savedGame = loadedData;
            allCards = loadedData.savedCards.cards;
            allDecks = loadedData.savedDecks.decks;
            return loadedData;
        }
        else
        {
            Debug.LogError("No file found at " + filePath + " !");
            return null;
        }
    }

    public Card LoadSingleCard(string cardName)
    {
        Debug.Log("Name being passed through here is: " + cardName);
        //List<Card> loadedCards = LoadAllCards().cards;
        List<Card> loadedCards = savedGame.savedCards.cards; //cards is a List<Card> inside the savedCards ListOfCards class

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
        List<Card> savedCards = savedGame.savedCards.cards;
        Debug.Log("Cards in UpdateCard: " + savedCards.Count);
        for(int i = 0; i < savedCards.Count; i++)
        {
            if(savedCards[i].cardName == cardToEdit.cardName)
            {
                //WE NEED TO MAKE SURE THIS SAVES THE CHANGES, PERHAPS CALL SAVEGAME() and LOADGAME()
                savedCards[i].archetypes = cardToEdit.archetypes;
                savedCards[i].isArchetyped = cardToEdit.isArchetyped;
                
                //WriteAllCardsToFile();
                //LoadCardsFromFile();
                //Debug.Log(allCards[i] + " now contains: " + allCards[i].archetypes[0]);
                //Debug.Log("allCards at index " + i + " is " + allCards[i].cardName + " and has archetypes " + allCards[i].archetypes[0]);
            }
        }   
        SaveGame(savedGame, "DFT");
    }

    public void WriteCardListToWrapper(SaveWrapper saveWrapper)
    {
        //Debug.Log("saveWrapper: " + saveWrapper);

        if(allCards == null || allCards.Count == 0)
        {
            Debug.Log("allCards is null!");
        }
           Debug.Log("allCards contains: " + allCards.Count);

        saveWrapper.savedCards.cards = allCards;
        Debug.Log("saveWrapper.savedCards.cards = " + saveWrapper.savedCards.cards.Count);
    }

    public void WriteDecksListToWrapper(SaveWrapper saveWrapper)
    {
       // saveWrapper.savedDecks.decks = allDecks;
       Debug.Log("saveWrapper: " + saveWrapper);
       if(allDecks == null)
       {
            Debug.LogError("allDecks is null!");
       }

       saveWrapper.savedDecks.decks = allDecks;
    }
}
