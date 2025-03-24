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
    public TMP_InputField searchInputField, saveNameInputField;
    public Button importAndSaveButton; //Button that imports and saves. What a twist!
  
    void Start()
    {
        if(importAndSaveButton != null)
        {
            importAndSaveButton.onClick.AddListener(ImportAndSaveButtonClicked);
        }
        else
        {
            Debug.Log("createButton is null!");
        }
    }

    void ImportAndSaveButtonClicked()
    {  
         StartCoroutine(ImportAndSaveCoroutine());
    }

    IEnumerator ImportAndSaveCoroutine()
    {
        cardCreator.SearchScryfall(searchInputField.text);

        //Wait for the search coroutines to finish
        while(cardCreator.isSearching)
        {
            yield return null;
        }
        Debug.Log("Save Name: " + saveNameInputField.text);
        SaveHandler saveHandler = SaveHandler.instance;
        if(saveHandler != null)
        {
            saveHandler.UpdateWrapperAndSaveGame(saveNameInputField.text);
        }
        else
        {
            Debug.LogError("SaveHandler instance not found!");
        }
        

        // Clear the card name input field
        searchInputField.text = "";     
    }
}
