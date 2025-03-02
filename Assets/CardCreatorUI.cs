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
    public TMP_InputField searchInputField, nameInputField;
    public Button createButton;
  
    void Start()
    {
        //Create the Create button (heh)

        if(createButton != null)
        {
            createButton.onClick.AddListener(ImportButtonClicked);
        }
        else
        {
            Debug.Log("createButton is null1");
        }
    }

    void ImportButtonClicked()
    {  

        //Card newCard = cardCreator.CreateCard(cardName, selectedColors[], selectedRarity, manaCost, chosenArchetypes, isRemoval, isCreature);
        //cardCreator.WriteCardToFile(newCard);

        cardCreator.SearchScryfall(searchInputField.text);

        // Clear the card name input field
        searchInputField.text = "";
        nameInputField.text = "";
              
    }

    void Update()
    {
        
    }


}
