using UnityEngine;
using UnityEngine.UI;

public class PanelColorHandler : MonoBehaviour
{
    void Start()
    {
        UpdateBarColors("emptyList");
    }

    public void UpdateBarColors(string currentState)
    {

        Image bar1 = transform.Find("Bar 1").GetComponent<Image>(); // Find bar1 within the panel
        GameObject bar2Object = transform.Find("Bar 2").gameObject; //bar2 GameObject for enabling/disabling
        Image bar2 = transform.Find("Bar 2").GetComponent<Image>(); // Find bar2 within the panel

        if(bar1 == null)
        {
            Debug.Log("Bar 1 not found on " + gameObject.name);
        }
        if(bar2 == null)
        {
            Debug.Log("Bar 2 not found on " + gameObject.name);
        }
        
        //Divide each value by 255 to make Unity happy
        Color redBar = new Color(65f/255f, 25f/255f, 5f/255f, 177f/255f);
        Color yellowBar = new Color(159f/255f, 123f/255f, 34f/255f, 177f);
        Color greenBar = new Color(48f/255f, 159f/255f, 34f/255f, 177f/255f);

        switch (currentState)
        {
            case "emptyList": 
                bar1.fillCenter = false;
                bar1.color = redBar;
                bar2Object.SetActive(true);
                break;
            case "oneItemInList":
                bar1.fillCenter = true;
                bar1.color = yellowBar;
                bar2Object.SetActive(true);
                break;
            case "fullList":
                bar1.fillCenter = true;
                bar1.color = greenBar;
                bar2Object.SetActive(false);
                break;
        }
    }
}