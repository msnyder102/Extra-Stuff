using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TooltipUI : MonoBehaviour {


    private RectTransform _myTransform;
    private Canvas _myCanvas;
    private Canvas _mainCanvas;
    private CharacterStats _characterStats;
    public Text nameText;
    public Text descText;
    public RectTransform propertiesPanel;

	// Use this for initialization
	void Awake () {
        _myTransform = GetComponent<RectTransform>();
        _myCanvas = GetComponent<Canvas>();
        _mainCanvas = _myCanvas.rootCanvas;
        _characterStats = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterStats>();
        gameObject.SetActive(false);
	}

    /// <summary>
    /// Positions and makes tooltip visible showing items information
    /// </summary>
    /// <param name="uiSlot">slot being hovered</param>
    public void ShowTooltip(UISlot uiSlot)
    {
        //activate
        gameObject.SetActive(true);

        //setup position
        Vector3[] corners = new Vector3[4];
        uiSlot.transform.GetWorldCorners(corners);
        _myTransform.anchoredPosition = new Vector2(corners[1].x, corners[1].y) * (1 / _mainCanvas.scaleFactor);
        if (uiSlot.slot is InventorySlot)
        {
            int rowOffset = ((InventorySlot)uiSlot.slot).RowOffset;
            int colOffset = ((InventorySlot)uiSlot.slot).ColOffset;
            _myTransform.anchoredPosition -= new Vector2((int)uiSlot.transform.sizeDelta.x * colOffset, -(int)uiSlot.transform.sizeDelta.y * rowOffset);
        }

        Item item = uiSlot.slot.item;
        //set tooltip info
        nameText.text = item.itemName;
        descText.text = item.description;
        //properties
        for (int i = 0; i < propertiesPanel.childCount; i++)
        {
            if(i >= item.statModifiers.Count)
            {
                //hide this property
                propertiesPanel.GetChild(i).GetChild(0).gameObject.SetActive(false);
                propertiesPanel.GetChild(i).GetChild(1).gameObject.SetActive(false);
            }
            else
            {
                propertiesPanel.GetChild(i).GetChild(0).gameObject.SetActive(true);
                propertiesPanel.GetChild(i).GetChild(1).gameObject.SetActive(true);
                Text propertyText = propertiesPanel.GetChild(i).GetChild(1).GetComponent<Text>();
                propertyText.text = item.statModifiers[i].Value + " to " + _characterStats.Stats[(int)item.statModifiers[i].Type].Label();
            }
        }
    }

    public void CloseTooltip()
    {
        gameObject.SetActive(false);
    }
}
