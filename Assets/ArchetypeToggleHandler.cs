using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArchetypeToggleHandler : MonoBehaviour
{
    private Toggle toggle;
    public ArchetyperUI archetyperUI;
    void Awake()
    {
        toggle = GetComponent<Toggle>();
        Debug.Log("Toggle name: " + toggle.gameObject.name);
        toggle.onValueChanged.AddListener(OnToggleValueChanged);
    }

    public void SetArchetyperUI(ArchetyperUI archetyperUI)
    {
        this.archetyperUI = archetyperUI;
    }

    public void OnToggleValueChanged(bool isOn)
    {
        if (archetyperUI != null)
        {
            archetyperUI.OnArchetypeToggleClicked(toggle);
        }
    }
}
