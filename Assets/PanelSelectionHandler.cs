using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PanelSelectionHandler : MonoBehaviour
{
    public GameObject addButton;
    public GameObject deckPanelPrefab;
    public Transform deckListCreaturePanel, deckListNoncreaturePanel;
    public List<GameObject> panels = new List<GameObject>();
    private GameObject selectedPanel;
    public SaveHandler saveHandler;
    public DeckCreatorUI deckCreatorUI;
    private string chosenColor;
    private string cardName;
    
    void Start()
    {
        addButton.SetActive(false);
    }

    public void SelectPanel(GameObject panel)
    {
        selectedPanel = panel;
        addButton.SetActive(true);
        Debug.Log("Panel Selected: " + panel.name);
    }

    public void OnAddButtonClicked()
    {
        
        //List<string> ignoredCards = deckCreatorUI.ignoreList;
        Debug.Log(deckCreatorUI.ignoreList.Count + " ignored cards in list.");
        chosenColor = deckCreatorUI.GetStoredFilterColor();
        Debug.Log("chosenColor " + chosenColor);
        if(deckPanelPrefab != null)
        {

            Card card = saveHandler.LoadSingleCard(selectedPanel.name);
            GameObject newPanel = null;
            

            if(card.type_line.Contains("Creature"))
            {
                newPanel = Instantiate(deckPanelPrefab, deckListCreaturePanel);
            }
            else
            {
                newPanel = Instantiate(deckPanelPrefab, deckListNoncreaturePanel);
            }

            panels.Add(newPanel);

            newPanel.name = card.cardName;
            cardName = card.cardName;
            TextMeshProUGUI newCardName = newPanel.transform.Find("Card Name").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI newCardManaCost = newPanel.transform.Find("Card Cost").GetComponent<TextMeshProUGUI>();

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

                CardData panelCardData = newPanel.GetComponent<CardData>();
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

            if(card.colors.Count > 1)
            {
                Debug.Log("card.colors.Count = " + card.colors.Count.ToString());
                deckCreatorUI.SetCardPanelColor(newPanel.GetComponent<UnityEngine.UI.Image>(), "M");  
                card.isMulticolored = true;
            }
            else
            {
                Debug.Log("card.colors.Count = " + card.colors.Count.ToString() + " (SHOULD BE 1) and the color is " + card.colors[0]);
                chosenColor = card.colors[0];
                deckCreatorUI.SetCardPanelColor(newPanel.GetComponent<UnityEngine.UI.Image>(), chosenColor);
            }
        }
        else
        {
            Debug.LogError("Panel Prefab is null!");
        }

        if (selectedPanel != null)
        {
            Destroy(selectedPanel);
            selectedPanel = null;
        }

        if(cardName != null)
        {
            Debug.Log("Card name is : " + cardName);
            deckCreatorUI.ignoreList.Add(cardName);
            Debug.Log("Filter color: " + chosenColor);
            Debug.Log(deckCreatorUI.ignoreList);
            deckCreatorUI.LoadAndDisplayCards(chosenColor, deckCreatorUI.ignoreList);

                    foreach(string item in deckCreatorUI.ignoreList)
        {
            Debug.Log("item in ignore list: " + item);
        }
        }
        else
        {
            Debug.LogError("No card name!");
        }

    }
}
