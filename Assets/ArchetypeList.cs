using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ArchetypeList", menuName = "Card Game/Archetype List")]
public class ArchetypeList : ScriptableObject
{
    //public List<string> Archetypes;
    [System.Serializable]
    //The class that contains the name and colors for each archetype
    public class ArchetypeColorPair
    {
        public string archetypeName;
        public string color1;
        public string color2;
    }

    //The list containing all of the archetypes
    public List<ArchetypeColorPair> archetypeColorPairs;
    //Method to get and return all the archetypes from the list
    public List<string> GetArchetypeColorPair(string archetypeName)
    {
        foreach (var pair in archetypeColorPairs)
        {
            if(pair.archetypeName == archetypeName)
            {
                return new List<string>{pair.color1, pair.color2};
            }
        }
        return null;
    }
}