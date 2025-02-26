using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using UnityEditor.Animations;

public class CardCreatorUI : MonoBehaviour
{

    public CardCreator cardCreator; //Code that actually creates the card objects
    public TMP_InputField searchInputField;
    public Button createButton;
    private List<Toggle> instantiatedToggles = new List<Toggle>();


    

    void Start()
    {
        //Create the Create button (heh)

        if(createButton != null)
        {
            createButton.onClick.AddListener(CreateCardButtonClicked);
        }
        else
        {
            Debug.Log("createButton is null1");
        }
    }

    void CreateCardButtonClicked()
    {  

        //Card newCard = cardCreator.CreateCard(cardName, selectedColors[], selectedRarity, manaCost, chosenArchetypes, isRemoval, isCreature);
        //cardCreator.WriteCardToFile(newCard);

        cardCreator.SearchScryfall(searchInputField.text);

        // Clear the card name input field
        searchInputField.text = "";
              
    }

    void Update()
    {
        
    }


}
