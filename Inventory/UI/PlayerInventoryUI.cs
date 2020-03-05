using UnityEngine;
using UnityEngine.UI;
using System.ComponentModel;
using System.Collections;
using UnityEngine.EventSystems;

[System.Serializable]
public class UISlot
{
    public RectTransform transform;
    public Slot slot;
    public UISlot() { }
    public UISlot(RectTransform t, Slot s)
    {
        transform = t;
        slot = s;
    }

}

public class PlayerInventoryUI : WindowUI {

    private PlayerInventory _playerInventory;
    public Sprite emptyIcon;
    public GameObject inventoryPanel;
    public TooltipUI tooltip;

    private UISlot[,] inventorySlots;

    //Equipment UI Slots

    [SerializeField]
    private UISlot MainhandSlot = new UISlot();
    [SerializeField]
    private UISlot OffhandSlot = new UISlot();
    [SerializeField]
    private UISlot HelmetSlot = new UISlot();
    [SerializeField]
    private UISlot ChestSlot = new UISlot();
    [SerializeField]
    private UISlot BootsSlot = new UISlot();
    [SerializeField]
    private UISlot Artifact1Slot = new UISlot();
    [SerializeField]
    private UISlot Artifact2Slot = new UISlot();
    [SerializeField]
    private UISlot Artifact3Slot = new UISlot();
    [SerializeField]
    private UISlot Artifact4Slot = new UISlot();

    // Use this for initialization
    void Start() {
        _playerInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>();
        _playerInventory.PropertyChanged += OtherPropertyChangedHandler;
        _playerInventory.InventoryChanged += InventoryChangedHandler;
        _playerInventory.EquipmentChanged += EquipmentChangedHandler;

        inventorySlots = new UISlot[PlayerInventory.InventoryRows, PlayerInventory.InventoryCols];

        Transform slot;
        for (int row = 0; row < PlayerInventory.InventoryRows; row++)
        {
            for (int col = 0; col < PlayerInventory.InventoryCols; col++)
            {
                slot = inventoryPanel.transform.GetChild((row * PlayerInventory.InventoryCols) + col);
                inventorySlots[row, col] = new UISlot(slot.GetComponent<RectTransform>(), _playerInventory.Inventory[row, col]);

                //add click listener for swap
                slot.GetComponent<Button>().onClick.AddListener(SwapListener(row, col));

                //add event listeners for tooltip
                EventTrigger trigger = slot.GetComponent<EventTrigger>();
                EventTrigger.Entry enter = new EventTrigger.Entry();
                enter.eventID = EventTriggerType.PointerEnter;
                enter.callback.AddListener(ShowToolTipListener(inventorySlots[row, col]));
                trigger.triggers.Add(enter);
                EventTrigger.Entry leave = new EventTrigger.Entry();
                leave.eventID = EventTriggerType.PointerExit;
                leave.callback.AddListener((eventData) => { CloseTooltip(); });
                trigger.triggers.Add(leave);
            }
        }

        //Transforms for the equipment slots are manually set in inspector
        //Setup the matching slot references
        MainhandSlot.slot = _playerInventory.MainhandSlot;
        OffhandSlot.slot = _playerInventory.OffhandSlot;
        HelmetSlot.slot = _playerInventory.HelmetSlot;
        ChestSlot.slot = _playerInventory.ChestSlot;
        BootsSlot.slot = _playerInventory.BootsSlot;
        Artifact1Slot.slot = _playerInventory.Artifact1Slot;
        Artifact2Slot.slot = _playerInventory.Artifact2Slot;
        Artifact3Slot.slot = _playerInventory.Artifact3Slot;
        Artifact4Slot.slot = _playerInventory.Artifact4Slot;

        //create array to make setup easier
        UISlot[] equipSlots = new UISlot[] { MainhandSlot, OffhandSlot, HelmetSlot, ChestSlot, BootsSlot, Artifact1Slot, Artifact2Slot, Artifact3Slot, Artifact4Slot };

        int ArtifactCount = 0;
        for (int i = 0; i < equipSlots.Length; i++)
        {
            if (equipSlots[i].slot.type == SlotType.Artifact)
            {
                ArtifactCount += 1;
            }
            //Add on click listener
            equipSlots[i].transform.GetComponent<Button>().onClick.AddListener(EquipListener(equipSlots[i].slot, ArtifactCount));

            //Add event listeners
            EventTrigger trigger = equipSlots[i].transform.GetComponent<EventTrigger>();
            EventTrigger.Entry enter = new EventTrigger.Entry();
            enter.eventID = EventTriggerType.PointerEnter;
            enter.callback.AddListener(ShowToolTipListener(equipSlots[i]));
            trigger.triggers.Add(enter);
            EventTrigger.Entry leave = new EventTrigger.Entry();
            leave.eventID = EventTriggerType.PointerExit;
            leave.callback.AddListener((eventData) => { CloseTooltip(); });
            trigger.triggers.Add(leave);
        }

        //hide panel initially
        CloseWindow();
	}

    /// <summary>
    /// Closure for lambda expression so that variable references aren't overwritten
    /// </summary>
    /// <param name="t">transform of slot</param>
    /// <param name="i">item in slot</param>
    /// <returns>lambda expression to call show tooltip</returns>
    private UnityEngine.Events.UnityAction<BaseEventData> ShowToolTipListener(UISlot slot)
    {
        return (eventData) => { ShowTooltip(slot); };
    }

    /// <summary>
    /// Closure for lambda expression so that variable references aren't overwritten
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <returns>lambda expression to call Swap</returns>
    private UnityEngine.Events.UnityAction SwapListener(int row, int col)
    {
        return () => { _playerInventory.Swap(row, col); };
    }

    /// <summary>
    /// Closure for lambda expression so that variable references aren't overwritten
    /// </summary>
    /// <param name="s">slot</param>
    /// <param name="slotNum">for slots which have the same type, this designates which slot number is being equipped</param>
    /// <returns>lambda expression to call Equip</returns>
    private UnityEngine.Events.UnityAction EquipListener(Slot s, int slotNum)
    {
        return () => { _playerInventory.Equip(s, slotNum); };
    }

    /// <summary>
    /// Called when the held item changes, either to nothing or to something
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OtherPropertyChangedHandler(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case "HeldItem":
                if (_playerInventory.HeldItem == null)
                {
                    Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                }
                else
                {
                    Sprite icon = _playerInventory.HeldItem.icon;
                    Texture2D cursorTex = new Texture2D((int)icon.rect.width, (int)icon.rect.height, TextureFormat.ARGB32, false);
                    var pixels = icon.texture.GetPixels((int)icon.textureRect.x,
                                                        (int)icon.textureRect.y,
                                                        (int)icon.textureRect.width,
                                                        (int)icon.textureRect.height);
                    cursorTex.SetPixels(pixels);
                    cursorTex.Apply();
                    cursorTex.alphaIsTransparency = true;
                    Cursor.SetCursor(cursorTex, Vector2.zero, CursorMode.Auto);
                }
                break;
        }
    }

    /// <summary>
    /// Called when an item is added or removed from an inventory slot
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void InventoryChangedHandler(object sender, PropertyChangedEventArgs e)
    {
        string[] info = e.PropertyName.Split('^');
        if (info.Length < 2) return;
        int row = int.Parse(info[0]);
        int col = int.Parse(info[1]);


        if (inventorySlots[row, col].transform.childCount > 0) //if there was already an item there, remove UI for it
        {
            GameObject invItem = inventorySlots[row, col].transform.GetChild(0).gameObject;
            ObjectPool.Instance.ReturnToPool(invItem);
        }

        if (_playerInventory.Inventory[row, col].isOccupied)    //there is now an item here so checkout item from pool and add to lead slot
        {
            if (_playerInventory.Inventory[row, col].IsLeadSlot())
            {
                GameObject invItem = ObjectPool.Instance.CheckOut("InventoryItem");
                invItem.transform.SetParent(inventorySlots[row, col].transform);
                invItem.GetComponent<ItemUI>().SetItem(_playerInventory.Inventory[row, col].item);
            }
        }
    }

    /// <summary>
    /// equipment slot item changed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void EquipmentChangedHandler(object sender, PropertyChangedEventArgs e)
    {
        string[] info = e.PropertyName.Split('^');
        SlotType type = (SlotType)int.Parse(info[0]);
        UISlot equipSlot = null;
        switch (type)
        {
            case SlotType.Mainhand:
                equipSlot = MainhandSlot;
                break;
            case SlotType.Offhand:
                equipSlot = OffhandSlot;
                break;
            case SlotType.Helmet:
                equipSlot = HelmetSlot;
                break;
            case SlotType.Chest:
                equipSlot = ChestSlot;
                break;
            case SlotType.Boots:
                equipSlot = BootsSlot;
                break;
            case SlotType.Artifact:
                switch (int.Parse(info[1]))
                {
                    case 1:
                        equipSlot = Artifact1Slot;
                        break;
                    case 2:
                        equipSlot = Artifact2Slot;
                        break;
                    case 3:
                        equipSlot = Artifact3Slot;
                        break;
                    case 4:
                        equipSlot = Artifact4Slot;
                        break;
                }
                break;
        }

        if (equipSlot == null) return;

        if (equipSlot.transform.childCount > 0) //if there was already an item there, remove UI for it
        {
            GameObject oldItem = equipSlot.transform.GetChild(0).gameObject;
            ObjectPool.Instance.ReturnToPool(oldItem);
        }

        if (equipSlot.slot.isOccupied)    //if there is an item in slot add UI for it
        {
            GameObject invItem = ObjectPool.Instance.CheckOut("InventoryItem");
            invItem.transform.SetParent(equipSlot.transform);
            invItem.GetComponent<ItemUI>().SetItem(equipSlot.slot.item);
        }
    }

    /// <summary>
    /// Makes tooltip visible, should be called from mouse hovering over item
    /// </summary>
    /// <param name="uiSlot">inventory slot being hovered over</param>
    private void ShowTooltip(UISlot uiSlot)
    {
        if (uiSlot.slot.isOccupied)
        {
            tooltip.ShowTooltip(uiSlot);
        }
    }

    /// <summary>
    /// Makes tooltip hidden, should be called when mouse exits from hovering an item
    /// </summary>
    private void CloseTooltip()
    {
        tooltip.CloseTooltip();
    }
}
