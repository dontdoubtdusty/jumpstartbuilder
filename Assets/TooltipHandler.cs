using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TooltipHandler : MonoBehaviour
{
    public static TooltipHandler _instance;
    public TextMeshProUGUI tooltipText;

    void Awake()
    {
        if( _instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    void Start()
    {
        Cursor.visible = true;
        //Start the app with the tooltip disabled
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Input.mousePosition;
    }

    public void SetAndShowTooltip(string message)
    {
        gameObject.SetActive(true);
        tooltipText.text = message;
    }

    public void HideTooltip()
    {
        gameObject.SetActive(false);
        tooltipText.text = string.Empty;
    }
}
