    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection.Emit;
    using JetBrains.Annotations;
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
        public ArchetypeList archetypeListScript;

        void Start()
        {
            loadedCards = deckCreator.LoadAllCards();
            //Debug.Log("loadedCards[0]: " + loadedCards[0].cardName);
            //Debug.Log("loadedCards[0] normal image uri: " + loadedCards[0].image_Uris.normal);
        }

        public void OnDisplayButtonClick()
        {
            int randomIndex = Random.Range(0, loadedCards.Count);
            Card randomCard = loadedCards[randomIndex];

            if(archetypeListScript != null && archetypeListScript.archetypeColorPairs != null)
            {

            }
            else 
            {
                Debug.LogError("archetypeListScript or archetypeColorPairs is null!");
            }
            DisplayArchetypes(randomCard, archetypeListScript.archetypeColorPairs);
            
            StartCoroutine(DisplayCard(randomCard.image_Uris.normal));
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
                
                if(card.colors.Contains(pair.color1) || card.colors.Contains(pair.color2))
                {
                    Debug.Log("Pair name: " + pair.archetypeName);
                    Toggle newToggle = Instantiate(togglePrefab, archetypePanel.transform);
                    Text toggleText = newToggle.GetComponentInChildren<Text>();
                    toggleText.text = pair.archetypeName;
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
