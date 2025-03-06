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
    public SaveHandler saveHandler;
    public GameObject cardPanelPrefab; //The panels showing card name and mana cost on the right side
    public Transform contentPanel; //The ScrollView's content panel, containing the instantiated cards
    // Start is called before the first frame update
    public GameObject archetypeHeaderPanelPrefab;
    public PanelSelectionHandler panelSelectionHandler;
    public string currentFilterColor;
    public List<string> ignoreList;
    public Toggle multicolorToggle, colorlessToggle;
    public CardData cardData;
    public GameObject creaturesPanel, noncreaturesPanel;
    public GameObject thrivingLandPanel, thrivingGatePanel;
    public GameObject archetypePanel1, archetypePanel2, archetypePanel3, archetypePanel4;
    public GameObject rarePanel, removalPanel;
    public ArchetypeList archetypeList;
    public GameObject statTextPrefab;

    [System.Serializable]
    public struct ColorMapping
    {
        public string colorName;
        public Color colorValue;
    }

    public List<ColorMapping> colorMapping;

    public void Start()
    {
        //Load full list of cards to start with
        currentFilterColor = "INIT";
        ignoreList = new List<string>();
    }

    public void OnColorButtonClick(GameObject button)
    {
        ColorButtonData colorData = button.GetComponent<ColorButtonData>();
        if (colorData != null)
        {   
            currentFilterColor = colorData.color;
            Debug.Log("colorData.color test: " + colorData.color);
            Debug.Log("currentFilterColor = " + currentFilterColor);
            if(ignoreList != null)
            {
             Debug.Log("Ignore list found! Containing: " + ignoreList.Count);
            }
            LoadAndDisplayCards(currentFilterColor, ignoreList);
            UpdateLandPanels();
            UpdateArchetypePanels();
        }
        else
        {
            Debug.LogError("ColorButtonData component not found on button!");
        }
    }

    public string GetStoredFilterColor()
    {
        Debug.Log("currentFilterColor X7: " + currentFilterColor);
        return currentFilterColor;
    }

    public void DisplayArchetypeHeaders()
    {
        List<string> matchingArchetypes = new List<string>();
        foreach(ArchetypeList.ArchetypeColorPair archetypeColorPair in archetypeList.archetypeColorPairs)
        {
            if(archetypeColorPair.color1 == currentFilterColor || archetypeColorPair.color2 == currentFilterColor)
            {
                matchingArchetypes.Add(archetypeColorPair.archetypeName);
                Debug.Log("Archetype name: " + archetypeColorPair.archetypeName);
            }
        }

        foreach(string archetype in matchingArchetypes)
        {
            GameObject newArchetypePanel = Instantiate(cardPanelPrefab, contentPanel);
            TextMeshProUGUI newPanelName = newArchetypePanel.transform.Find("Card Name").GetComponent<TextMeshProUGUI>();
            newPanelName.text = archetype;
        }
    }

    public void DisplayCardsInScrollview(string filterColor, List<Card> cards, List<string> ignoredCardsList)
    {
        if(cards != null && filterColor != null) //If there are cards and a filter is selected
        {
            List<Card> filteredCards = new List<Card>();

            //Check each card in the cards list for matching color
            foreach (Card card in cards)
            {
                if(!ignoredCardsList.Contains(card.cardName))
                {
                    if(card.colors.Contains(filterColor) || card.colors.Contains("C"))
                    { 
                            filteredCards.Add(card);
                    }
                    else if (filterColor == "C")
                    {
                        filteredCards.Add(card);
                    }  
                }
       
            }

            // Sort filteredCards by mana cost using a bubble sort. It just works idk.

            filteredCards.Sort((a, b) => a.manaCost.CompareTo(b.manaCost));

            foreach (Card card in filteredCards)
            {
                //Substantiate prefab Card Panel
                GameObject newCardPanel = Instantiate(cardPanelPrefab, contentPanel);

                //Add click listener
                newCardPanel.GetComponent<Button>().onClick.AddListener(() => panelSelectionHandler.SelectPanel(newCardPanel));
                newCardPanel.name = card.cardName;

                if(card.colors.Count > 1)
                {
                    //Debug.Log("card.colors.Count = " + card.colors.Count.ToString());
                    SetCardPanelColor(newCardPanel.GetComponent<UnityEngine.UI.Image>(), "M");  
                }
                else
                {
                    SetCardPanelColor(newCardPanel.GetComponent<UnityEngine.UI.Image>(), card.colors[0]);
                }

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
                panelCardData.archetypes = card.archetypes;
                panelCardData.isRemoval = card.isRemoval;
                panelCardData.isCreature = card.isCreature;
                panelCardData.isMulticolored = card.isMulticolored;
                panelCardData.type_line = card.type_line;
                panelCardData.image_Uris = card.image_Uris;
            }
        }   
        else
        {
            Debug.LogError("Cards could not be loaded. :(");
        }
    }

    public void LoadAndDisplayCards(string filterColor, List<string> ignoredCardsList)
    /*
        DisplayCardsInScrollview needs to run for each archetype header
    */
    {
        //Kill the cards already there, kill them dead
        foreach(Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }
        List<Card> cards = saveHandler.LoadAllCards().cards;

        DisplayArchetypeHeaders();
        DisplayCardsInScrollview(filterColor, cards, ignoredCardsList);
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

    /* 
    Currently multicolored cards are not imported so this method is unused
    
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
    } 
    */

    public void ColorlessToggle()
    {
        bool toggleOn = colorlessToggle.GetComponent<Toggle>().isOn;
        if (!toggleOn) //If off, remove all multicolored cards from the list
        {
            foreach(Card card in saveHandler.LoadAllCards().cards)
            {
                if (card.colors.Contains("C"))
                {
                    ignoreList.Add(card.cardName);
                    LoadAndDisplayCards(currentFilterColor, ignoreList);
                }
            }
            return;
        }
        
        foreach(Card card in saveHandler.LoadAllCards().cards)
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
        //Set the text of the Card Count text box to the cardCount
        UpdateCardCountText(creaturesPanel.transform, noncreaturesPanel.transform, creatureCardsCount, noncreatureCardsCount);
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

    public void UpdateArchetypePanels()
    {
        //Create a list to store the correct archetypes
        List<string> matchingArchetypes = new List<string>();
        //Extract the correct four archetypes
        foreach(ArchetypeList.ArchetypeColorPair archetypeColorPair in archetypeList.archetypeColorPairs)
        {
            if(archetypeColorPair.color1 == currentFilterColor || archetypeColorPair.color2 == currentFilterColor)
            {
                matchingArchetypes.Add(archetypeColorPair.archetypeName);
                Debug.Log("Archetype name: " + archetypeColorPair.archetypeName);
            }
        }

        //Find the panels that hold the archetype names
        Transform archetype1 = archetypePanel1.transform.Find("Archetype Name");
        Transform archetype2 = archetypePanel2.transform.Find("Archetype Name");
        Transform archetype3 = archetypePanel3.transform.Find("Archetype Name");
        Transform archetype4 = archetypePanel4.transform.Find("Archetype Name");

        //Set the text boxes to the correct archetypes
        archetype1.GetComponentInChildren<TextMeshProUGUI>().text = matchingArchetypes[0];
        archetype2.GetComponentInChildren<TextMeshProUGUI>().text = matchingArchetypes[1];
        archetype3.GetComponentInChildren<TextMeshProUGUI>().text = matchingArchetypes[2];
        archetype4.GetComponentInChildren<TextMeshProUGUI>().text = matchingArchetypes[3];
        
        //Find the panels that hold the card names
        Transform archetypeCardList1 = archetypePanel1.transform.Find("Archetype Card List");
        Transform archetypeCardList2 = archetypePanel2.transform.Find("Archetype Card List");
        Transform archetypeCardList3 = archetypePanel3.transform.Find("Archetype Card List");
        Transform archetypeCardList4 = archetypePanel4.transform.Find("Archetype Card List");

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
            Debug.Log("i = " +  i);
            Transform child = noncreaturesPanel.transform.GetChild(i); //Get the child at the current index
            Debug.Log("Child's name is: " + child.name);
            CardData cardData = child.GetComponent<CardData>(); //Get that child's card data, steal it if you have to
            cardDatas.Add(cardData);
            Debug.Log("hey you're here");
            Debug.Log("this card's name is: " + cardData.cardName);
            if(cardData == null)
            {
                Debug.LogError("CardData not found!");
            }
        }

        //Check for and clear existing panels
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

        //Instantiate a new TextMeshProGUI prefab for each CardData in cardDatas
        for (int i = 0; i < cardDatas.Count; i++)
        {
            GameObject newPanel;
            //archetype1, 2, 3, 4 are the text boxes containing the archetype names
            //Instantiate cards by archetype, repeating if neccesary 
            if(cardDatas[i].archetypes.Contains(archetype1.GetComponentInChildren<TextMeshProUGUI>().text))
            {
                newPanel = Instantiate(statTextPrefab, archetypeCardList1.transform);
                newPanel.GetComponent<TextMeshProUGUI>().text = cardDatas[i].name;
            }
            if(cardDatas[i].archetypes.Contains(archetype2.GetComponentInChildren<TextMeshProUGUI>().text))
            {
                newPanel = Instantiate(statTextPrefab, archetypeCardList2.transform);
                newPanel.GetComponent<TextMeshProUGUI>().text = cardDatas[i].name;
            }
            if(cardDatas[i].archetypes.Contains(archetype3.GetComponentInChildren<TextMeshProUGUI>().text))
            {
                newPanel = Instantiate(statTextPrefab, archetypeCardList3.transform);
                newPanel.GetComponent<TextMeshProUGUI>().text = cardDatas[i].name;
            }
            if(cardDatas[i].archetypes.Contains(archetype4.GetComponentInChildren<TextMeshProUGUI>().text))
            {
                newPanel = Instantiate(statTextPrefab, archetypeCardList4.transform);
                newPanel.GetComponent<TextMeshProUGUI>().text = cardDatas[i].name;
            }
            Debug.Log("There are " + cardDatas.Count + " cards.");
        }

        //Update bar colors depending on card count
        //Access the PanelColorHandler of the panel that needs to be changed
        PanelColorHandler panelColorHandler = archetypePanel1.GetComponentInChildren<PanelColorHandler>();

        /*
            IMPORTANT:      NEED TO COMPLETE THIS FOR THE OTHER THREE PANELS TO PROCEED
        */
        switch(archetypeCardList1.childCount)
        {
            case 0: panelColorHandler.UpdateBarColors("emptyList");
            break;
            case 1: panelColorHandler.UpdateBarColors("oneItemInList");
            break;
            default: panelColorHandler.UpdateBarColors("fullList"); //2 or higher
            break;            
        }
    }

    private void ClearStatPanels(Transform parent)
    {
        if(parent.childCount > 0)
        {
            for (int i = parent.childCount - 1; i >= 0; i--)
            // Iterate through children and destroy them, starting from the last child to avoid index issues.
            {
                Destroy(parent.GetChild(i).gameObject);
            }
        }
    }
}
