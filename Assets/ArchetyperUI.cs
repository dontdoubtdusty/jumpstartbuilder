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
        Assign chosen archetypes to displayed card
        Save card after/as archetypes are assigned
        Create a Dictionary in a separate file:
            Dictionary<string, List<string>>
            string = the card name and List<string> = the associated archetypes
            Backup in case we change anything later so we don't have to re-archetype


        Add scoring/yes no maybe system to archetype choices
        */

        public SaveHandler saveHandler;
        public CardCreator cardCreator;
        public List<Card> loadedCards;
        public GameObject cardDisplayPanel, archetypePanel;
        public Button displayButton;
        public Toggle togglePrefab;
        public List<Toggle> togglesList;
        public ArchetypeList archetypeListScript;
        public TextMeshProUGUI archetypedCardsCounter;
        public TextAsset cardListSourceFile;
        private List<string> userArchetypes = new List<string>();
        private List<Card> untaggedCards = new List<Card>();
        int completedCards;
        string filePath;
        private System.DateTime lastWriteTime;
        int loadedCardsLength;

        void Start()
        {
            loadedCardsLength = saveHandler.LoadAllCards().cards.Count;
            filePath = Path.Combine(Application.dataPath, "CardData.json");
            lastWriteTime = File.GetLastWriteTime(filePath);
            DisplayNextCard();
            UpdateArchetypeCounter();
        }

        void Update()
        {
            if(File.GetLastWriteTime(filePath) != lastWriteTime)
            {
                lastWriteTime = File.GetLastWriteTime(filePath);
                Debug.Log("File changed, updating.");
                saveHandler.LoadAllCards();
                DisplayNextCard();
                UpdateArchetypeCounter();
            }
        }

        public void DisplayNextCard()
        {
            SaveHandler.ListOfCards cards = saveHandler.LoadAllCards();

            // Check if the card data was loaded successfully.
            if (cards == null || cards.cards == null)
            {
                Debug.LogError("Error loading cards from save handler");
                return; // Exit if there was an error.
            }

            //Clear previous list of cards
            untaggedCards.Clear();

            //Retrieve list of cards with no archetypes
            foreach (Card card in cards.cards)
            {
                if (card.archetypes.Count < 1)
                {
                    untaggedCards.Add(card);
                }
            }

            if(untaggedCards.Count == 0)
            {
                Debug.Log("No untagged cards found!");
                return;
            }

            userArchetypes.Clear();

            // Start the coroutine and wait for it to finish.
            StartCoroutine(WaitForDisplayCard(untaggedCards[0].image_Uris.normal, untaggedCards[0]));
        }

        IEnumerator WaitForDisplayCard(string imageUrl, Card card)
        {
            yield return StartCoroutine(DisplayCard(imageUrl)); // Wait for DisplayCard to finish.

            UpdateArchetypeCounter();
            DisplayArchetypes(card, archetypeListScript.archetypeColorPairs);
        }
        public void OnHitchButtonClicked()
        {
            Card selectedCard = untaggedCards[0];
            selectedCard.archetypes = userArchetypes;
            saveHandler.UpdateCard(selectedCard); 
            loadedCards = saveHandler.LoadAllCards().cards;
            DisplayNextCard();
            ResetToggles();
            //Debug.Log(selectedCard + " now has archetype(s): " + selectedCard.archetypes[0] + " and " + selectedCard.archetypes[1]);
        }

        public void OnArchetypeToggleClicked(Toggle toggle)
        {
            bool toggleOn = toggle.GetComponent<Toggle>().isOn;
            Text labelText = toggle.GetComponentInChildren<Text>();

            if(toggleOn)
            {
                userArchetypes.Add(labelText.text.ToString());
                Debug.Log(labelText.text + " added!");
                Debug.Log("userArchetypes contains: " + userArchetypes.Count);
                Debug.Log(userArchetypes[0]);
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

            //Clear previous toggles list
            togglesList.Clear();
            //Debug.Log("Card color: " + card.colors[0]);

            foreach (ArchetypeList.ArchetypeColorPair pair in archetypeColorPairs)
            {
                //Debug.Log("pair.color1: " + pair.color1 + " pair.color2 " + pair.color2);
                
                if(card.colors.Contains(pair.color1) || card.colors.Contains(pair.color2) || card.colors.Contains("C"))
                {
                    //Debug.Log("Pair name: " + pair.archetypeName);
                    GameObject newTogglePrefab = Instantiate(togglePrefab.gameObject, archetypePanel.transform);
                    ArchetypeToggleHandler archetypeToggleHandler = newTogglePrefab.GetComponent<ArchetypeToggleHandler>();
                    Toggle newToggle = newTogglePrefab.GetComponent<Toggle>();  
                    Text toggleText = newToggle.GetComponentInChildren<Text>();
                    //Debug.Log("Toggle text: " + toggleText);
                    toggleText.text = pair.archetypeName;
                    togglesList.Add(newToggle);

                    archetypeToggleHandler.SetArchetyperUI(this);
                }
            }
        }

        IEnumerator DisplayCard(string imageUrl)
        {
            //Debug.Log("imageurl: " + imageUrl);
            using UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(imageUrl);
            {
                yield return webRequest.SendWebRequest();

                //Debug.Log("imageUrl: " + imageUrl);

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

        public void UpdateArchetypeCounter()
        {
            SaveHandler.ListOfCards cards = saveHandler.LoadAllCards();

            // Check for null values.
            if (cards == null || cards.cards == null)
            {
                return;
            }

            completedCards = 0;

            foreach (Card card in cards.cards)
            {
                if(card.archetypes.Count > 0)
                {
                    completedCards++;
                    //Debug.Log(completedCards);
                }
            }

            Debug.Log("There are " + completedCards + " completed cards.");
            archetypedCardsCounter.text = "Completed: " + completedCards.ToString() + "/" + loadedCardsLength; //ex (0/240)
        }

        public void ResetToggles()
        {
            foreach(Toggle toggle in togglesList)
            {
                toggle.isOn = false;
            }
        }
    }
