using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class DeckCreatorUI : MonoBehaviour
{
    [Header("Important Stuff (Boring)")]
    public PanelSelectionHandler panelSelectionHandler;
    public CardData cardData; 
    public List<ColorMapping> colorMapping; //Our list of MTG colors
    SaveHandler saveHandler = SaveHandler.instance;

    [Header("Card List")]
    public GameObject cardPanelPrefab;
    public GameObject archetypeHeaderPanelPrefab; //The panels showing card name and mana cost on the right side
    public Transform contentPanel; //The ScrollView's content panel, containing the instantiated cards
    // Start is called before the first frame update
    public string currentFilterColor;
    public List<string> ignoreList;
    public Toggle multicolorToggle, colorlessToggle;

    [Header("Deck List")]
    public GameObject creaturesPanel;
    public GameObject noncreaturesPanel;
    public GameObject thrivingLandPanel, thrivingGatePanel;
    public TextMeshProUGUI cardCountDisplayText;

    [Header("Archtypes List")]
    public GameObject archetypePanel1;
    public GameObject archetypePanel2, archetypePanel3, archetypePanel4;
    public GameObject rarePanel, removalPanel, signaturePanel;
    public ArchetypeList archetypeList;
    public GameObject statTextPrefab;

    [Header("Pack Saving")]
    public Button saveButton;
    public TMP_Dropdown deckArchetypeInput;

    [System.Serializable]
    public struct ColorMapping
    {
        public string colorName;
        public Color colorValue;
    }

    public void Start()
    {
        //Load full list of cards to start with
        currentFilterColor = "INIT";
        ignoreList = new List<string>();
        //THIS IS JUST A PLACEHOLDER FILE FOR TESTING PURPOSES
        //REPLACE THIS WITH A USER INPUT
        //ALSO TAKE A USER INPUT
        //DON'T FORGET THAT
        saveHandler.LoadGame("DFT");
    }

    public void OnColorButtonClick(GameObject button)
    {
        ColorButtonData colorData = button.GetComponent<ColorButtonData>();
        if (colorData != null)
        {   
            currentFilterColor = colorData.color;
            //Debug.Log("colorData.color test: " + colorData.color);
            //Debug.Log("currentFilterColor = " + currentFilterColor);
            if(ignoreList != null)
            {
             //Debug.Log("Ignore list found! Containing: " + ignoreList.Count);
            }
            LoadAndDisplayCards(currentFilterColor, ignoreList);
            UpdateLandPanels();
            UpdateArchetypePanels();
            SetArchetypeOptions();
        }
        else
        {
            Debug.LogError("ColorButtonData component not found on button!");
        }
    }

    public string GetStoredFilterColor()
    {
        //Debug.Log("currentFilterColor X7: " + currentFilterColor);
        return currentFilterColor;
    }

    public void GetCardsInDeck()
    {
        //DONT FORGET TO MAKE THIS A LIST<STRING> METHOD WHEN DONE TESTING!!!!
        //
        //

        List<string> cards = new List<string>();
        string cardName;
        //i set to 1 to skip "Creatures" and "Noncreatures" header panels
        //The panel names are the same as the card names
        for(int i = 1; i < creaturesPanel.transform.childCount; i++)
        {
            cardName = creaturesPanel.transform.GetChild(i).name;
            cards.Add(cardName);
        }
        for(int i = 1; i < noncreaturesPanel.transform.childCount; i++)
        {
            cardName = noncreaturesPanel.transform.GetChild(i).name;
            cards.Add(cardName);
        }

        Debug.Log("cards contains " + cards.Count + " cards");
        //return cards;
    }

    public void DisplayArchetypesAndCards(List<Card> cards, List<string> ignoredCardsList)
    {
        //Create the list of filtered cards first
        List<Card> filteredCards = new List<Card>();
        List<Card> removalCards = new List<Card>();

        currentFilterColor = GetStoredFilterColor();

        foreach(Card card in cards)
        {
            if(!ignoredCardsList.Contains(card.cardName)) //Check card is not on ignored list
            {
                if(card.colors.Contains(currentFilterColor)) //Check card contains stored filter color
                {
                    filteredCards.Add(card);
                }
            }
        }

        // Sort filteredCards by mana cost OR card name using a bubble sort. It just works idk.

        filteredCards.Sort((a, b) => a.manaCost.CompareTo(b.manaCost));
        //filteredCards.Sort((a, b) => a.cardName.CompareTo(b.cardName));

        //Retrieve the list of applicable archetypes based on currentFilterColor
        List<string> matchingArchetypes = new List<string>();
        foreach(ArchetypeList.ArchetypeColorPair archetypeColorPair in archetypeList.archetypeColorPairs)
        {
            if(archetypeColorPair.color1 == currentFilterColor || archetypeColorPair.color2 == currentFilterColor)
            {
                matchingArchetypes.Add(archetypeColorPair.archetypeName);
                Debug.Log("Archetype name: " + archetypeColorPair.archetypeName);
            }
        }
        //Instantiate a header for each archetype
        //Followed by instantiating all the cards that fit under that archetype
        foreach(string archetypeName in matchingArchetypes)
        {
            LoadArchetypeHeader(archetypeName);
            foreach(Card card in filteredCards)
            {
                if(card.archetypes.Contains(archetypeName))
                {
                    DisplayCardInScrollview(card);
                }
            }
            
        }

        LoadArchetypeHeader("Removal");
        foreach(Card card in filteredCards)
        {
            if(card.isRemoval)
            {
                DisplayCardInScrollview(card);
            }
        }
        //Do the same for removal
        //Do the same for signature cards
    }

    public void LoadArchetypeHeader(string archetype)
    {
            GameObject newArchetypePanel = Instantiate(archetypeHeaderPanelPrefab, contentPanel);
            TextMeshProUGUI newPanelName = newArchetypePanel.transform.Find("Card Name").GetComponent<TextMeshProUGUI>();
            newPanelName.text = archetype;
    }

    public void DisplayCardInScrollview(Card card)
    {
        //Substantiate prefab Card Panel
        //contentPanel is the scrollview Content object
        GameObject newCardPanel = Instantiate(cardPanelPrefab, contentPanel);

        //Add click listener
        newCardPanel.GetComponent<Button>().onClick.AddListener(() => panelSelectionHandler.SelectPanel(newCardPanel));
        newCardPanel.name = card.cardName;

        SetCardPanelColor(newCardPanel.GetComponent<UnityEngine.UI.Image>(), card.colors[0]);

        //Find the name and mana cost TMP text boxes on the card panels

        TextMeshProUGUI newCardName = newCardPanel.transform.Find("Card Name").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI newCardManaCost = newCardPanel.transform.Find("Card Cost").GetComponent<TextMeshProUGUI>();

        if(newCardName != null && newCardManaCost != null)
        //Replace the placeholder text of the Card Panels with the card object's info
        {
            newCardName.text = card.cardName;
            newCardManaCost.text = card.manaCost.ToString();
        }
        else
        {
            Debug.LogError("Card Name or Card Cost TMP elements are null!");
        }

        CardData panelCardData = newCardPanel.GetComponent<CardData>();
        panelCardData.cardName = card.cardName;
        panelCardData.colors = card.colors;
        panelCardData.rarity = card.rarity;
        panelCardData.manaCost = card.manaCost;
        panelCardData.oracle_text = card.oracle_text;
        panelCardData.archetypes = card.archetypes;
        panelCardData.isRemoval = card.isRemoval;
        panelCardData.isCreature = card.isCreature;
        panelCardData.isMulticolored = card.isMulticolored;
        panelCardData.type_line = card.type_line;
        panelCardData.image_Uris = card.image_Uris; 
    }

    public void LoadAndDisplayCards(string filterColor, List<string> ignoredCardsList)
    {
        //Kill the cards already there, kill them dead
        foreach(Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }
        //USING A PLACEHOLDER FILE FOR TESTING PURPOSES
        //DON'T FORGET TO REPLACE
        List<Card> cards = saveHandler.savedGame.savedCards.cards;

        DisplayArchetypesAndCards(cards, ignoredCardsList);
    }


    public void SetCardPanelColor(UnityEngine.UI.Image cardPanel, string cardColor)
    {
        UnityEngine.UI.Image image = cardPanel;
        if (image == null) return; 

        foreach(ColorMapping colorMapping in colorMapping)
        {
            if (cardColor == colorMapping.colorName)
            {
                image.color = colorMapping.colorValue;
            }
        }
    }

    /* Currently multicolored cards are not imported so this method is unused
    
    public void MulticolorToggle()
    {
        bool toggleOn = multicolorToggle.GetComponent<Toggle>().isOn;
        if (!toggleOn) //If off, remove all multicolored cards from the list
        {
            foreach(Card card in saveHandler.LoadAllCards().cards)
            {
                if (card.colors.Contains("M"))
                {
                    ignoreList.Add(card.cardName);
                    LoadAndDisplayCards(currentFilterColor, ignoreList);
                }
            }
            return;
        }
        
        foreach(Card card in saveHandler.LoadAllCards().cards)
        {
            if (card.colors.Contains("M"))
            {
                ignoreList.Remove(card.cardName);
            }
        }

        LoadAndDisplayCards(currentFilterColor, ignoreList);
        return;
    } */

    public void ColorlessToggle()
    {
        List<Card> cards = saveHandler.savedGame.savedCards.cards;
        bool toggleOn = colorlessToggle.GetComponent<Toggle>().isOn;
        if (!toggleOn) //If off, remove all multicolored cards from the list
        {
            foreach(Card card in cards)
            {
                if (card.colors.Contains("C"))
                {
                    ignoreList.Add(card.cardName);
                    LoadAndDisplayCards(currentFilterColor, ignoreList);
                }
            }
            return;
        }
        
        foreach(Card card in cards)
        {
            if (card.colors.Contains("C"))
            {
                ignoreList.Remove(card.cardName);
            }
        }

        LoadAndDisplayCards(currentFilterColor, ignoreList);
        return;
    }

    public void UpdateCategoryCounts()
    {
        int creatureCardsCount = 0;
        int noncreatureCardsCount = 0;
        //Get list of child objects under the category panel, which equals the number of cards in that category
        foreach(Transform child in creaturesPanel.transform)
        {
            creatureCardsCount++;
        }

        foreach(Transform child in noncreaturesPanel.transform)
        {
            noncreatureCardsCount++;
        }

        creatureCardsCount--; //Minus 1 to exclude the category name panel
        noncreatureCardsCount--;

        Debug.Log("creaturesPanel.transform: " + creaturesPanel.transform);
        //Update the text of the Card Count text box to the cardCount
        UpdateCardCountText(creaturesPanel.transform, noncreaturesPanel.transform, creatureCardsCount, noncreatureCardsCount);
        //Update the card count display text to the total
        UpdateCardCountDisplay(creatureCardsCount + noncreatureCardsCount);
    }

    private void UpdateCardCountText(Transform creaturesParentPanel, Transform noncreaturesParentPanel, int creatureCount, int noncreatureCount)
    {
        //Find the panels labeling the Creatures and Noncreatures side
        Transform creaturesPanel = creaturesParentPanel.Find("Creatures");
        Transform noncreaturesPanel = noncreaturesParentPanel.Find("Noncreatures");

        //Find the panel containing the Card Count text box
        Transform creatureCountPanel = creaturesPanel.Find("Card Count");
        Transform noncreatureCountPanel = noncreaturesPanel.Find("Card Count");

        //Find the text box with the card count
        TextMeshProUGUI creatureCountText = creatureCountPanel.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI noncreatureCountText = noncreatureCountPanel.GetComponent<TextMeshProUGUI>();

        //Update the card counts
        creatureCountText.text = creatureCount.ToString();
        noncreatureCountText.text = noncreatureCount.ToString();
    }

    public void SaveCurrentDeck()
    {
        //Create an initial list to hold all the collected CardData components
        List<CardData> currentDeckList = new List<CardData>();
        //Create a ListOfDecks object that will hold the deck name and list of DeckEntry objects
        SaveHandler.ListOfDecks listOfDecks = new SaveHandler.ListOfDecks();

        //Iterate through creature/noncreature cards and add them to the list
        for(int i = 1; i < creaturesPanel.transform.childCount; i++) //+1 to skip the header panels
        {
            currentDeckList.Add(creaturesPanel.transform.GetChild(i).GetComponent<CardData>());
        }
        for(int i = 1; i < noncreaturesPanel.transform.childCount; i++) //+1 to skip the header panels
        {
            currentDeckList.Add(noncreaturesPanel.transform.GetChild(i).GetComponent<CardData>());
        }

        Debug.Log("deckList contains " + currentDeckList.Count + " CardData objects.");
        //Insert name creating logic here
        string newDeckName = GetSelectedArchetype();
        //Get saved deck archetype
        string newDeckArchetype = GetSelectedArchetype();

        //Create a DeckEntry object that will hold each deck's name and contents
        SaveHandler.ListOfDecks.DeckEntry newDeckEntry = new SaveHandler.ListOfDecks.DeckEntry(newDeckName, newDeckArchetype);

        //Add each card in the list to deckContents
        foreach(CardData card in currentDeckList)
        {
            //The CardData class is MonoBehavior so it is not serializable
            //Use FromCardData to convert each CardData into a serializable Card object
            Card newCard = Card.FromCardData(card);
            if(newDeckEntry.deckContents == null)
            {
                Debug.LogError("No deck contents!");
            }
            //Add each card to the deckContents
            newDeckEntry.deckContents.Add(newCard);
        }

        Debug.Log("This deck's name is " + newDeckEntry.deckName + " and it contains " + newDeckEntry.deckContents.Count + " cards.");

        saveHandler.allDecks.Add(newDeckEntry);
        //
        //
        // THIS NEEDS TO BE REPLACED WITH A USER SAVE INPUT
        //
        //
        //Add saved deck to save
        saveHandler.UpdateWrapperAndSaveGame("DFT");
    }

    private void UpdateLandPanels()
    //Auto-fill the two land slots for each deck based on filter color
    {
        switch(currentFilterColor)
        {
            case "W": 
                thrivingLandPanel.GetComponentInChildren<TextMeshProUGUI>().text = "Thriving Heath";
                thrivingGatePanel.GetComponentInChildren<TextMeshProUGUI>().text = "Citadel Gate";
                break;
            case "U": 
                thrivingLandPanel.GetComponentInChildren<TextMeshProUGUI>().text = "Thriving Isle";
                thrivingGatePanel.GetComponentInChildren<TextMeshProUGUI>().text = "Sea Gate";
                break;
            case "B": 
                thrivingLandPanel.GetComponentInChildren<TextMeshProUGUI>().text = "Thriving Moor";
                thrivingGatePanel.GetComponentInChildren<TextMeshProUGUI>().text = "Black Dragon Gate";
                break;
            case "R": 
                thrivingLandPanel.GetComponentInChildren<TextMeshProUGUI>().text = "Thriving Bluff";
                thrivingGatePanel.GetComponentInChildren<TextMeshProUGUI>().text = "Cliffgate";
                break;
            case "G": 
                thrivingLandPanel.GetComponentInChildren<TextMeshProUGUI>().text = "Thriving Grove";
                thrivingGatePanel.GetComponentInChildren<TextMeshProUGUI>().text = "Manor Gate";
                break;
        }
    }
    

    public List<string> GetMatchingArchetypes(string colorChoice)
    {
        //Create a list to store the correct archetypes
        List<string> matchingArchetypes = new List<string>();
        //Extract the correct four archetypes
        foreach(ArchetypeList.ArchetypeColorPair archetypeColorPair in archetypeList.archetypeColorPairs)
        {
            if(archetypeColorPair.color1 == colorChoice || archetypeColorPair.color2 == colorChoice)
            {
                matchingArchetypes.Add(archetypeColorPair.archetypeName);
                Debug.Log("Archetype name: " + archetypeColorPair.archetypeName);
            }
        }
        
        return matchingArchetypes;
    }
    public void UpdateArchetypePanels()
    {
        List<string> matchingArchetypes = GetMatchingArchetypes(currentFilterColor);

        //Find the panels that hold the archetype names
        Transform archetype1 = archetypePanel1.transform.Find("Archetype Name");
        Transform archetype2 = archetypePanel2.transform.Find("Archetype Name");
        Transform archetype3 = archetypePanel3.transform.Find("Archetype Name");
        Transform archetype4 = archetypePanel4.transform.Find("Archetype Name");

        //Find the panel that holds removal
        //The panels are all from prefabs so the text box is just called "Archetype Name" for all of them
        Transform removalTransform = removalPanel.transform.Find("Archetype Name");
        Transform signatureTransform = signaturePanel.transform.Find("Archetype Name");

        //Set the text of four archetype headers
        archetype1.GetComponentInChildren<TextMeshProUGUI>().text = matchingArchetypes[0];
        archetype2.GetComponentInChildren<TextMeshProUGUI>().text = matchingArchetypes[1];
        archetype3.GetComponentInChildren<TextMeshProUGUI>().text = matchingArchetypes[2];
        archetype4.GetComponentInChildren<TextMeshProUGUI>().text = matchingArchetypes[3];
        
        //Find the panels that hold the card names
        Transform archetypeCardList1 = archetypePanel1.transform.Find("Archetype Card List");
        //Debug.Log("Card list 1: " + archetypeCardList1.name + " has " + archetypeCardList1.childCount + " children.");
        Transform archetypeCardList2 = archetypePanel2.transform.Find("Archetype Card List");
        Transform archetypeCardList3 = archetypePanel3.transform.Find("Archetype Card List");
        Transform archetypeCardList4 = archetypePanel4.transform.Find("Archetype Card List");
        Transform removalCardList = removalPanel.transform.Find("Archetype Card List");
        Transform signatureCardList = signaturePanel.transform.Find("Archetype Card List");

                //Clear existing cards from panels
        if(archetypeCardList1.transform.childCount > 0)
        {
            ClearStatPanels(archetypeCardList1);           
        }
        if(archetypeCardList2.transform.childCount > 0)
        {
            ClearStatPanels(archetypeCardList2);
        }
        if(archetypeCardList3.transform.childCount > 0)
        {
            ClearStatPanels(archetypeCardList3);
        }
        if(archetypeCardList4.transform.childCount > 0)
        {
            ClearStatPanels(archetypeCardList4);
        }
        if(removalCardList.transform.childCount > 0)
        {
            ClearStatPanels(removalCardList);
        }
        if(signatureCardList.transform.childCount > 0)
        {
            ClearStatPanels(signatureCardList);
        }



        //Create a list to store the CardData objects
        List<CardData> cardDatas = new List<CardData>();

        //Iterate through the creatures panel
        for (int i = 1; i < creaturesPanel.transform.childCount; i++)
        {
            Transform child = creaturesPanel.transform.GetChild(i); //Get the child at the current index
            CardData cardData = child.GetComponent<CardData>(); //Get that child's card data, steal it if you have to
            if(cardData == null)
            {
                Debug.LogError("CardData not found!");
            }
            cardDatas.Add(cardData);
        }    

        //Iterate through the noncreatures panel
        for (int i = 1; i < noncreaturesPanel.transform.childCount; i++)
        {
            Transform child = noncreaturesPanel.transform.GetChild(i); //Get the child at the current index
            CardData cardData = child.GetComponent<CardData>(); //Get that child's card data, steal it if you have to
            cardDatas.Add(cardData);
            if(cardData == null)
            {
                Debug.LogError("CardData not found!");
            }
        }



        //Instantiate a new TextMeshProGUI prefab for each CardData in cardDatas
        for (int i = 0; i < cardDatas.Count; i++)
        {
            GameObject newPanel;
            //First check to make sure the card has any archetypes (some cards are pure removal)
            if(cardDatas[i].archetypes.Count > 0)
            {
                //archetype1, 2, 3, 4 are the text boxes containing the archetype names
                //Instantiate cards by archetype, repeating if neccesary 
                if(cardDatas[i].archetypes.Contains(archetype1.GetComponentInChildren<TextMeshProUGUI>().text))
                {
                    newPanel = Instantiate(statTextPrefab, archetypeCardList1.transform);
                    newPanel.GetComponent<TextMeshProUGUI>().text = cardDatas[i].cardName;
                }
                if(cardDatas[i].archetypes.Contains(archetype2.GetComponentInChildren<TextMeshProUGUI>().text))
                {
                    newPanel = Instantiate(statTextPrefab, archetypeCardList2.transform);
                    newPanel.GetComponent<TextMeshProUGUI>().text = cardDatas[i].cardName;
                }
                if(cardDatas[i].archetypes.Contains(archetype3.GetComponentInChildren<TextMeshProUGUI>().text))
                {
                    newPanel = Instantiate(statTextPrefab, archetypeCardList3.transform);
                    newPanel.GetComponent<TextMeshProUGUI>().text = cardDatas[i].cardName;
                }
                if(cardDatas[i].archetypes.Contains(archetype4.GetComponentInChildren<TextMeshProUGUI>().text))
                {
                    newPanel = Instantiate(statTextPrefab, archetypeCardList4.transform);
                    newPanel.GetComponent<TextMeshProUGUI>().text = cardDatas[i].cardName;
                }
            }

            //Removal: check cardData isRemoval property
            if(cardDatas[i].isRemoval)
            {
                newPanel = Instantiate(statTextPrefab, removalCardList.transform);
                newPanel.GetComponent<TextMeshProUGUI>().text = cardDatas[i].cardName;
            }

            //Signature Cards: check for rare or mythic rarity
            if(cardDatas[i].rarity == CardRarity.Rare || cardDatas[i].rarity == CardRarity.Mythic)
            {
                newPanel = Instantiate(statTextPrefab, signatureCardList.transform);
                newPanel.GetComponent<TextMeshProUGUI>().text = cardDatas[i].cardName;
            }
        }

        //Update bar colors depending on card count
        //Access the PanelColorHandler of the panel that needs to be changed
        PanelColorHandler panelColorHandler1 = archetypePanel1.GetComponentInChildren<PanelColorHandler>();
        PanelColorHandler panelColorHandler2 = archetypePanel2.GetComponentInChildren<PanelColorHandler>();
        PanelColorHandler panelColorHandler3 = archetypePanel3.GetComponentInChildren<PanelColorHandler>();
        PanelColorHandler panelColorHandler4 = archetypePanel4.GetComponentInChildren<PanelColorHandler>();
        PanelColorHandler panelColorHandlerRemoval = removalPanel.GetComponentInChildren<PanelColorHandler>();
        PanelColorHandler panelColorHandlerSignature = signaturePanel.GetComponentInChildren<PanelColorHandler>();

        //Debug.Log("Card list 1: " + archetypeCardList1.name + " has " + archetypeCardList1.childCount + " children.");
        //Count the child objects of each archetypeCardList to determine the bar color
        switch(archetypeCardList1.transform.childCount)
        {
            case 0: panelColorHandler1.UpdateBarColors("emptyList");
            break;
            case 1: panelColorHandler1.UpdateBarColors("oneItemInList");
            break;
            default: panelColorHandler1.UpdateBarColors("fullList"); //2 or higher
            break;            
        }
        switch(archetypeCardList2.childCount)
        {
            case 0: panelColorHandler2.UpdateBarColors("emptyList");
            break;
            case 1: panelColorHandler2.UpdateBarColors("oneItemInList");
            break;
            default: panelColorHandler2.UpdateBarColors("fullList"); //2 or higher
            break;            
        }
        switch(archetypeCardList3.childCount)
        {
            case 0: panelColorHandler3.UpdateBarColors("emptyList");
            break;
            case 1: panelColorHandler3.UpdateBarColors("oneItemInList");
            break;
            default: panelColorHandler3.UpdateBarColors("fullList"); //2 or higher
            break;            
        }
        switch(archetypeCardList4.childCount)
        {
            case 0: panelColorHandler4.UpdateBarColors("emptyList");
            break;
            case 1: panelColorHandler4.UpdateBarColors("oneItemInList");
            break;
            default: panelColorHandler4.UpdateBarColors("fullList"); //2 or higher
            break;            
        }
        switch(removalCardList.childCount)
        {
            case 0: panelColorHandlerRemoval.UpdateBarColors("emptyList");
            break;
            case 1: panelColorHandlerRemoval.UpdateBarColors("oneItemInList");
            break;
            default: panelColorHandlerRemoval.UpdateBarColors("fullList");
            break;
        }
        switch(signatureCardList.childCount)
        {
            case 0: panelColorHandlerSignature.UpdateBarColors("emptyList");
            break;
            case 1: panelColorHandlerSignature.UpdateBarColors("oneItemInList");
            break;
            default: panelColorHandlerSignature.UpdateBarColors("fullList");
            break;
        }
    }

    private void SetArchetypeOptions()
    {
        deckArchetypeInput.ClearOptions();
        deckArchetypeInput.AddOptions(GetMatchingArchetypes(currentFilterColor));
    }

    private string GetSelectedArchetype()
    {
       string chosenArchetype = deckArchetypeInput.captionText.text;
       return chosenArchetype;
    }

    private void ClearStatPanels(Transform parent)
    {
        if(parent.childCount > 0)
        {
            for (int i = parent.childCount - 1; i >= 0; i--)
            // Iterate through children and destroy them, starting from the last child to avoid index issues.
            {
                DestroyImmediate(parent.GetChild(i).gameObject);
            }
            Debug.Log("Child count is now: " + parent.childCount );
        }
    }

    private void UpdateCardCountDisplay(int cardCount)
    {
        cardCountDisplayText.text = cardCount.ToString() + "/12"; //ex. "3/12"
    }
}
