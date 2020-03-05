using UnityEngine;
using System.Collections.Generic;
using System.ComponentModel;

public class PlayerInventory : MonoBehaviour, INotifyPropertyChanged {

    public const int InventoryRows = 5;
    public const int InventoryCols = 12;

    //notifiers
    public event PropertyChangedEventHandler PropertyChanged;
    /// <summary>
    /// Event raised whenever an item in an inventory slot changes (added or removed)
    /// </summary>
    public event PropertyChangedEventHandler InventoryChanged;
    /// <summary>
    /// Event raised prior to removing a piece of equipment
    /// </summary>
    public event PropertyChangedEventHandler EquipmentRemoving;
    /// <summary>
    /// Event raised after equipment slot is changed
    /// </summary>
    public event PropertyChangedEventHandler EquipmentChanged;
    private void OnPropertyChanged(string info)
    {
        PropertyChangedEventHandler handler = PropertyChanged;
        if (handler != null) handler(this, new PropertyChangedEventArgs(info));
    }
    private void OnInventoryChanged(int row, int col)
    {
        PropertyChangedEventHandler handler = InventoryChanged;
        if (handler != null) handler(this, new PropertyChangedEventArgs(row + "^" + col));
    }
    /// <summary>
    /// Should be called prior to removing a piece of equipment
    /// </summary>
    /// <param name="info"></param>
    private void OnEquipmentRemoving(string info)
    {
        PropertyChangedEventHandler handler = EquipmentRemoving;
        if (handler != null) handler(this, new PropertyChangedEventArgs(info));
    }
    /// <summary>
    /// Should be called right after adding a piece of equipment
    /// </summary>
    /// <param name="info"></param>
    private void OnEquipmentChanged(string info)
    {
        PropertyChangedEventHandler handler = EquipmentChanged;
        if (handler != null) handler(this, new PropertyChangedEventArgs(info));
    }

    /// <summary>
    /// Storage for items held in inventory
    /// </summary>
    private InventorySlot[,] _inventory  = new InventorySlot[InventoryRows, InventoryCols];  //
    public InventorySlot[,] Inventory { get { return _inventory; } private set { _inventory = value; } }

    public Slot MainhandSlot { get; private set; }
    public Slot OffhandSlot { get; private set; }
    public Slot HelmetSlot { get; private set; }
    public Slot ChestSlot { get; private set; }
    public Slot BootsSlot { get; private set; }
    public Slot Artifact1Slot { get; private set; }
    public Slot Artifact2Slot { get; private set; }
    public Slot Artifact3Slot { get; private set; }
    public Slot Artifact4Slot { get; private set; }

    private Item _heldItem;
    public Item HeldItem
    {
        get
        {
            return _heldItem;
        }
        private set
        {
            _heldItem = value;
            OnPropertyChanged("HeldItem");
        }
    }

	// Use this for initialization
	void Awake () {
        ItemDatabaseManager.Instance.Init();   //forces database to load, this will go somewhere else once loading screens are written
        for(int row = 0; row < InventoryRows; row++)
        {
            for (int col = 0; col < InventoryCols; col++)
            {
                _inventory[row, col] = new InventorySlot();
            }
        }
        MainhandSlot = new Slot(SlotType.Mainhand);
        OffhandSlot = new Slot(SlotType.Offhand);
        HelmetSlot = new Slot(SlotType.Helmet);
        ChestSlot = new Slot(SlotType.Chest);
        BootsSlot = new Slot(SlotType.Boots);
        Artifact1Slot = new Slot(SlotType.Artifact);
        Artifact2Slot = new Slot(SlotType.Artifact);
        Artifact3Slot = new Slot(SlotType.Artifact);
        Artifact4Slot = new Slot(SlotType.Artifact);
    }

    /// <summary>
    /// Checks if the inventory slots starting as this slot have enough room for the item
    /// </summary>
    /// <param name="row">inventory row of lead slot</param>
    /// <param name="col">inventory column of lead slot</param>
    /// <param name="item">item being checked</param>
    /// <returns></returns>
    private bool CheckSlotsForSize(int row, int col, Item item)
    {
        if (row + item.size.Rows() > InventoryRows) return false;
        if (col + item.size.Cols() > InventoryCols) return false;
        return true;
    }

    /// <summary>
    /// Checks if the inventory slots are already occupied
    /// </summary>
    /// <param name="row">row of the lead slot</param>
    /// <param name="col">column of the lead slot</param>
    /// <param name="item">item wanting to be placed</param>
    /// <param name="itemCount">output variable, the number of unique items that occupy slots in this range</param>
    /// <returns>True if slots are available for this item, false otherwise</returns>
    private bool CheckSlotsForAvailability(int row, int col, Item item, out int itemCount)
    {
        List<int> uniqueIDs = new List<int>();
        itemCount = 0;
        bool available = true;
        for (int i = row; i < (row + item.size.Rows()); i++ )
        {
            for (int j = col; j < (col + item.size.Cols()); j++)
            {
                if (Inventory[i, j].isOccupied)
                {
                    available = false;
                    if (!uniqueIDs.Contains(Inventory[i, j].item.uniqueID))
                    {
                        uniqueIDs.Add(Inventory[i, j].item.uniqueID);
                        itemCount += 1;
                    }
                }
            }
        }
        return available;
    }

    /// <summary>
    /// Adds given item to this slot and all others in size
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <param name="item"></param>
    private void AddItemToSlots(int row, int col, Item item)
    {
        int rowOffset = 0;
        for (int i = row; i < (row + item.size.Rows()); i++)
        {
            int colOffset = 0;
            for (int j = col; j < (col + item.size.Cols()); j++)
            {
                Inventory[i, j].AddInventoryItem(item, rowOffset, colOffset);
                OnInventoryChanged(i, j);
                colOffset += 1;
            }
            rowOffset += 1;
        }
    }

    /// <summary>
    /// Removes item from this slot and other slots it is occupying
    /// </summary>
    /// <param name="row">row of a slot containing the item</param>
    /// <param name="col">col of a slot containing the item</param>
    /// <returns>item in the slot</returns>
    private Item RemoveItemFromSlots(int row, int col)
    {
        if (!Inventory[row, col].IsLeadSlot())
        {
            row = row - Inventory[row, col].RowOffset;
            col = col - Inventory[row, col].ColOffset;
        }
        Item item = Inventory[row,col].item;
        if (item == null) return null;

        for (int i = row; i < (row + item.size.Rows()); i++)
        {
            for (int j = col; j < (col + item.size.Cols()); j++)
            {
                Inventory[i, j].RemoveItem();
                OnInventoryChanged(i, j);
            }
        }
        return item;
    }

#region Public Methods
    /// <summary>
    /// Add given item to first available area in inventory
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool AddItem(Item item)
    {
        //find first open slot for an item
        for (int row = 0; row < (InventoryRows - item.size.Rows() + 1); row++)
        {
            for (int col = 0; col < (InventoryCols - item.size.Cols() + 1); col++)
            {
                int itemCount;
                if (CheckSlotsForSize(row, col, item) && CheckSlotsForAvailability(row, col, item, out itemCount))
                {
                    AddItemToSlots(row, col, item);
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Swap held item for item at index
    /// This can also be used when held item is null or clicking on empty slot
    /// </summary>
    /// <param name="row">selected row</param>
    /// <param name="col">selected column</param>
    public void Swap(int row, int col)
    {
        if (HeldItem == null)   //just grabbing what is there
        {
            HeldItem = RemoveItemFromSlots(row, col);
        }
        else  //placing held item there
        {
            if (CheckSlotsForSize(row, col, HeldItem))
            {
                int itemCount;
                if (CheckSlotsForAvailability(row, col, HeldItem, out itemCount))
                {
                    //there was nothing in these slots previously, so just add the helditem here
                    AddItemToSlots(row, col, HeldItem);
                    HeldItem = null;
                }
                else
                {
                    //there was something here, if only 1 item swap held item for it
                    if (itemCount == 1)
                    {
                        Item temp = HeldItem;
                        //TODO: find a better way to do this than relooping all the slots
                        //find the one item in these slots and remove it
                        for (int i = row; i < (row + HeldItem.size.Rows()); i++)
                        {
                            for (int j = col; j < (col + HeldItem.size.Cols()); j++)
                            {
                                if (Inventory[i, j].isOccupied)
                                {
                                    HeldItem = RemoveItemFromSlots(i, j);
                                }
                            }
                        }
                        AddItemToSlots(row, col, temp);
                    }
                }
            }
        }
    }
    /// <summary>
    /// Equip has seperate function to swap models on character and ensure slot only gets valid items
    /// </summary>
    /// <param name="index"></param>
    public void Equip(Slot equipSlot, int slotNum)
    {
        if(HeldItem == null)
        {
            if (equipSlot.isOccupied) //grab equipped item
            {
                OnEquipmentRemoving(((int)equipSlot.type).ToString() + "^" + slotNum);
                HeldItem = equipSlot.RemoveItem();
                OnEquipmentChanged(((int)equipSlot.type).ToString() + "^" + slotNum);
            }
        }
        else
        {
            if (equipSlot.ValidateSlot(HeldItem))
            {
                Item temp = null;
                if (equipSlot.isOccupied)
                {
                    OnEquipmentRemoving(((int)equipSlot.type).ToString() + "^" + slotNum);
                    temp = equipSlot.RemoveItem();
                }
                equipSlot.AddItem(HeldItem);
                OnEquipmentChanged(((int)equipSlot.type).ToString() + "^" + slotNum);
                HeldItem = temp;
            }
        }
    }

    public void DropHeldItem()
    {
        if(HeldItem != null)    //double check
        { 
            GameObject loot = ObjectPool.Instance.CheckOut("Loot");
            loot.GetComponent<Loot>().lootItem = HeldItem;
            loot.transform.position = transform.position + new Vector3(0, 2, 0) + transform.forward;
            HeldItem = null;
        }
    }

    /// <summary>
    /// Returns the Slot corresponding to the given type.
    /// Used for converting the event string for equipment removing/changed.
    /// </summary>
    /// <param name="type">type of item looking for the slot for</param>
    /// <param name="slotNum">this is used for artifact pieces to determine which artifact slot to return (default artifact slot 1)</param>
    /// <returns></returns>
    public Slot GetEquipmentSlotByType(SlotType type, int slotNum)
    {
        switch (type)
        {
            case SlotType.Mainhand:
                return MainhandSlot;
            case SlotType.Offhand:
                return OffhandSlot;
            case SlotType.Helmet:
                return HelmetSlot;
            case SlotType.Chest:
                return ChestSlot;
            case SlotType.Boots:
                return BootsSlot;
            case SlotType.Artifact:
                switch (slotNum)
                {
                    case 1:
                    default:
                        return Artifact1Slot;
                    case 2:
                        return Artifact2Slot;
                    case 3:
                        return Artifact3Slot;
                    case 4:
                        return Artifact4Slot;
                }
            default:
                return null;
        }
    }
    #endregion
}
