using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeckCollectionUI : MonoBehaviour
{
    //This list should at some point take from the user save instead of a public ArchetypesList
    SaveHandler saveHandler = SaveHandler.instance;
    public GameObject packParentPanel;
    List<GameObject> packPanels = new List<GameObject>();
    List<GameObject> packTitlePanels = new List<GameObject>();
    List<GameObject> manaPanels = new List<GameObject>();
    public ArchetypeList savedArchetypes;
    public GameObject manaChoiceParentPanel;
    private List<GameObject> manaChoicePanels = new List<GameObject>();
    List<ArchetypeList.ArchetypeColorPair> matchingArchetypes = new List<ArchetypeList.ArchetypeColorPair>();
    string testFilter = "R";
    private string currentFilterColor = "W";
    private string packColor;
    void Awake()
    {
        GetAndSetSavedArchetypes();
        GetManaChoicePanels();
        AddButtonListeners();
        Debug.Log("There are " + manaChoicePanels.Count + " mana choices.");
    }
    // Start is called before the first frame update
    void Start()
    {
        currentFilterColor = "W";
    }

    public void OnColorButtonClick(string color)
    {
        currentFilterColor = color;
        Debug.Log("currentFilterColor: " + currentFilterColor);
        SetMatchingArchetypes();
        GetPackPanels(packParentPanel);
        SetPackPanels(packPanels);

    }

    private void GetManaChoicePanels()
    {
        for(int i = 0; i < manaChoiceParentPanel.transform.childCount; i++)
        {
            manaChoicePanels.Add(manaChoiceParentPanel.transform.GetChild(i).gameObject);
        }
    }

    private void GetAndSetSavedArchetypes()
    {
        savedArchetypes = saveHandler.archetypeList;
    }
    private void SetMatchingArchetypes()
    {
        Debug.Log("saveHandler: " + saveHandler);
        Debug.Log("Archetypes in save: " + saveHandler.archetypeList.archetypeColorPairs.Count);
        foreach(ArchetypeList.ArchetypeColorPair archetype in saveHandler.archetypeList.archetypeColorPairs)
        {
            if(archetype.color1 == currentFilterColor || archetype.color2 == currentFilterColor)
            {
                matchingArchetypes.Add(archetype);
            }
        }
    }

    private void GetPackPanels(GameObject packParentPanel)
    {
        for(int i = 0; i < packParentPanel.transform.childCount; i++)
        {
            //The panel containing the pack header and contents
            GameObject packPanel = packParentPanel.transform.GetChild(i).gameObject;
            packPanels.Add(packPanel);
            Debug.Log("There are " + packPanels.Count + " panels.");
        }
    }

    private void SetPackPanels(List<GameObject> packPanels)
    {
        Debug.Log("packPanels count: " + packPanels.Count);
        for(int i = 0; i < packPanels.Count; i++)
        {
            Debug.Log("i is " + i + " and the Pack Panel at i is " + packPanels[i].name);
            SetPackHeader(packPanels[i], i);
            SetPackContents(packPanels[i]);
        }
    }
    private void SetPackHeader(GameObject packPanel, int panelIndex)
    {
        //Get the header panel part of the pack panel
        Debug.Log("packPanel: " + packPanel + " and panelIndex: " +  panelIndex);
        GameObject headerPanel = packPanel.transform.Find("Pack Header Panel").gameObject;
        Debug.Log("Header panel: " + headerPanel);

        //Set the pack name
        SetPackName(headerPanel, panelIndex);

        //Set the pack color
        SetPackColor(headerPanel, panelIndex);

        //Activate the applicable buttons to switch between decks
    }

    private void SetPackName(GameObject packHeaderPanel, int panelIndex)
    {
        //The panel containing the pack name
        GameObject namePanel = packHeaderPanel.transform.Find("Pack Name Panel").gameObject;
            Debug.Log("Name panel: " + namePanel);
            Debug.Log("namePanel text box:" + namePanel.GetComponentInChildren<TextMeshProUGUI>());
            Debug.Log(matchingArchetypes + " contains " + matchingArchetypes.Count);
            Debug.Log("Archetype name: " + matchingArchetypes[panelIndex]);
        namePanel.GetComponentInChildren<TextMeshProUGUI>().text = matchingArchetypes[panelIndex].archetypeName;
    }

    private void SetPackContents(GameObject packPanel)
    {
        //The panel containing the pack contents
        GameObject contentsPanel = packPanel.transform.Find("Pack Contents Panel").gameObject;
    }

    private void SetPackColor(GameObject packHeaderPanel, int panelIndex)
    {

            //Pick whichever color is not the filter color in the archetype
            //This will be the color of the pack (packColor)
            if(matchingArchetypes[panelIndex].color1 != currentFilterColor)
            {
                packColor = matchingArchetypes[panelIndex].color1;
            }
            else
            {
                packColor = matchingArchetypes[panelIndex].color2;
            }
            Debug.Log("This pack's color is: " + packColor);
        GameObject manaPanel = packHeaderPanel.transform.Find("Pack Mana Panel").gameObject;
        manaPanel.transform.GetComponentInChildren<TextMeshProUGUI>().text = packColor;
    }

    private void AddButtonListeners()
    {
        foreach(GameObject manaChoicePanel in manaChoicePanels)
        {
            Button button = manaChoicePanel.GetComponentInChildren<Button>();
            button.onClick.AddListener(() => OnColorButtonClick(button.name));
        }
    }


    private void GetArchetypeHeaders(SaveHandler saveHandler)
    {
        //Find all the header panels and add them to the list


        //Set the text of each header to the archetype name
        for(int i = 0; i < matchingArchetypes.Count; i++)
        {
            //Pick whichever color is not the filter color in the archetype
            //This will be the color of the pack (packColor)
            if(matchingArchetypes[i].color1 != currentFilterColor)
            {
                packColor = matchingArchetypes[i].color1;
            }
            else
            {
                packColor = matchingArchetypes[i].color2;
            }

            //Find the panel holding the color letter
            //GameObject manaPanel = pack
            Debug.Log(matchingArchetypes[i].archetypeName);
            Debug.Log("This pack's color is: " + packColor);
            packPanels[i].GetComponentInChildren<TextMeshProUGUI>().text = matchingArchetypes[i].archetypeName;
        }
    }
}
