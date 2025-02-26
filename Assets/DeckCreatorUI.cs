using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/*

KNOWN ISSUES:
--------------

Clicking on ColorButton resets list, ignoring list of ignored cards
Clicking on AddButton for multicolored card ends the filter loop and panels aren't instantiated properly

FUTURE GOALS:
-------------

Enable mulitcolored check box
Enable colorless check box

*/
public class DeckCreatorUI : MonoBehaviour
{

    public DeckCreator deckCreator;
    public GameObject cardPanelPrefab; //The panels showing card name and mana cost on the right side
    public Transform contentPanel; //The ScrollView's content panel, containing the instantiated cards
    // Start is called before the first frame update
    public GameObject archetypeHeaderPanelPrefab;
    public PanelSelectionHandler panelSelectionHandler;
    public string currentFilterColor;
    public List<string> ignoreList;
    public Toggle multicolorToggle, colorlessToggle;
    public CardData cardData;

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
        LoadAndDisplayUnfilteredCards();

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

    void LoadAndDisplayUnfilteredCards()
    {
        //Kill the cards already there, kill them dead
        foreach(Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }
        
        List<Card> cards = deckCreator.LoadAllCards();

        foreach (Card card in cards)
        {
            //Substantiate prefab Card Panel
            GameObject newCardPanel = Instantiate(cardPanelPrefab, contentPanel);


            if(card.colors.Count > 1)
            {
                //Debug.Log("card.colors.Count = " + card.colors.Count.ToString());
                SetCardPanelColor(newCardPanel.GetComponent<UnityEngine.UI.Image>(), "M");  
            }
            else
            {
                SetCardPanelColor(newCardPanel.GetComponent<UnityEngine.UI.Image>(), card.colors[0]);
                //Debug.Log("card.colors[0]: " + card.colors[0]);
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
        }
        
    }
    public void LoadAndDisplayCards(string filterColor, List<string> ignoredCardsList)
    {
        //Kill the cards already there, kill them dead
        foreach(Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }
        List<Card> cards = deckCreator.LoadAllCards();

        Debug.Log(ignoredCardsList.Count + " items in ignoredCardsList.");
        
        if(cards != null && filterColor != null) //If there are cards and a filter is selected
        {
            List<Card> filteredCards = new List<Card>();

            //Check each card in the cards list for matching color
            foreach (Card card in cards)
            {
                if(!ignoredCardsList.Contains(card.cardName))
                {
                    if(card.colors.Count > 0)
                    {
                        card.isMulticolored = true;
                        if(card.colors.Contains(filterColor) || card.colors.Contains("C"))
                        { 
                            filteredCards.Add(card);
                        }
                    }
                    else if (filterColor == "C")
                    {
                        filteredCards.Add(card);
                    } 
                }
       
            }

                    // Sort filteredCards by cardName using a bubble sort. It just works idk.

            filteredCards.Sort((a, b) => a.cardName.CompareTo(b.cardName));

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
                /*panelCardData.imageUris.small = card.imageUris.small;
                panelCardData.imageUris.large = card.imageUris.large;
                panelCardData.imageUris.png = card.imageUris.png;
                panelCardData.imageUris.art_crop = card.imageUris.art_crop;
                panelCardData.imageUris.border_crop = card.imageUris.border_crop;*/
            }

            List<string> archetypeNames = new List<string>();
            // Get a list of unique archetypes found in the filtered cards.
            foreach (Card card in filteredCards)
            {
                foreach (string archetype in card.archetypes)
                {
                    if(!archetypeNames.Contains(archetype)) //Check to make sure the uniqueArchetypes list does not already contain the archetype
                    {
                        archetypeNames.Add(archetype); //If it does not, it'll be added to the list
                    }
                }
            }
   
        }   
        else
        {
            Debug.LogError("Cards could not be loaded. :(");
        }
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

    public void MulticolorToggle()
    {
        bool toggleOn = multicolorToggle.GetComponent<Toggle>().isOn;
        if (!toggleOn) //If off, remove all multicolored cards from the list
        {
            foreach(Card card in deckCreator.LoadAllCards())
            {
                if (card.colors.Contains("M"))
                {
                    ignoreList.Add(card.cardName);
                    LoadAndDisplayCards(currentFilterColor, ignoreList);
                }
            }
            return;
        }
        
        foreach(Card card in deckCreator.LoadAllCards())
        {
            if (card.colors.Contains("M"))
            {
                ignoreList.Remove(card.cardName);
            }
        }

        LoadAndDisplayCards(currentFilterColor, ignoreList);
        return;
    }

        public void ColorlessToggle()
    {
        bool toggleOn = colorlessToggle.GetComponent<Toggle>().isOn;
        if (!toggleOn) //If off, remove all multicolored cards from the list
        {
            foreach(Card card in deckCreator.LoadAllCards())
            {
                if (card.colors.Contains("C"))
                {
                    ignoreList.Add(card.cardName);
                    LoadAndDisplayCards(currentFilterColor, ignoreList);
                }
            }
            return;
        }
        
        foreach(Card card in deckCreator.LoadAllCards())
        {
            if (card.colors.Contains("C"))
            {
                ignoreList.Remove(card.cardName);
            }
        }

        LoadAndDisplayCards(currentFilterColor, ignoreList);
        return;
    }
}
