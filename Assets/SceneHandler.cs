using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{
    public void LoadDeckBuilderScreen()
    {
        SceneManager.LoadScene("Deck Builder Screen");
    }

    public void LoadCardInputScreen()
    {
        SceneManager.LoadScene("Card Input Screen");
    }

    public void LoadArchetyperScreen()
    {
        SceneManager.LoadScene("Archetyper Screen");
    }
    public void LoadMainMenuScreen()
    {
        SceneManager.LoadScene("Main Menu Screen");
    }
    
    public void LoadDeckCollectionScreen()
    {
        SceneManager.LoadScene("Deck Collection Screen");
    }
}
