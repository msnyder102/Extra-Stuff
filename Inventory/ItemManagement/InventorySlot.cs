using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySlot : Slot {

    /// <summary>
    /// Row offset from the lead slot for the group of slots containing this item
    /// </summary>
    public int RowOffset { get; private set; }
    /// <summary>
    /// Column offset from the lead slot for the group of slots containing this item
    /// </summary>
    public int ColOffset { get; private set; }

    public InventorySlot() : base(SlotType.Inventory)
    {
        RowOffset = 0;
        ColOffset = 0;
    }

    /// <summary>
    /// Adds item to slot
    /// Will not add the item if it is invalid for the slot or if the slot is already occupied
    /// Caller needs to handle removing the previous item themselves
    /// </summary>
    /// <param name="item"></param>
    /// <returns>
    /// True if item was added to slot
    /// False otherwise
    /// </returns>
    public bool AddInventoryItem(Item item, int rowOff, int colOff)
    {
        if (base.AddItem(item))
        {
            RowOffset = rowOff;
            ColOffset = colOff;
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Removes item currently occupying the slot
    /// </summary>
    /// <returns>The item previously in the slot</returns>
    new public Item RemoveItem()
    {
        RowOffset = 0;
        ColOffset = 0;
        return base.RemoveItem();
    }

    /// <summary>
    /// Returns whether this inventory slot is the lead slot (upperleft most slot containing the same item)
    /// Empty slots will return false
    /// </summary>
    /// <returns>True if lead slot, false otherwise</returns>
    public bool IsLeadSlot()
    {
        if (base.isOccupied && RowOffset == 0 && ColOffset == 0) return true;
        else return false;
    }
}
