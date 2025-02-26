using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    public string cardName; // You might want a deck name
    public List<Card> Cards;

    public Deck(string name)
    {
        cardName = name;
        Cards = new List<Card>();
    }

    public void AddCard(Card card)
    {
        Cards.Add(card);
    }
}
