using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ArchetypeList", menuName = "Card Game/Archetype List")]
public class ArchetypeList : ScriptableObject
{
    //public List<string> Archetypes;
    [System.Serializable]
    public class ArchetypeColorPair
    {
        public string archetypeName;
        public string color1;
        public string color2;
    }

    public List<ArchetypeColorPair> archetypeColorPairs;
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