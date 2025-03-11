using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PanelSelectionHandler : MonoBehaviour
{
    public GameObject addButton, removeButton;
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
        removeButton.SetActive(false);
    }

    public void SelectPanel(GameObject panel)
    {
        selectedPanel = panel;

        //Determine which panel the selected panel is in
        if(selectedPanel.transform.parent.name == "Viewport Card List")
        {
            addButton.SetActive(true);
        }
        else
        {
            removeButton.SetActive(true);
        }
        
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

            //Add click listener
            newPanel.GetComponent<Button>().onClick.AddListener(() => SelectPanel(newPanel));

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

            /*          
            Multicolored cards, currently not necessary as we exclude them from the import

            if(card.colors.Count > 1)
            {
                Debug.Log("card.colors.Count = " + card.colors.Count.ToString());
                deckCreatorUI.SetCardPanelColor(newPanel.GetComponent<UnityEngine.UI.Image>(), "M");  
                card.isMulticolored = true;
            }  
            */

            if(card.colors[0] != "C")
            {
                deckCreatorUI.SetCardPanelColor(newPanel.GetComponent<UnityEngine.UI.Image>(), chosenColor);
            }
            else
            {
                deckCreatorUI.SetCardPanelColor(newPanel.GetComponent<UnityEngine.UI.Image>(), "C");
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

        //Update Creatures/Noncreatures headers
        deckCreatorUI.UpdateCategoryCounts();
        deckCreatorUI.UpdateArchetypePanels();  

        //Hide the Add button
        addButton.SetActive(false);

    }

    public void OnRemoveButtonClick()
    {
        StartCoroutine(RemoveCardAndUpdateUI());
    }

    public IEnumerator RemoveCardAndUpdateUI()
    {
        if (selectedPanel != null)
        {
            Destroy(selectedPanel);
            selectedPanel = null;
        }

        if(cardName != null)
        {
            Debug.Log("Card name is : " + cardName);
            deckCreatorUI.ignoreList.Remove(cardName);
            Debug.Log("Filter color: " + chosenColor);
            Debug.Log(deckCreatorUI.ignoreList);
            deckCreatorUI.LoadAndDisplayCards(chosenColor, deckCreatorUI.ignoreList);
            //Update Creatures/Noncreatures headers
            deckCreatorUI.UpdateCategoryCounts();
            yield return null;
            deckCreatorUI.UpdateArchetypePanels();
            yield return null;
        }
        else
        {
            Debug.LogError("No card name!");
        }


        removeButton.SetActive(false);
    }
}
