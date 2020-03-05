using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ItemUI : MonoBehaviour {

    public int slotWidth;
    public int slotHeight;

    private Item _item;
    [SerializeField]
    private Image _image;

	// Use this for initialization
	void Start () {
        _image = GetComponent<Image>();
	}
	
    /// <summary>
    /// Designate the item being displayed
    /// </summary>
    /// <param name="item">the item</param>
	public void SetItem(Item item)
    {
        _item = item;
        _image.sprite = item.icon;
        _image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, slotWidth * _item.size.Cols());
        _image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, slotHeight * _item.size.Rows());
        _image.rectTransform.localScale = Vector3.one;
        _image.rectTransform.anchoredPosition = Vector3.zero;
    }
}
