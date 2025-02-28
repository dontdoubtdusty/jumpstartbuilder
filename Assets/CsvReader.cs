using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CSVCardReader : MonoBehaviour
{
    public TextAsset csvFile; // Drag your CSV file here in the Unity Editor
    public string outputFilePath = Path.Combine(Application.dataPath, "ArchetypeCardLists.txt"); // Path to the output text file


    public Dictionary<string, List<string>> archetypeCardLists = new Dictionary<string, List<string>>();

    void Start()
    {
        if (csvFile != null)
        {
            ParseCSV(csvFile.text);
            //PrintArchetypeCardLists(); // Optional: Print the results to the console
            WriteArchetypeCardListsToFile();
        }
        else
        {
            Debug.LogError("CSV file not assigned!");
        }
    }

    void ParseCSV(string csvText)
    {
        string[] lines = csvText.Split('\n');
        List<string> processedLines = new List<string>();

        // Combine lines until a line ends with a quote
        for (int i = 0; i < lines.Length; i++)
        {
            string currentLine = lines[i].Trim();
            while (i + 1 < lines.Length && !currentLine.EndsWith("\""))
            {
                i++;
                currentLine += lines[i].Trim();
            }
            // Add this line to remove the trailing quote.
            if(currentLine.EndsWith("\""))
            {
                currentLine = currentLine.Substring(0, currentLine.Length-1);
            }
            processedLines.Add(currentLine);
        }

        foreach (string line in processedLines)
        {
            //Debug.Log("line: " + line);
            if (string.IsNullOrEmpty(line)) continue; // Skip empty lines

            string cleanedLine = line.Trim(); // Remove leading/trailing whitespace
            if (!cleanedLine.StartsWith("\"(")) continue; //skip lines that do not start with an archetype.

            int archetypeEndIndex = cleanedLine.IndexOf(')');
            if (archetypeEndIndex == -1) continue; // Skip lines without a closing parenthesis

            string archetypeName = cleanedLine.Substring(2, archetypeEndIndex - 2); // Extract archetype name
            string cardListString = cleanedLine.Substring(archetypeEndIndex + 1); // Extract card list

            string[] cards = cardListString.Split('/');
            List<string> cardList = new List<string>();

            foreach (string card in cards)
            {
                string trimmedCard = card.Trim();
                if (!string.IsNullOrEmpty(trimmedCard))
                {
                    cardList.Add(trimmedCard);
                }
            }

            archetypeCardLists[archetypeName] = cardList;
        }
    }

    void PrintArchetypeCardLists()
    {
        foreach (var archetype in archetypeCardLists)
        {
            Debug.Log("Archetype: " + archetype.Key);
            foreach (var card in archetype.Value)
            {
                Debug.Log("- " + card);
            }
        }
    }

    void WriteArchetypeCardListsToFile()
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(outputFilePath))
            {
                foreach (var archetype in archetypeCardLists)
                {
                    writer.WriteLine("Archetype: " + archetype.Key);
                    foreach (var card in archetype.Value)
                    {
                        writer.WriteLine("- " + card);
                    }
                }
            }
            Debug.Log("Archetype card lists written to " + outputFilePath);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error writing to file: " + ex.Message);
        }
    }
}