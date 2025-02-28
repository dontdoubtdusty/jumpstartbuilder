    using System.Collections;
    using System.Collections.Generic;
using System.IO;
using System.Reflection.Emit;
    using JetBrains.Annotations;
using TMPro;
using UnityEngine;
    using UnityEngine.Networking;
    using UnityEngine.UI;


    public class ArchetyperUI : MonoBehaviour
    {

        /*
        TO DO LIST:
        -----------
        Exclude mulitcolored cards from the list
        Assign chosen archetypes to displayed card
        */

        public DeckCreator deckCreator;
        public List<Card> loadedCards;
        public GameObject cardDisplayPanel, archetypePanel;
        public Button displayButton;
        public Toggle togglePrefab;
        public List<Toggle> togglesList;
        public ArchetypeList archetypeListScript;
        public TextMeshProUGUI archetypedCardsCounter;
        public TextAsset cardListSourceFile;
        private int currentCardIndex = -1;
        private List<string> userArchetypes = new List<string>();

        void Start()
        {
            loadedCards = deckCreator.LoadAllCards().cards;
            Debug.Log("Loaded cards: " + loadedCards);
        }

        public void OnDisplayButtonClick()
        {
            //Display completed cards counter
            int completedCards = 0;
            foreach (Card card in loadedCards)
            {
                if(card.archetypes.Count > 0)
                {
                    completedCards++;
                }
            }
            
            archetypedCardsCounter.text = completedCards.ToString() + "/" + loadedCards.Count; //ex (0/240)

            //Load random card from loadedCards list
            int randomIndex = Random.Range(0, loadedCards.Count);
            Debug.Log("loadedCards.Count: " + loadedCards.Count);
            Card randomCard = loadedCards[randomIndex];
            currentCardIndex = randomIndex;

            if(archetypeListScript != null && archetypeListScript.archetypeColorPairs != null)
            {

            }
            else 
            {
                Debug.LogError("archetypeListScript or archetypeColorPairs is null!");
            }
            if(randomCard.archetypes != null)
            {
                Debug.Log("Archetypes: " + randomCard.archetypes.Count + " " +  randomCard.archetypes);
                Debug.Log("Card list contains: " + loadedCards.Count);
            }
            else
            {
                Debug.Log("Archetypes list is null!");
            }

            DisplayArchetypes(randomCard, archetypeListScript.archetypeColorPairs);
            
            StartCoroutine(DisplayCard(randomCard.image_Uris.normal));
        }

        public void OnHitchButtonClicked()
        {
            Card selectedCard = loadedCards[currentCardIndex];
            Debug.Log("The card at index " + currentCardIndex + " is " + selectedCard.cardName);
            selectedCard.archetypes = userArchetypes;
            Debug.Log(selectedCard + " now has archetype(s): " + selectedCard.archetypes[0] + " and " + selectedCard.archetypes[1]);
        }

        public void OnArchetypeToggleClicked(Toggle toggle)
        {
            bool toggleOn = toggle.GetComponent<Toggle>().isOn;
            Text labelText = toggle.GetComponentInChildren<Text>();

            if(toggleOn)
            {
                userArchetypes.Add(labelText.ToString());
                Debug.Log(labelText.text + " added!");
                Debug.Log("userArchetypes contains: " + userArchetypes.Count);
                return;
            }
            
            userArchetypes.Remove(toggle.GetComponentInChildren<Text>().ToString());
            Debug.Log(labelText.text + " removed!");
            Debug.Log("userArchetypes contains: " + userArchetypes.Count);
        }
        public void DisplayArchetypes(Card card, List<ArchetypeList.ArchetypeColorPair> archetypeColorPairs)
        {
            //Kill the toggles already there, kill them dead
            foreach(Transform child in archetypePanel.transform)
            {
                Destroy(child.gameObject);
            }
            //Debug.Log("Card color: " + card.colors[0]);
            foreach (ArchetypeList.ArchetypeColorPair pair in archetypeColorPairs)
            {
                //Debug.Log("pair.color1: " + pair.color1 + " pair.color2 " + pair.color2);
                
                if(card.colors.Contains(pair.color1) || card.colors.Contains(pair.color2) || card.colors.Contains("C"))
                {
                    Debug.Log("Pair name: " + pair.archetypeName);
                    GameObject newTogglePrefab = Instantiate(togglePrefab.gameObject, archetypePanel.transform);
                    ArchetypeToggleHandler archetypeToggleHandler = newTogglePrefab.GetComponent<ArchetypeToggleHandler>();
                    Toggle newToggle = newTogglePrefab.GetComponent<Toggle>();  
                    Text toggleText = newToggle.GetComponentInChildren<Text>();
                    Debug.Log("Toggle text: " + toggleText);
                    toggleText.text = pair.archetypeName;

                    archetypeToggleHandler.SetArchetyperUI(this);
                }
            }
        }

        IEnumerator DisplayCard(string imageUrl)
        {
            Debug.Log("imageurl: " + imageUrl);
            using UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(imageUrl);
            {
                yield return webRequest.SendWebRequest();

                Debug.Log("imageUrl: " + imageUrl);

                if(webRequest.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log("Web request successful!");
                    Texture2D spriteTexture = DownloadHandlerTexture.GetContent(webRequest);
                    Sprite sprite = 
                        Sprite.Create(spriteTexture, new Rect(0, 0, spriteTexture.width, spriteTexture.height), new Vector2(0.5f, 0.5f));
                    Image imageComponent = cardDisplayPanel.GetComponent<Image>();
                    imageComponent.sprite = sprite;
                    imageComponent.preserveAspect = true;
                }
                else
                {
                    Debug.LogWarning("Web request failure: " + webRequest.result);
                }
            }
        }
    }
