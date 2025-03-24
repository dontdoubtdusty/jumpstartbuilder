using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string message;

    public void OnPointerEnter(PointerEventData eventData)
    {
        CardData cardData = GetComponent<CardData>();
        if(cardData.oracle_text != null)
        {
        message = cardData.oracle_text;
                //Debug.Log("Mouse entered button!");
        TooltipHandler._instance.SetAndShowTooltip(message);
        }


    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("Mouse exited button!");
        TooltipHandler._instance.HideTooltip();
    }
}
